namespace BookMate.Processing.Tests

module AnalyseTest = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse.AnalyseDomain
    open BookMate.Processing.Analyse.AnalyseUtils

    [<Fact>]
    let ``Words, part of speech pairs should return correct statistics`` () =
      let actual = computeWordPosStat [|("test", Noun); ("test", Verb); ("cat", Noun)|]
      actual  |> should equal [|("test", [| Noun; Verb |], 2); ("cat", [| Noun |], 1)|]

    [<Fact>]
    let ``Edge cases should be correctly handled in statistics`` () =
      let actual = computeWordPosStat [|(null, Noun);("", Verb); ("  ", Noun)|]
      actual |> should equal [|("", [| Verb |], 1); ("  ", [| Noun |], 1)|]
      
    [<Fact>]
    let ``Empty array of input pairs should result in empty statistics`` () =
      let actual = computeWordPosStat [||]
      actual |> should equal [||]