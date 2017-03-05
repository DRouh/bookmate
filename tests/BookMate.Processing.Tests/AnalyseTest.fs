namespace BookMate.Processing.Tests

module AnalyseTest = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse.AnalyseDomain
    open BookMate.Processing.Analyse.AnalyseUtils

    [<Fact>]
    let ``Words, part of speech pairs should return correct statistics`` () =
      let testData = [|("test", Noun); ("test", Verb); ("cat", Noun)|]
      let expected = [|("test", [| Noun; Verb |], 2); ("cat", [| Noun |], 1)|]
      let actual = computeWordPosStat testData
      expected = actual |> should be True

    [<Fact>]
    let ``Edge cases should be correctly handled in statistics`` () =
      let testData = [|(null, Noun);("", Verb); ("  ", Noun)|]
      let expected = [|("", [| Verb |], 1); ("  ", [| Noun |], 1)|]
      let actual = computeWordPosStat testData
      expected = actual |> should be True

    [<Fact>]
    let ``Empty array of input pairs should result in empty statistics`` () =
      let testData :(string*CommonPoS)[]= [||]
      let expected :(string*CommonPoS[]*int)[]= [||]
      let actual = computeWordPosStat testData
      expected = actual |> should be True