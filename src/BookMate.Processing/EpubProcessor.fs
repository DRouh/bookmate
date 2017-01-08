namespace BookMate.Processing

module EpubProcessor = 
    open BookMate.Core.Helpers.IOHelper
    open BookMate.Helpers
    open HtmlAgilityPack
    open System.IO
    open BookMate.Helpers.AnalyseHelper
    open POSHelper
    open BookMate.Core.Helpers

    let private prepareStatForExport = Array.Parallel.map (fun (w, p, c) -> sprintf "%s %A %i %s" w p c System.Environment.NewLine) >> String.concat (@"")
    let private prepareNewTranslationsForExport = Array.Parallel.map (fun (w, p,s) -> sprintf "%s, %s, %s %s" w p s System.Environment.NewLine) >> String.concat (@"")

    let private loadBook = 
        function
        | _ -> ""
        // function
        // | "" | null -> failwith "Can't use empty file path to load the book"
        // | path -> 
        //     match File.Exists(path) with
        //     | true -> 
        //         let fileName = getFileNameWithoutExtension path
        //         let book = EpubReader.Read(path)
        //         let text = book.ToPlainText()
        //         printfn "Loaded %s" fileName
        //         text
        //     | _ -> failwith "File does not exist."
    
    let tryFind (lookup:(string*('a*'b)[])[]) word =
        lookup |> Array.where (fun (e, _) -> e = word) |> Seq.tryHead

    let processText posTagger dict (text:string) = 
        let findInDic = tryFind dict
        match text.Trim() with 
        | null | "" | "\r" | "\r\n" | "\n" -> text
        | _ ->
            let paragraph = text
            let wordsToCheck = textToWords <| true <| paragraph           
            let translations = 
                wordsToCheck
                |> Array.Parallel.choose (fun w -> 
                       match findInDic w with
                       | Some t -> Some(t)
                       | _ -> None)
                |> Map.ofArray

            let taggedWords = tagWords posTagger paragraph//todo refactor this to return common pos 
            let mutable processedText = text
            for (word, pos) in taggedWords do
                match stanfordToCommonPos pos with
                | None -> ()
                | Some commonPos ->
                    let wordTranslations =  translations.TryFind word
                    match wordTranslations with
                    | Some v ->
                        if Seq.isEmpty v then 
                            ()
                        else
                            let translationsForExactPos = v |> Array.where (fun (p, _) -> commonPos |> List.contains p)
                            let choice = if Seq.isEmpty translationsForExactPos then v else translationsForExactPos
                            let translationsToUse = choice |> Array.map (snd) |> Seq.truncate 3 |> Array.ofSeq |> Array.reduce (StringHelper.stringify)
                            let pattern = @"\b" + word + @"\b"
                            let replace = word + "{" + translationsToUse + "}"
                            processedText <- RegexHelper.regexReplace text pattern replace
                    | _ -> ()
            processedText 

    let convertPosForArray = 
        Array.Parallel.choose (fun (e, pS, r) -> 
            match matchCommonPos pS with
            | Some p -> Some(e, p, r)
            | _ -> None)

    let formDictionaryForBook (userDefinedWords:string[]) wordStat queriedTranslations (dbDictionary:(string*CommonPoS*string)[]) =
        let numOfTranslationsToUse = 3
        //todo determine complexity of word or its spread
        let wordsToTranslate = 
            wordStat 
            |> Array.Parallel.map (fun (e:string, posString, r) -> (e.ToLower(), posString, r))
            |> Array.where (fun (e, p, c) -> c < 20 || (userDefinedWords |> Seq.contains e))

        let trans = 
            queriedTranslations 
            |> convertPosForArray
            
        let lookupWord (lookupDic:(string*CommonPoS*string)[]) (engWord:string) (pos:CommonPoS[])  = 
            let existInPos (x:CommonPoS) = pos |> Array.exists (fun elem -> elem = x)
            let all = lookupDic |> Array.where (fun (e, p, t) -> e = engWord)
            let exactPos = all |> Array.where (fun (e, p, t) -> existInPos p)
            let res = if exactPos <> Array.empty then exactPos else all
            res |> Seq.truncate numOfTranslationsToUse |> Array.ofSeq |> Array.map (fun (e,p,s) -> (p,s))

        let look = lookupWord <| (trans |> Array.append (dbDictionary) |> Array.distinct)

        let translatedWords = 
            wordsToTranslate 
            |> Array.Parallel.map (fun (eng, pos, _) -> (eng, pos, look <| eng <| pos))
            |> Array.Parallel.map (fun (e, _, trs) -> (e, trs))
        translatedWords

    let loadUserDefinedWords = 
        let fileName = BookMate.Core.Configuration.getUserDefinedWordsFilePath
        if File.Exists fileName then
            let words = File.ReadAllLines fileName
            words
        else 
            Array.empty<string>

     //Array.empty<string>
    let processBook bookPath = 
        let fileName = getFileNameWithoutExtension bookPath
        let path = getDirectoryName bookPath
        printfn "Processing %s" fileName
        
        //load book
        let text = loadBook bookPath
        
        //load pos tagger
        let posTagger = getPosTagger
        
        //load dictionary from database
        let dbDictionary = [|("", Adjective, "")|]  //BookMate.Integration.DBHelper.loadFromDB |> convertPosForArray

        //get book statistics
        let wordStat = AnalyseHelper.getWordStats posTagger text

        //save book statistics
        let statToExport' = wordStat |> prepareStatForExport
        do IOHelper.writeToFile (path +/ (sprintf "%s-pos-stat.txt" fileName)) statToExport'

        //detetrmine words to query translation for
        let userDefinedWords = loadUserDefinedWords //load words that user explicitly asked to translate
        let translationsToQuery = 
            wordStat 
            |> Array.Parallel.map (fun (x, _, _) -> x) 
            |> Array.where (fun x -> x.Length >= 3)
            |> Array.append (userDefinedWords)
            |> Array.distinct
            |> Array.except (dbDictionary |> Array.map (fun (x, _, _) -> x) |> Array.distinct) 

        let queriedTranslations = TranslationDownloadHelper.downloadDictionaryTranslations <| translationsToQuery 
        //save queried translations
        let preparedForSavingTranslations = prepareNewTranslationsForExport <| queriedTranslations
        do IOHelper.writeToFile (path +/ (sprintf "%s-translations.txt" fileName)) preparedForSavingTranslations
        
        //form dictionary for the book

        let dictionaryForBook = formDictionaryForBook userDefinedWords wordStat queriedTranslations dbDictionary
        
        //IO operations - creatint tmp folder, unzipping file,...
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
        
        let contentFiles = Directory.GetFiles(workingPath, "*.*html", System.IO.SearchOption.AllDirectories)
        printfn "Discovered file structure:"
        for f in contentFiles do
            do printfn "-%s" <| getFileName f
        
        let translateProcessText = processText <| posTagger <| dictionaryForBook
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
            
            use reader = System.IO.File.OpenText(processingFileName)
            do html.Load(reader)
            let textNodes = html.DocumentNode.SelectNodes("//p//text()")
            
            if not (isNull textNodes) then 
                let mutable inx = 1
                for p in textNodes do
                    do updateProgress filesToProcessCount textNodes.Count i inx
                    if p :? HtmlTextNode then 
                        let node = p :?> HtmlTextNode
                        node.Text <- translateProcessText <| node.Text
                    inx <- inx + 1
            else 
                do updateProgress filesToProcessCount 1 i 1
                let progressPercent = (int (100.0f * (float32 i + 1.0f) / (float32 contentFiles.Length)))
                printf "\rProcessing:%i%%" progressPercent
            let mutable result = null
            use writer = new System.IO.StringWriter()
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


//Bootstrap project where all DI will be wired
//Extract book analysis from main routine and create it as separate service
//Separate database access functionality from implementation and current database