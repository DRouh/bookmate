namespace BookMate.Processing.Tests

module EpubProcessorTest = 
  open System
  open System.IO
  open Xunit
  open FsUnit.Xunit
  open BookMate.Processing.POS
  open BookMate.Processing.Epub.Domain
  open BookMate.Processing.Epub.IO
  open BookMate.Processing.Epub.Processor
  open BookMate.Processing.StanfordTagger
    
  let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
  let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
  let getExtractTargetPath() = 
      Path.Combine
          (sampleDirectory, sprintf "%s_%s" (Path.GetFileNameWithoutExtension(sampleFile)) (Guid.NewGuid().ToString()))
  let toEpubFilePath = toFilePath Epub
  let sampleHtmlDoc = Path.Combine(sampleDirectory, "epub30-titlepage.xhtml")
  let sampleProcessedText = Path.Combine(sampleDirectory, "epub30-translatedexample.txt")
  let sampleTaggedWords = Path.Combine(sampleDirectory, "epub30-tagged.json")
  
  let getPaths() = 
    let path = getExtractTargetPath()
    
    let filePath = 
        sampleFile
        |> toEpubFilePath
        |> Option.get
    
    let extractTargetPath = 
        path
        |> toExtractTargetPath
        |> Option.get
    
    (path, filePath, extractTargetPath)
  
  let cleanUp extractTargetPath = do Directory.Delete(extractTargetPath, true)
    
  [<Fact>]
  let ``Read book should contain valid data about all files``() = 
    let (path, filePath, extractTargetPath) = getPaths()
    let unpackedBook = extractBook (filePath) (extractTargetPath) |> Option.get
    let expectedFiles = 
        Directory.GetFiles(path, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
    let (actualFiles, actualLocation) = readBook unpackedBook
    //clean up
    do cleanUp path
    let actualFileCount = actualFiles |> List.length
    let actualFilePaths = actualFiles |> List.map ((fun f -> f.Path) >> (fun (AnyHtmlFilePath efp) -> efp))
    //validate contents of a read book
    actualLocation |> should equal unpackedBook
    actualFilePaths |> should equal expectedFiles
    actualFiles
    |> List.map (fun f -> f.Content)
    |> List.reduce (+)
    |> (String.IsNullOrEmpty >> not)
    |> should be True
    actualFiles
    |> List.map (fun f -> f.Name)
    |> should equal (expectedFiles |> List.map (Path.GetFileNameWithoutExtension))
  
  [<Fact>]
  let ``Processed book should have the same number of files``() = 
    let (path, filePath, extractTargetPath) = getPaths()
    let unpackedBook = extractBook filePath extractTargetPath |> Option.get
    let (readFiles, readLocation) = readBook unpackedBook
    do cleanUp path
    let processedFiles = processEpubBook (readFiles, readLocation)
    processedFiles.Length |> should equal readFiles.Length
  
  [<Fact>]
  let ``Processed file should have the original name and path``() = 
    let rawFile = 
        sampleHtmlDoc
        |> toAnyHtml
        |> Option.get
        |> toFileInEpub
    
    let { Name = name; Path = path; Content = content } = processFileInEpub rawFile [] []
    name |> should equal rawFile.Name
    path |> should equal rawFile.Path
  
  [<Fact>]
  let ``Should apply translations to the file being processed``() = 
    let exampleProcessedContent = File.ReadAllText sampleProcessedText
    
    let exampleTaggedWords = 
        (parseStanfordNlpServiceResponse (File.ReadAllText sampleTaggedWords))
        |> Option.get
        |> Array.where (fun (x, y) -> y.IsSome)
        |> Array.map (fun (x, y) -> (x, y.Value))
        |> List.ofArray
    
    let exampleTranslations = 
        [ ("documents", "документы", Noun)
          ("reproduced", "воспроизведен", Verb)
          ("work", "работа", Noun) ]
    
    let rawFile = 
        sampleHtmlDoc
        |> toAnyHtml
        |> Option.get
        |> toFileInEpub
    
    let { Name = _; Path = _; Content = content } = processFileInEpub rawFile exampleTaggedWords exampleTranslations
    content |> should equal exampleProcessedContent