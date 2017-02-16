namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open System
    open System.IO
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing
    open BookMate.Processing.EpubProcessor
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse
    open BookMate.Processing.AnalyseHelper
    open BookMate.Processing.DomainModel
    
    let flip f a b = f b a
    let unpackBook' = flip unpackBook
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    
    [<Theory>]
    [<InlineData(null); InlineData(""); InlineData("c:\\q1q1w1"); InlineData("c:\\q1q1w1.gibberish")>]
    let ``Path to nonexistent file is not valid FilePath`` (filePath) = 
        let actual = filePath |> toFilePath
        actual = None |> should be True
    
    [<Fact>]
    let ``Path to existing non-epub file is not valid FilePath``() = 
        let sampleTxtFilePath = Directory.GetFiles(sampleDirectory, "*.txt").[0]
        let actual = sampleTxtFilePath |> toFilePath
        actual = None |> should be True
    
    [<Fact>]
    let ``Unpacked Dir is not yet exisitng dir``() = 
        let actual = sampleDirectory |> toPackDirPath
        actual = InvalidDirPath |> should be True
    
    [<Theory>]
    [<InlineData(null); InlineData(""); InlineData("qwerty")>]
    [<InlineData("c:/gibberish"); InlineData("/gibberish")>]
    [<InlineData("~/gibberish"); InlineData("../gibberish")>]
    let ``Unpacked Dir should be a creatable path to not yet exisitng dir`` (filePath) = 
        let actual = filePath |> toPackDirPath
        actual = InvalidDirPath |> should be True
    
    [<Fact>]
    let ``Unpacked Dir is valid path to not yet exisitng dir``() = 
        let tmpPath = Path.Combine(sampleDirectory, "validdic")
        let actual = tmpPath |> toPackDirPath
        let expected = PackDirPath tmpPath
        actual = expected |> should be True
    
    [<Fact>]
    let ``Unpacking valid epub file should result in path to original file and to directory``() = 
        let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
        let fileName = Path.GetFileNameWithoutExtension(sampleFile)
        let saveDirName = sprintf "%s_%s" fileName (Guid.NewGuid().ToString())
        let saveDirPath = Path.Combine(sampleDirectory, saveDirName)
        
        let expected = 
            ((sampleFile
              |> toFilePath
              |> Option.get), UnpackedDirPath saveDirPath)
            |> UnpackedPath
        
        let actual = 
            unpackBook (sampleFile
                        |> toFilePath
                        |> Option.get) (saveDirPath)
        
        actual.IsSome |> should be True
        expected = actual.Value |> should be True
        Directory.Exists(saveDirPath) |> should be False //ToDo next work item
