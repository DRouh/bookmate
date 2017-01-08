namespace BookMate.Helpers

module AnalyseHelper = 
    open BookMate.Core.Helpers.StringHelper
    open BookMate.Core.Helpers.RegexHelper
    //open edu.stanford.nlp.``process``
    //open java.io
    //open java.util
    //open edu.stanford.nlp.tagger.maxent
    open BookMate.Processing.POSHelper
    
//    let tagWords (tagger : MaxentTagger) text = 
    let tagWords (tagger) text = 
        [|("", CC)|]
        // let reader = new StringReader(text)
        // let taggedWords = MaxentTagger.tokenizeText(reader).toArray()
        // let mutable a = 0
        // let progress = (float32 taggedWords.Length)
        
        // let updateDownloadProgress() = 
        //     a <- a + 1
        //     printf "\rTagging:%i%%" (int (100.0f * (float32 a / progress)))
        
        // let stat = 
        //     taggedWords
        //     |> Array.Parallel.collect (fun x -> 
        //            updateDownloadProgress()
        //            let taggedWords = tagger.tagSentence(x :?> ArrayList).toArray()
        //            let taggedStrings = taggedWords |> Array.map (string)
        //            taggedStrings)
        //     |> Array.Parallel.choose (fun x -> 
        //            let wordTag = x |> (split ([| '/' |]))
        //            match wordTag with
        //            | [| word; tag |] -> 
        //                match matchStanfordPoS tag with
        //                | Some t -> Some(word, t)
        //                | _ -> None
        //            | _ -> None)        

        // printf "\rTagging:%i%%" 100
        // stat
    
    let getWordStats tagger text = 
        [|("", [|Adjective|],1)|]
        // printfn "Calculating statistics..."
        // let taggedWords = tagWords tagger text
        
        // let stat = 
        //     taggedWords
        //     |> Array.Parallel.choose (fun (word, sPos) -> 
        //            match stanfordToCommonPos sPos with
        //            | Some cPos -> 
        //                let l = toLower <| word
        //                match wordFilter l with
        //                | Some w -> Some(w, cPos)
        //                | _ -> None
        //            | _ -> None)
        //     |> Array.groupBy (id)
        //     |> Array.Parallel.map (fun ((w, ts), _) -> w, ts |> Array.ofList, ts.Length)
        //     |> Array.sortByDescending (fun (_, _, c) -> c)
        // stat
    
    let textToWords omitPunctuation text = 
        [|""|]
        // let paragraph = text
        // let tokenizerFactory = PTBTokenizer.factory (CoreLabelTokenFactory(), "")
        // let paragraphReader = new java.io.StringReader(paragraph)
        // let rawWords = tokenizerFactory.getTokenizer(paragraphReader).tokenize()
        // do paragraphReader.close()
        
        // let words = rawWords.toArray() |> Array.map (string)
        // if omitPunctuation then words |> Array.Parallel.choose (wordFilter)
        // else words
    
    let getPosTagger = ""
        // let jarRoot = BookMate.Core.Configuration.getStanfordModelFolder //@"d:\downloads\stanford-postagger-2014-08-27"
        // let modelsDirectory = jarRoot + @"\models"
        // let tagger = new MaxentTagger(modelsDirectory + @"\english-bidirectional-distsim.tagger")
        // tagger