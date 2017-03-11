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
  open BookMate.Processing.HtmlUtils
    
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
    let bookLocation = extractBook (filePath) (extractTargetPath) |> Option.get
    let expectedFiles = 
        Directory.GetFiles(path, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
    let (actualFiles, actualLocation) = readBook bookLocation
    //clean up
    do cleanUp path
    let actualFileCount = actualFiles |> List.length
    let actualFilePaths = actualFiles |> List.map ((fun f -> f.Path) >> (fun (AnyHtmlFilePath efp) -> efp))
    //validate contents of a read book
    actualLocation |> should equal bookLocation
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
    let bookLocation = extractBook filePath extractTargetPath |> Option.get
    let (readFiles, readLocation) = readBook bookLocation
    do cleanUp path
    let processedBook = processEpubBook (readFiles, readLocation)
    processedBook.Length |> should equal readFiles.Length
  
  [<Fact>]
  let ``Processed file should have the original name and path``() = 
    let rawBookFile = 
        sampleHtmlDoc
        |> toAnyHtml
        |> Option.get
        |> toFileInEpub
    
    let { Name = name; Path = path; Content = content } = processFileInEpub rawBookFile [] []
    name |> should equal rawBookFile.Name
    path |> should equal rawBookFile.Path
  
  [<Fact>]
  let ``Should apply translations to the file being processed``() = 
    let expectedHtml = File.ReadAllText sampleProcessedText |> loadHtml

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
    
    let rawBookFile = 
        sampleHtmlDoc
        |> toAnyHtml
        |> Option.get
        |> toFileInEpub
    
    let { Name = _; Path = _; Content = content } = processFileInEpub rawBookFile exampleTaggedWords exampleTranslations
    let actualHtml = loadHtml content
    actualHtml.DocumentNode.OuterHtml |> should equal expectedHtml.DocumentNode.OuterHtml

  [<Fact>]
  let ``Should analyse tag text correctly``() = 
    let taggedWords =  sampleTaggedWords |> File.ReadAllText |> parseStanfordNlpServiceResponse
    let wordsToTranslate = List.empty<Word>
    
    let bookFile = 
        sampleHtmlDoc
        |> toAnyHtml
        |> Option.get
        |> toFileInEpub
    
    let expected = 
        { File = bookFile
          AnalysisData = 
              { WordsToTranslate = wordsToTranslate
                TaggedText = 
                    [ ("The", [ Noun; Pronoun; Particle ]);("documents", [ Noun ]);("canonically", [ Adverb ])
                      ("located", [ Adjective ]);("at", [ Preposition; Conjunction ]);("reproduced", [ Verb; Participle ])
                      ("in", [ Preposition; Conjunction ]);("EPUB", [ Noun ]);("3", [ Numeral ]);("format", [ Noun ])
                      ("All", [ Noun; Pronoun; Particle ]);("rights", [ Noun ]);("reserved", [ Verb; Participle ])
                      ("This", [ Noun; Pronoun; Particle ]);("work", [ Noun ]);("is", [ Verb ]);("protected", [ Verb; Participle ])
                      ("under", [ Preposition; Conjunction ]);("17", [ Numeral ]);("of", [ Preposition; Conjunction ])
                      ("the", [ Noun; Pronoun; Particle ]);("and", [ Conjunction ]);("dissemination", [ Noun ])
                      ("of", [ Preposition; Conjunction ]);("this", [ Noun; Pronoun; Particle ]);("work", [ Noun ])
                      ("with", [ Preposition; Conjunction ]);("changes", [ Noun ]);("is", [ Verb ])
                      ("prohibited", [ Verb; Participle ]);("except", [ Preposition; Conjunction ])
                      ("with", [ Preposition; Conjunction ]);("the", [ Noun; Pronoun; Particle ])
                      ("written", [ Verb; Participle ]);("permission", [ Noun ]);("of", [ Preposition; Conjunction ])
                      ("the", [ Noun; Pronoun; Particle ]);("EPUB", [ Noun ]);("is", [ Verb ]);("a", [ Noun; Pronoun; Particle ])
                      ("registered", [ Verb; Participle ]);("trademark", [ Noun ]);("of", [ Preposition; Conjunction ])
                      ("the", [ Noun; Pronoun; Particle ]) ] } }

    let tagTextMock text = async { return taggedWords }
    let determineWordsToTranslateMock taggedWords = async { return wordsToTranslate }

    let analysedText = (analyseText bookFile tagTextMock determineWordsToTranslateMock) |> Async.RunSynchronously
    analysedText |> should equal expected