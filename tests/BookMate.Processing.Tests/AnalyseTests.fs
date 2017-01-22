namespace BookMate.Processing.Tests

module AnalyseTests = 
    open Xunit
    open FsUnit.Xunit
    open System.Reflection

    [<Fact>]
    let ``Test of Test`` () =
      1 = 1 |> should be True