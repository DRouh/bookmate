namespace BookMate.Processing.Epub

module Processor = 
    open System
    open System.IO

    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO 
    open BookMate.Processing.POS
    open BookMate.Processing.Translation
    open BookMate.Processing.HtmlUtils

    let getFileName = Path.GetFileNameWithoutExtension
    let readAllText = File.ReadAllText
    
    let toFileInEpub (file : FilePath) = 
        let (AnyHtmlFilePath fp) = file
        let fileName = getFileName fp
        let fileContent = readAllText fp
        { Name = fileName
          Path = file
          Content = fileContent }
    
    let toAnyHtml = toFilePath AnyHtml
    
    let readBook (unpackedBook : BookLocation) : OriginalBook = 
        let (EpubFilePath filePath, UnpackedBookPath dirPath) = unpackedBook
        let files = 
            Directory.GetFiles(dirPath, "*.*html", System.IO.SearchOption.AllDirectories)
            |> Seq.toList
            |> List.choose toAnyHtml
            |> List.map toFileInEpub
        (files, unpackedBook)

    let processFileInEpub (rawFile: BookFile) taggedWords translations : ProcessedFileInBook = 
      let htmlDoc = loadHtml rawFile.Content
      let applyTranslations' =  applyTranslations taggedWords translations 
      
      let processingFunction text = applyTranslations' text 3

      let processedHtml = processNodes htmlDoc processingFunction
      let updatedContent = htmlToText processedHtml
      { Name = rawFile.Name; Path = rawFile.Path; Content = updatedContent } 
    
    let processEpubBook (rawBook: OriginalBook) : ProcessedBook =
        let (files, location) = rawBook
        files