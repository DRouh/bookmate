namespace BookMate.Processing.Tests

module EpubIOTest
 = 
    open System
    open System.IO
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse.AnalyseDomain
    open BookMate.Processing.Analyse.AnalyseUtils
    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO
    
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let toEpubFilePath = toFilePath Epub

    [<Theory>]
    [<InlineData(null); InlineData(""); InlineData("c:\\q1q1w1"); InlineData("c:\\q1q1w1.gibberish")>]
    let ``Path to nonexistent file is not valid FilePath`` (filePath) = 
        let actual = filePath |> toEpubFilePath
        None |> should equal actual
    
    [<Fact>]
    let ``Path to existing non-epub file is not valid FilePath``() = 
        let sampleTxtFilePath = Directory.GetFiles(sampleDirectory, "*.txt").[0]
        let actual = sampleTxtFilePath |> toEpubFilePath
        None |> should equal actual
    
    [<Fact>]
    let ``Unpacked Dir is not yet exisitng dir``() = 
        let actual = sampleDirectory |> toPackDirPath
        actual = None |> should be True
    
    [<Theory>]
    [<InlineData(null); InlineData(""); InlineData("qwerty")>]
    [<InlineData("c:/gibberish"); InlineData("/gibberish")>]
    [<InlineData("~/gibberish"); InlineData("../gibberish")>]
    let ``Unpacked Dir should be a creatable path to not yet exisitng dir`` (filePath) = 
        let actual = filePath |> toPackDirPath
        None |> should equal actual
    
    [<Fact>]
    let ``Unpacked Dir is valid path to not yet exisitng dir``() = 
        let tmpPath = Path.Combine(sampleDirectory, "validdic")
        let actual = tmpPath |> toPackDirPath |> Option.get 
        let expected = PackDirPath tmpPath
        expected |> should equal actual
        
    
    [<Fact>]
    let ``Unpacking valid epub file should result in path to original file and to directory``() = 
        let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
        let fileName = Path.GetFileNameWithoutExtension(sampleFile)
        let saveDirName = sprintf "%s_%s" fileName (Guid.NewGuid().ToString())
        let saveDirPath = Path.Combine(sampleDirectory, saveDirName)
        
        let expected = 
            ((sampleFile
              |> toEpubFilePath
              |> Option.get), UnpackedBookPath saveDirPath)
        
        let actual = 
            unpackBook (sampleFile |> toEpubFilePath |> Option.get) 
                       (saveDirPath |> toPackDirPath |> Option.get)
     
        actual.IsSome |> should be True
        expected |> should equal actual.Value

        Directory.Exists(saveDirPath) |> should be True
        Directory.GetFiles(saveDirPath) |> Seq.isEmpty |> should be False
        do Directory.Delete(saveDirPath, true)