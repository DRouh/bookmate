namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse
    open BookMate.Processing.AnalyseHelper
    open BookMate.Processing.EpubProcessor

    [<Fact>]
    let ``Test`` () =
      1 = 1 |> should be True