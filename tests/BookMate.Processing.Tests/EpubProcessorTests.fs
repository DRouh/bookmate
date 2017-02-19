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

    [<Fact>]
    let ``Read book should have valid location data``() = 
        let fileName = Path.GetFileNameWithoutExtension(sampleFile)
        let saveDirName = sprintf "%s_%s" fileName (Guid.NewGuid().ToString())
        let saveDirPath = Path.Combine(sampleDirectory, saveDirName)

        let unpackedBook = unpackBook (sampleFile |> toFilePath |> Option.get) (saveDirPath |> toPackDirPath |> Option.get) |> Option.get
        
        do Directory.Delete(saveDirPath, true)
        
        let actual = readBook unpackedBook

        actual.Location = unpackedBook |> should be True

    