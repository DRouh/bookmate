namespace BookMate.Processing

module AnalyseHelper = 
    open BookMate.Core.Helpers.StringHelper
    open BookMate.Core.Helpers.RegexHelper
    open POS
    open Analyse

    let getPosTagger () : Tagger = raise (System.NotImplementedException "Not ready")
    let getTokenizer () : Tokenizer = raise (System.NotImplementedException "Not ready")

    let tagText (textToTags: Tagger) text =
        let tokens = textToTags text
        tokens

    let tagWords text = 
        let tagText = tagText (getPosTagger())
        let taggedWords = text |> tagText
        taggedWords |> Array.ofSeq
    
    let getWordStats text = 
        let taggedWords = tagWords text
        let stats =
             taggedWords
             |> Array.Parallel.map (fun (word, pos) -> (word.ToLower(), pos))
             |> Array.groupBy (fun (word, _) -> word)
             |> Array.Parallel.map (fun (word, group) -> 
                                                let poss = group |> Array.map (fun (w,p) -> p)
                                                (word, poss, group.Length))
             |> Array.sortByDescending (fun (_, _, c) -> c)
        stats
    
    let textToWords omitPunctuation text = 
        let tokenizeText = getTokenizer() omitPunctuation
        let words = text |> tokenizeText
        words |> Array.ofSeq