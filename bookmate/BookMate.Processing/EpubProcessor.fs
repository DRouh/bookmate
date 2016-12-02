namespace BookMate.Processing

module EpubProcessor = 
    open BookMate.Core.Helpers
    open BookMate.Core.Helpers.IOHelper
    open BookMate.Helpers
    open EpubSharp
    open HtmlAgilityPack
    open System.IO
    
    let private prepareForExport = 
        Array.Parallel.map (fun (k, v) -> sprintf "%s %i %s" k v System.Environment.NewLine) >> String.concat (@"")
    
    let private prepareForExport2 (s : (string * string) []) = 
        s
        |> Array.Parallel.map (fun (k, v) -> (k.Trim(), v.Trim()))
        |> Array.Parallel.map (fun (k, v) -> sprintf "%s, %s%s" k (v) System.Environment.NewLine)
        |> String.concat (@"")
    
    let private writeStatsToFile stat path name = 
        let statString = stat |> prepareForExport
        do IOHelper.writeToFile (path +/ name + "-stats.txt") statString
    
    let private loadBook bookPath = 
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
    
    let private downloadTranslations (toQuery : string []) = 
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
        printfn ""
        toQuery
        |> splitByChuncks
        |> asyncParalleldownload
        |> form
    
    let private downloadManyTranslations (toQuery : string []) = 
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
        printfn ""
        toQuery
        |> asyncParalleldownload
        |> Array.Parallel.map (fun (x, y) -> oneToMany y x)
        |> Array.concat
        |> Array.Parallel.map (fun (x, y) -> (y, x))
    
    let saveBookStats text storedTranslationsForBook bookPath = 
        let path = getDirectoryName bookPath
        let fileName = getFileNameWithoutExtension bookPath
        let stat = AnalyseHelper.getWordStats text
        let forExportStat = stat |> prepareForExport
        do IOHelper.writeToFile (path +/ (sprintf "%s-stat.txt" fileName)) forExportStat

        let uniqueWords = 
            stat
            |> Array.Parallel.map (fun (k, v) -> k)
            |> Array.distinct
        
        let toQuery = uniqueWords |> Array.except (storedTranslationsForBook |> Array.Parallel.map (fun (x, y) -> x))
        let downloadedTranslations = downloadManyTranslations (toQuery |> Array.take 0)
        let allTranslations = downloadedTranslations |> Array.append (storedTranslationsForBook)
        
        let toExportTranslation = 
            allTranslations
            |> Array.Parallel.map (fun (x, y) -> (x.Trim(), y.Trim()))
            |> Array.distinct
            |> Array.where (fun (x, y) -> x <> y)
            |> Array.except (storedTranslationsForBook)
            |> prepareForExport2
        do IOHelper.writeToFile (sprintf @"%s\%s-translations.txt" path fileName) toExportTranslation
    
    let processText (dict:Map<string,string>) (text:string) = 
        let mutable processedText = text
        let t = processedText.Trim()
        if t <> "" && t <> "\r" && t <> "\r\n" && t <> "\n" then 
            for k in dict do
                if processedText.Contains(k.Key) then 
                    let pattern = @"\b" + k.Key + @"\b"
                    let replace = k.Key + "{" + k.Value + "}"
                    if RegexHelper.isMatch pattern text then 
                        processedText <- RegexHelper.regexReplace text pattern replace
        processedText

    let processBook bookPath = 
        let fileName = getFileNameWithoutExtension bookPath
        let path = getDirectoryName bookPath
        printfn "Processing %s" fileName
        
        //analyze book's content
        let text = loadBook bookPath
        let wordStat = AnalyseHelper.getWordStats text
        let wordsToTranslate = wordStat |> Array.where (fun (k,v) -> v < 15) |> Array.Parallel.map (fun (k, v) -> k) |> Array.where (fun x -> x.Length > 3)
        let dbDictionary = BookMate.Integration.DBHelper.loadFromDB
        
        do saveBookStats <| text <| dbDictionary <| bookPath
        
        //group and stringify!
        let dic = 
            dbDictionary
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
            wordsToTranslate
            |> Array.Parallel.map (fun x -> (x, dic.TryFind(x)))
            |> Array.where (fun (_, y) -> y.IsSome)
            |> Array.Parallel.map (fun (x, y) -> (x, y.Value))
            |> Map.ofArray
        
        let (tmpUnCompressedBookPath, tmpPath) = 
            //creating folder for processing
            let tmpPath = path +/ sprintf "tmp_%s" (System.Guid.NewGuid().ToString())
            createFolder tmpPath |> ignore
            let tmpFileName = tmpPath +/ (fileName + ".zip")
            printfn "Created tmp folder %s..." tmpPath

            //creating zip and ucompress it
            do File.Copy(bookPath, tmpFileName)
            do File.SetAttributes(tmpFileName, FileAttributes.Normal) //needed to allow deleting of this file

            let tmpUnCompressedBookPath = tmpPath +/ fileName
            createFolder (tmpUnCompressedBookPath) |> ignore
            do unzipFile tmpFileName tmpUnCompressedBookPath
            (tmpUnCompressedBookPath, tmpPath)
        
        //analyzing contents
        //not all EPUB file have appropriate structure so check if OEBPS folder exists 
        //and if it doesn't then look for a content files in the root 
        let workingPath = 
            if (Directory.Exists(tmpUnCompressedBookPath +/ "OEBPS")) then tmpUnCompressedBookPath +/ "OEBPS"
            else tmpUnCompressedBookPath
        
        let contentFiles = Directory.GetFiles(workingPath, "*.*html")
        printfn "Discovered file structure:"
        for f in contentFiles do
            do printfn "-%s" <| getFileName f
        
        let translateProcesText = processText <| translationsToUse

        let updateProgress fileCount nodeCount fileInd nodeInd =
            let processedNodeProgress = (float32 nodeInd) / (float32 nodeCount)
            let prevFilesProgress = (float32 fileInd) / (float32 fileCount - 1.0f)
            let processedFilesProgress = (float32 fileInd + 1.0f) / (float32 fileCount)
            let progressPercent = int <| (50.0f * (prevFilesProgress + processedNodeProgress * processedFilesProgress))
            printf "\rProcessing:%i%%" progressPercent

        //actual translation here
        let filesToProcessCount = contentFiles.Length
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
                    do updateProgress filesToProcessCount textNodes.Count i inx
                    if p :? HtmlTextNode then 
                        let node = p :?> HtmlTextNode
                        node.Text <- translateProcesText <| node.Text
                    inx <- inx + 1
            else 
                do updateProgress filesToProcessCount 1 i 1
                let progressPercent = (int (100.0f * (float32 i + 1.0f) / (float32 contentFiles.Length)))
                printf "\rProcessing:%i%%" progressPercent
            let mutable result = null
            use writer = new StringWriter()
            html.Save(writer)
            result <- writer.ToString()
            IOHelper.writeToFile processingFileName result
        
        printfn "" 

        //preparing new epub file
        let newFileName = path +/ (sprintf "%s-translated.epub" fileName)
        do zipFile tmpUnCompressedBookPath newFileName
        //clean up
        printfn ""
        printfn "Cleaning up tmp folder."
        do deleteFiles tmpPath "*.*" true true
        printfn "The book has been processed."
