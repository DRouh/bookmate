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
    
    let readBook (bookLocation : BookLocation) : OriginalBook = 
        let (EpubFilePath filePath, UnpackedBookPath dirPath) = bookLocation
        let files = 
            Directory.GetFiles(dirPath, "*.*html", System.IO.SearchOption.AllDirectories)
            |> Seq.toList
            |> List.choose toAnyHtml
            |> List.map toFileInEpub
        (files, bookLocation)

    let analyseText (userPrefs : UserPrefs) (content : string) : Word list = []

    let analyseBook  (userPrefs : UserPrefs) (originalBook : OriginalBook) : AnalysedBook = 
      let (bookFiles, location) = originalBook
      bookFiles 
      |> List.map (fun bf ->
          let text = bf.Content |> loadHtml |> getTextFromHtml |> String.concat " "
          let wordsToTranslate = analyseText userPrefs text
          { File = { Name = bf.Name; Path = bf.Path; Content = bf.Content }; AnalysisData = { WordsToTranslate = wordsToTranslate} })

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