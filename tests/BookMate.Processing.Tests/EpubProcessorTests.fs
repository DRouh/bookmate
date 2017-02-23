namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open System
    open System.IO

    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO
    open BookMate.Processing.EpubProcessor

    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
    let toAnyHtmlFilePath = toFilePath AnyHtml
    let toEpubFilePath = toFilePath Epub
    [<Fact>]
    let ``Read book should have valid location data``() = 
        let fileName = Path.GetFileNameWithoutExtension(sampleFile)
        let saveDirName = sprintf "%s_%s" fileName (Guid.NewGuid().ToString())
        let saveDirPath = Path.Combine(sampleDirectory, saveDirName)

        let epubFile = sampleFile |> toEpubFilePath |> Option.get
        let saveDirectory =  saveDirPath |> toPackDirPath |> Option.get
        let unpackedBook = unpackBook (epubFile) (saveDirectory) |> Option.get
        
        let expectedFiles =  Directory.GetFiles(saveDirPath, "*.*html", System.IO.SearchOption.AllDirectories) |> Seq.toList
        
        let actualReadBook = readBook unpackedBook

        let actualFileCount = actualReadBook.Files |> List.length
        let actualFilePaths = actualReadBook.Files |> List.map ((fun f -> f.path) >> (function | FilePath efp -> efp))
        
        actualReadBook.Location = unpackedBook |> should be True
        actualFilePaths = expectedFiles |> should be True
        //clean up
        do Directory.Delete(saveDirPath, true)
        
    