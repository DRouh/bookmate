namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open System
    open System.IO
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing
    open BookMate.Processing.POS
    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO
    open BookMate.Processing.Epub.Processor
    
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
    
    let getSaveDirPath() = 
        let sampleFileName = Path.GetFileNameWithoutExtension(sampleFile)
        Path.Combine(sampleDirectory, sprintf "%s_%s" sampleFileName (Guid.NewGuid().ToString()))
    
    let toAnyHtmlFilePath = toFilePath AnyHtml
    let toEpubFilePath = toFilePath Epub
    
    let sampleText = "He gave them some water as they water the plants daily."
    let sampleTaggedWords = 
        [ ("He", [ Pronoun ])
          ("gave", [ Verb ])
          ("them", [ Pronoun ])
          ("some", [ Noun; Pronoun; Particle ])
          ("water", [ Noun ])
          ("as", [ Preposition; Conjunction ])
          ("they", [ Pronoun ])
          ("water", [ Verb ])
          ("the", [ Noun; Pronoun; Particle ])
          ("plants", [ Noun ])
          ("daily", [ Adverb ]) ]   
    [<Fact>]
    let ``Read book should contain valid data about all files``() = 
        let saveDirPath = getSaveDirPath()
        
        let epubFile = 
            sampleFile
            |> toEpubFilePath
            |> Option.get
        
        let saveDirectory = 
            saveDirPath
            |> toPackDirPath
            |> Option.get
        
        let unpackedBook = unpackBook (epubFile) (saveDirectory) |> Option.get
        let expectedFiles = 
            Directory.GetFiles(saveDirPath, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
        let actualReadBook = readBook unpackedBook |> Option.get
        let actualFileCount = actualReadBook.Files |> List.length
        let actualFilePaths = actualReadBook.Files |> List.map ((fun f -> f.Path) >> (function 
                                                                | AnyHtmlFilePath efp -> efp))
        
        let actualFileContents = 
            actualReadBook.Files
            |> List.map (fun f -> f.Content)
            |> List.reduce (+)
        
        let actualFileNames = actualReadBook.Files |> List.map (fun f -> f.Name)
        //validate contents of a read book
        actualReadBook.Location = unpackedBook |> should be True
        actualFilePaths = expectedFiles |> should be True
        actualFileContents
        |> (String.IsNullOrEmpty >> not)
        |> should be True
        actualFileNames = (expectedFiles |> List.map (Path.GetFileNameWithoutExtension)) |> should be True
        //clean up
        do Directory.Delete(saveDirPath, true)
    
    // [<Fact>]
    // let ``Should apply translation to a text``() = 
    //     let expectedText = "He gave them some water{вода} as they water{вода} the plants daily."
    //     let wordsToTranslate = [ "water" ]
    //     let translations = [ (Word "water", Word "вода", Noun) ]
    //     let actualText = applyTranslations sampleTaggedWords translations sampleText
    //     actualText = expectedText |> should be True
    
    [<Fact>]
    let ``Should translate taking POS into account``() = 
      let expectedText = "He gave them some water{вода} as they water{поливать} the plants daily."
      let wordsToTranslate = [ "water" ]
      let translations = [ (Word "water", Word "вода", Noun); (Word "water", Word "поливать", Verb);]
      let actualText = applyTranslations sampleTaggedWords translations sampleText
      actualText = expectedText |> should be True
