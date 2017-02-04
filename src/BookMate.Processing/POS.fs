namespace BookMate.Processing

module POS = 
    type CommonPoS = 
        | Adjective //good,beautiful, nice, my
        | Adverb // never,there,completely
        | Conjunction //and,but,when
        | Interjection //oh!, good lord!
        | Invariant //????wtf
        | Noun //lion,table, freedom, love
        | Numeral //fifteen
        | Participle
        | Particle
        | Preposition //in,above,to,for,at
        | Pronoun //he, she, it
        | Verb //go 
        | Predicative
    
    let matchCommonPos (value:string) =
        match value.ToLower() with
        | "adjective " -> Some Adjective 
        | "adverb " -> Some Adverb 
        | "conjunction " -> Some Conjunction 
        | "interjection" -> Some Interjection
        | "invariant" -> Some Invariant
        | "noun" -> Some Noun
        | "numeral" -> Some Numeral
        | "participle" -> Some Participle
        | "particle" -> Some Particle
        | "preposition" -> Some Preposition
        | "pronoun" -> Some Pronoun
        | "verb " -> Some Verb 
        | "predicative" -> Some Predicative
        | _ -> None