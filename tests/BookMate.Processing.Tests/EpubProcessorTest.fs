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
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
    let getSaveDirPath() = Path.Combine(sampleDirectory, sprintf "%s_%s"  (Path.GetFileNameWithoutExtension(sampleFile)) (Guid.NewGuid().ToString()))
    let toEpubFilePath = toFilePath Epub
    let sampleHtmlDoc = Path.Combine(sampleDirectory, "epub30-titlepage.xhtml")
    let sampleProcessedText= Path.Combine(sampleDirectory, "epub30-translatedexample.txt")


    let getPaths () =
        let saveDirPath = getSaveDirPath()
        let filePath = sampleFile |> toEpubFilePath |> Option.get
        let packDirpath = saveDirPath |> toPackDirPath |> Option.get
        (saveDirPath, filePath, packDirpath)
    let cleanUp saveDirPath =
        do Directory.Delete(saveDirPath, true)
    

    [<Fact>]
    let ``Read book should contain valid data about all files``() = 
        let (saveDirPath, filePath, packDirpath) = getPaths()
        let unpackedBook = unpackBook (filePath) (packDirpath) |> Option.get

        let expectedFiles = Directory.GetFiles(saveDirPath, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
        
        let actualReadBook = readBook unpackedBook |> Option.get

        //clean up
        do cleanUp saveDirPath

        let actualFileCount = actualReadBook.Files |> List.length
        let actualFilePaths = actualReadBook.Files |> List.map ((fun f -> f.Path) >> (fun (AnyHtmlFilePath efp) -> efp))
        
        //validate contents of a read book
        actualReadBook.Location  |> should equal unpackedBook
        actualFilePaths |> should equal expectedFiles     
        actualReadBook.Files
        |> List.map (fun f -> f.Content)
        |> List.reduce (+)
        |> (String.IsNullOrEmpty >> not)
        |> should be True
        actualReadBook.Files |> List.map (fun f -> f.Name) |> should equal (expectedFiles |> List.map (Path.GetFileNameWithoutExtension))
        

    [<Fact>]
    let ``Processed book should have the same number of files`` () = 
      let (saveDirPath, filePath, packDirpath) = getPaths()
      let unpackedBook = unpackBook filePath packDirpath |> Option.get
      let readBook = readBook unpackedBook |> Option.get
      do cleanUp saveDirPath
      let processed = processEpubBook readBook
      processed.Files.Length |> should equal readBook.Files.Length
      
    [<Fact>]
    let ``Processed file should have the original name and path`` () = 
      let rawFile = sampleHtmlDoc |> toAnyHtml |> Option.get |> toFileInEpub
      let { Name = name; Path = path; Content = content } = processFileInEpub rawFile
      name |> should equal rawFile.Name
      path |> should equal rawFile.Path
    
    // [<Fact>]
    // let ``Should apply translations to the file being processed`` () = 
    //   let exampleProcessedContent = File.ReadAllText sampleProcessedText
    //   let rawFile = sampleHtmlDoc |> toAnyHtml |> Option.get |> toFileInEpub
    //   let { Name = _; Path = _; Content = content } = processFileInEpub rawFile
      
    //   content |> should equal exampleProcessedContent