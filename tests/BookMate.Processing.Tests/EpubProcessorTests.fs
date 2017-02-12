namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open System
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing
    open BookMate.Processing.EpubProcessor
    open BookMate.Processing.POS
    open BookMate.Processing.Analyse
    open BookMate.Processing.AnalyseHelper
    open BookMate.Processing.DomainModel
    
    (*
  1. Read Book
  2. Get book representation
  3. Save book as simple text file
  4. Save book as epub
*)

(*
  1. Grab the epub file
  2. Extract all text from file
  3. Return it as a text

*)

(*
  1. Grab the epub file
  2. Extract all text from file:
     2.1) feature - analyse proper structure of every epub file. May be 
          irrelevant since no all of the files have proper structure.
  3. Analyse file sentence by sentence:
     3.1) feature - gather statistic on the word frequncies.
     3.2)* learn about what NLP can offer to enable better reader better experience
     3.3) take into account user's vocabulary
  4. Refresh translations database with 
     4.1) Decide what words should be fetched from external sources and query them.
  5. Translate book sentence by sentence:
     5.1) Tag every word in sentence and look for best suiting translation available
*)

    [<Fact>]
    let ``Unpacking invalid file path must result in invalid epub book path``() = 
        let data = InvalidFilePath
        let expected = InvalidPath
        let actual = unpackBook data
        expected = actual |> should be True
    
    [<Theory>]
    [<InlineData(null); InlineData(""); InlineData("c:\\q1q1w1"); InlineData("c:\\q1q1w1.gibberish")>]
    let ``Unpacking should result in invalid path if file path does not exist`` (filePath) = 
        let data = filePath
        let expected = InvalidPath
        
        let actual = 
            data
            |> toFilePath
            |> unpackBook
        expected = actual |> should be True
