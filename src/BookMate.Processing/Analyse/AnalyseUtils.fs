﻿namespace BookMate.Processing.Analyse

module AnalyseUtils = 
  open BookMate.Core.Helpers.StringHelper
  open BookMate.Core.Helpers.RegexHelper
  open BookMate.Processing.POS
  open BookMate.Processing.Analyse.AnalyseDomain

  let stubTagger : Tagger = fun (_:string) -> Seq.empty<string*CommonPoS>
  let stubTokenizer : Tokenizer = fun (_:bool) (_:string) -> Seq.empty<string>
  
  let getPosTagger () : Tagger = stubTagger
  let getTokenizer () :  Tokenizer = stubTokenizer

  let tagWords' (textToTags: Tagger) = textToTags >> Array.ofSeq 

  let tagWords = tagWords' (getPosTagger())

  let computeWordPosStat = 
      Array.where (fun (word:string, pos:CommonPoS) -> word <> null)
      >> Array.Parallel.map (fun (word:string, pos) -> (word.ToLower(), pos))
      >> Array.groupBy (fun (word, _) -> word)
      >> Array.Parallel.map (fun (word, group) -> 
                                      let poss = group |> Array.map (fun (w,p) -> p)
                                      (word, poss, group.Length))
      >> Array.sortByDescending (fun (_, _, c) -> c)

  let getWordStats = tagWords >> computeWordPosStat
  
  let textToWords omitPunctuation text = 
      let tokenizeText = getTokenizer() omitPunctuation
      let words = text |> tokenizeText
      words |> Array.ofSeq