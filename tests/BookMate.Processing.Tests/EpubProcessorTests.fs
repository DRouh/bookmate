namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open System
    open System.IO
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO
    open BookMate.Processing.EpubProcessor
    
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
    
    let getSaveDirPath() = 
        let sampleFileName = Path.GetFileNameWithoutExtension(sampleFile)
        Path.Combine(sampleDirectory, sprintf "%s_%s" sampleFileName (Guid.NewGuid().ToString()))
    
    let toAnyHtmlFilePath = toFilePath AnyHtml
    let toEpubFilePath = toFilePath Epub
    
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
    


    
    
    [<Fact>]
    let ``Should read all text from *HTML file`` = 
        let fileText = Path.Combine(sampleDirectory, "epub30-titlepage.xhtml") |> File.ReadAllText
        let html = HtmlUtils.loadHtml fileText
        let actualText = HtmlUtils.getTextFromHtml html
        let expretedText = Path.Combine(sampleDirectory, "epub30-titlepage.txt") |> File.ReadAllLines

        actualText = expretedText |> should be True