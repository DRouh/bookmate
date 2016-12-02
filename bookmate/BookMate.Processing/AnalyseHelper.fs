namespace BookMate.Helpers

module AnalyseHelper = 
    open BookMate.Core.Helpers.StringHelper
    open BookMate.Core.Helpers.RegexHelper
    open edu.stanford.nlp.``process``
    open java.io
    open java.util
    open edu.stanford.nlp.tagger.maxent
    open BookMate.Processing.POSHelper
    
    let tagWords (tagger : MaxentTagger) text = 
        let reader = new StringReader(text)
        let taggedWords = MaxentTagger.tokenizeText(reader).toArray()
        
        let mutable a = 0
        let progress = (float32 taggedWords.Length)
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rTagging:%i%%" (int (100.0f * (float32 a / progress)))

        let stat = 
            taggedWords
            |> Array.Parallel.map (fun x -> 
                updateDownloadProgress() 
                tagger.tagSentence(x :?> ArrayList).toArray()
            )
            |> Array.Parallel.map (fun x -> x |> Array.map (string))
            |> Array.concat
            |> Array.Parallel.map (fun x -> x |> (split ([| '/' |])))
            |> Array.Parallel.map (function 
                   | [| word; tag |] -> Some(word, matchStanfordPoS tag)
                   | _ -> None)
            |> Array.where (fun x -> x.IsSome)
            |> Array.Parallel.map (fun x -> x.Value)
            |> Array.where (fun (x, y) -> y.IsSome)
            |> Array.Parallel.map (fun (x, y) -> (x, y.Value))
        stat
        
    let getWordStats text = 
        printfn "Calculating statistics..."
        text
        |> split [| ' ' |]
        |> Array.Parallel.map (sanitize)
        |> Array.filter (isEmpty >> not)
        |> Array.Parallel.map (toLower)
        |> Array.filter (fun x -> 
               match wordFilter x with
               | Some x -> true
               | _ -> false)
        |> Array.groupBy (id)
        |> Array.Parallel.map (fun (key, values) -> (key, values.Length))
        |> Array.sortByDescending (fun (k, v) -> v)

    let getWordStats2 tagger text = 
        printfn "Calculating statistics..."
        let taggedWords = tagWords tagger text
        
        let stat = 
            taggedWords
            |> Array.Parallel.map (fun (x, y) -> (toLower x, stanfordToCommonPos y))
            |> Array.where (fun (x, y) -> y.IsSome)
            |> Array.Parallel.map (fun (x, y) -> (x, y.Value))
            |> Array.filter (fun (x, y) -> 
                   match wordFilter x with
                   | Some x -> true
                   | _ -> false)
            |> Array.groupBy (id)
            |> Array.Parallel.map (fun (key, values) -> (key, values.Length))
            |> Array.Parallel.map (fun ((x, z), c) -> (x, z |> Array.ofList, c))
            |> Array.sortByDescending (fun (k, z, c) -> c)
        stat

    let textToWords omitPunctuation text = 
        let paragraph = text
        let tokenizerFactory = PTBTokenizer.factory (CoreLabelTokenFactory(), "")
        let paragraphReader = new java.io.StringReader(paragraph)
        let rawWords = tokenizerFactory.getTokenizer(paragraphReader).tokenize()
        do paragraphReader.close()
        let words = rawWords.toArray() |> Array.map (string)
        if omitPunctuation then 
            words
            |> Array.map (wordFilter)
            |> Array.where (Option.isSome)
            |> Array.map (Option.get)
        else words
    
    let getPosTagger = 
        let jarRoot = @"d:\downloads\stanford-postagger-2014-08-27"
        let modelsDirectory = jarRoot + @"\models"
        let tagger = new MaxentTagger(modelsDirectory + @"\english-bidirectional-distsim.tagger")
        tagger
