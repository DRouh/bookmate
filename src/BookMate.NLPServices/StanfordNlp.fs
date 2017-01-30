namespace BookMate.NLPServices

module StanfordNlp = 
    open java.io
    open edu.stanford.nlp.tagger.maxent
    open java.util
    
    let loadStanfordTagger (pathToModel) = 
        printfn "Loading Maxent Tagger from %s" pathToModel
        let tagger = MaxentTagger(pathToModel)
        printfn "Tagger successfully loaded."
        tagger
    
    let tagText (tagger : MaxentTagger) text = 
        let tagWords (sentence : obj) = tagger.tagSentence(sentence :?> ArrayList).toArray() |> Array.map (string)
        
        let toWordAndTag (str : string) = 
            match str.Split([| '/' |]) with
            | [| w; t |] -> Some(w, t)
            | _ -> None
        
        let reader = new StringReader(text)
        MaxentTagger.tokenizeText(reader).toArray()
        |> Array.collect tagWords
        |> Array.choose toWordAndTag
