namespace BookMate.Processing

module EpubProcessor = 
    open BookMate.Helpers
    open IOHelper
    open EpubSharp
    open System.IO
    open HtmlAgilityPack
    
    let prepareForExport = 
        Array.Parallel.map (fun (k, v) -> sprintf "%s %i %s" k v System.Environment.NewLine) >> String.concat (@"")
    
    let prepareForExport2 (s : (string * string) []) = 
        s
        |> Array.Parallel.map (fun (k, v) -> (k.Trim(), v.Trim()))
        |> Array.Parallel.map (fun (k, v) -> sprintf "%s, %s%s" k (v) System.Environment.NewLine)
        |> String.concat (@"")
    
    let wrireStatsToFile stat path name = 
        let statString = stat |> prepareForExport
        do IOHelper.writeToFile (path +/ name + "-stats.txt") statString
    
    let loadBook bookPath = 
        match bookPath with
        | "" | null -> failwith "Can't use empty file path to load the book"
        | path -> 
            match File.Exists(path) with
            | true -> 
                let fileName = getFileNameWithoutExtension path
                let book = EpubReader.Read(bookPath)
                let text = book.ToPlainText()
                printfn "Loaded %s" fileName
                text
            | _ -> failwith "File does not exist."
    
    let downloadTranslations (toQuery : string []) = 
        let chunkSize = 30
        //progress update
        let progress = (float32 toQuery.Length) / 30.0f
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let splitByChuncks = 
            Array.chunkBySize chunkSize
            >> Array.map (Array.reduce StringHelper.stringify)
            >> Array.ofSeq
        
        let asyncParalleldownload = 
            Array.Parallel.mapi 
                (fun i p -> (p, (YandexHelper.askYaTranslateAsyncf p updateDownloadProgress) |> Async.RunSynchronously))
        
        let form (arg : (string * string []) []) = 
            arg
            |> Array.Parallel.map (fun (p, a) -> (StringHelper.unstringify p, a))
            |> Array.Parallel.map (fun (p, a) -> (p, a, p.Length = a.Length))
            |> Array.Parallel.map (fun (p, a, se) -> 
                   if (se) then (Seq.zip p a)
                   else Seq.empty<string * string>)
            |> Seq.concat
            |> Array.ofSeq
        toQuery
        |> splitByChuncks
        |> asyncParalleldownload
        |> form
    
    let downloadManyTranslations (toQuery : string []) = 
        //progress update
        printfn ""
        let progress = (float32 toQuery.Length)
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading dictionary translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let asyncParalleldownload = 
            Array.Parallel.mapi 
                (fun i p -> (p, (YandexHelper.askYaDictionaryAsyncf p updateDownloadProgress) |> Async.RunSynchronously))
        
        let oneToMany (ys : string []) (x : string) = 
            x
            |> Array.replicate ys.Length
            |> Array.zip ys
        toQuery
        |> asyncParalleldownload
        |> Array.Parallel.map (fun (x, y) -> oneToMany y x)
        |> Array.concat
        |> Array.Parallel.map (fun (x, y) -> (y, x))
    
    let processBook bookPath = 
        let fileName = getFileNameWithoutExtension bookPath
        let path = getDirectoryName bookPath
        printfn "Processing %s" fileName
        //analyze book's content
        let text = loadBook bookPath
        let stat = AnalyseHelper.getWordStats text
        let forExportStat = stat |> prepareForExport
        do IOHelper.writeToFile (IOHelper.op_PlusDivide path (sprintf "%s-Stat.txt" fileName)) forExportStat
        let uniqueRare = 
            stat
            |> Array.where (fun (k, v) -> v < 20)
            |> Array.Parallel.map (fun (k, v) -> k)
            |> Array.distinct
        
        let storedTranslationsForBook = DBHelper.loadFromDB
        let toQuery = uniqueRare |> Array.except (storedTranslationsForBook |> Array.Parallel.map (fun (x, y) -> x))
        let downloadedTranslations = downloadManyTranslations (toQuery |> Array.take 0) //remove truncate 
        let allTranslations = downloadedTranslations |> Array.append (storedTranslationsForBook)
        
        let toExportTranslation = 
            allTranslations
            |> Array.Parallel.map (fun (x, y) -> (x.Trim(), y.Trim()))
            |> Array.distinct
            |> Array.where (fun (x, y) -> x <> y)
            //  |> Array.except (storedTranslationsForBook)
            |> prepareForExport2
        do IOHelper.writeToFile (sprintf @"%s\%s-translations.txt" path fileName) toExportTranslation
        //group and stringify!
        let dic = 
            storedTranslationsForBook
            |> Array.where (fun (x, y) -> x.Length > 3)
            |> Array.groupBy (fun (x, y) -> x)
            |> Array.Parallel.map (fun (x, y) -> 
                   (x, 
                    y
                    |> Array.map (fun (_, d) -> d)
                    |> Seq.truncate 3
                    |> Seq.reduce StringHelper.stringify))
            |> Map.ofArray
        
        let translationsToUse = 
            uniqueRare
            |> Array.where (fun x -> x.Length > 3)
            |> Array.Parallel.map (fun x -> (x, dic.TryFind(x)))
            |> Array.where (fun (_, y) -> y.IsSome)
            |> Array.Parallel.map (fun (x, y) -> (x, y.Value))
            |> Map.ofArray
        
        //creating folder for processing
        let tmpFolderName = sprintf "tmp_%s" (System.Guid.NewGuid().ToString())
        let tmpPath = IOHelper.op_PlusDivide path tmpFolderName
        createFolder tmpPath |> ignore
        let tmpFileName = IOHelper.op_PlusDivide tmpPath (fileName + ".zip")
        printfn "Created tmp folder %s..." tmpPath
        //creating zip and ucompress it
        do File.Copy(bookPath, tmpFileName)
        do File.SetAttributes(tmpFileName, FileAttributes.Normal) //needed to allow deleting of this file
        let tmpUnCompressedBookPath = IOHelper.op_PlusDivide tmpPath fileName
        createFolder (tmpUnCompressedBookPath) |> ignore
        do unzipFile tmpFileName tmpUnCompressedBookPath
        //analyzing contents
        //not all EPUB file have appropriate structure so check if OEBPS folder exists and if it does not then look for content file in the root 
        let workingPath = 
            if (Directory.Exists(tmpUnCompressedBookPath +/ "OEBPS")) then tmpUnCompressedBookPath +/ "OEBPS"
            else tmpUnCompressedBookPath
        
        let contentFiles = Directory.GetFiles(workingPath, "*.*html")
        printfn "Discovered file structure:"
        for f in contentFiles do
            printfn "-%s" f
        //actual translation here
        for i in 0..contentFiles.Length - 1 do
            let processingFileName = workingPath +/ contentFiles.[i]
            let html = new HtmlAgilityPack.HtmlDocument()
            html.OptionWriteEmptyNodes <- true
            html.OptionOutputAsXml <- true
            do html.Load(processingFileName, System.Text.Encoding.UTF8)
            let textNodes = html.DocumentNode.SelectNodes("//p//text()")
            if not (isNull textNodes) then 
                let mutable inx = 1
                for p in textNodes do
                    let progressPercent = 
                        (int 
                             (100.0f 
                              * (((((float32 inx) / (float32 textNodes.Count))) * (float32 i + 1.0f)) 
                                 / (float32 contentFiles.Length))))
                    printf "\rProcessing:%i%%" progressPercent
                    if p :? HtmlTextNode then 
                        let node = p :?> HtmlTextNode
                        let t = node.Text.Trim()
                        if t <> "" && t <> "\r" && t <> "\r\n" && t <> "\n" then 
                            let mutable text = node.Text
                            for (k) in translationsToUse do
                                if text.Contains(k.Key) then 
                                    let pattern = @"\b" + k.Key + @"\b"
                                    let replace = k.Key + "{" + k.Value + "}"
                                    if RegexHelper.isMatch pattern text then 
                                        node.Text <- RegexHelper.regexReplace text pattern replace
                    inx <- inx + 1
            else 
                let progressPercent = (int (100.0f * (float32 i + 1.0f) / (float32 contentFiles.Length)))
                printf "\rProcessing:%i%%" progressPercent
            let mutable result = null
            use writer = new StringWriter()
            html.Save(writer)
            result <- writer.ToString()
            IOHelper.writeToFile processingFileName result
        //preparing new epub file
        let newFileName = path +/ (sprintf "%s-translated.epub" fileName)
        do zipFile tmpUnCompressedBookPath newFileName
        //clean up
        printfn ""
        printfn "Cleaning up tmp folder."
        do deleteFiles tmpPath "*.*" true true
        printfn "The book has been processed."
