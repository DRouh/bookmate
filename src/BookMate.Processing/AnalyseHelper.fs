namespace BookMate.Processing

module AnalyseHelper = 
    open BookMate.Core.Helpers.StringHelper
    open BookMate.Core.Helpers.RegexHelper
    open POS
    open Analyse

    let getPosTagger () : Tagger = fun _ -> raise (System.NotImplementedException "Not ready")
    let getTokenizer () :  Tokenizer = fun _ -> raise (System.NotImplementedException "Not ready")

    let tagWords' (textToTags: Tagger) = textToTags >> Array.ofSeq 

    let tagWords = tagWords' (getPosTagger())

    let computeWordPosStat = 
        Array.where (fun (word:string, pos:CommonPoS) -> isNotNull word)
        Array.Parallel.map (fun (word:string, pos) -> (word.ToLower(), pos))
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