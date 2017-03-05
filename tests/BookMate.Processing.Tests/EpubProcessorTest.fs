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

    [<Fact>]
    let ``Read book should contain valid data about all files``() = 
        let saveDirPath = getSaveDirPath()
        let unpackedBook = unpackBook (sampleFile |> toEpubFilePath |> Option.get) (saveDirPath |> toPackDirPath |> Option.get) |> Option.get

        let expectedFiles = Directory.GetFiles(saveDirPath, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
        
        let actualReadBook = readBook unpackedBook |> Option.get
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
        //clean up
        do Directory.Delete(saveDirPath, true)
    
    