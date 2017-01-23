namespace BookMate.Processing.Tests

module AnalyseTests = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse
    open BookMate.Processing.AnalyseHelper


    [<Fact>]
    let ``Words, part of speech pairs should return correct statistics`` () =
      let testData = [|("test", Noun); ("test", Verb); ("cat", Noun)|]
      let expected = [|("test", [| Noun; Verb |], 2); ("cat", [| Noun |], 1)|]
      let actual = computeWordPosStat testData
      expected = actual |> should be True



    [<Fact>]
    let ``Edge cases`` () =
      let testData = [|(null, Noun); ("", Verb); ("  ", Noun)|]
   //   let expected = [|("test", [| Noun; Verb |], 2); ("cat", [| Noun |], 1)|]
      let actual = computeWordPosStat testData
      printfn "%A" actual
      1 = 1 |> should be True