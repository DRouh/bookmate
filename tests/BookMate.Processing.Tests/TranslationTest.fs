namespace BookMate.Processing.Tests

module TranslationTest = 
    open System
    open System.IO
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.POS
    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO
    open BookMate.Processing.Translation
    
    let sampleText = "He gave them some water as they water the plants daily."
    
    let sampleTaggedWords = 
        [ ("He", [ Pronoun ])
          ("gave", [ Verb ])
          ("them", [ Pronoun ])
          ("some", [ Noun; Pronoun; Particle ])
          ("water", [ Noun ])
          ("as", [ Preposition; Conjunction ])
          ("they", [ Pronoun ])
          ("water", [ Verb ])
          ("the", [ Noun; Pronoun; Particle ])
          ("plants", [ Noun ])
          ("daily", [ Adverb ]) ]
    
    [<Fact>]
    let ``Empty translations list should cause to return original text``() = 
        let actualText = applyTranslations sampleTaggedWords [] sampleText 1
        actualText 
        |> should equal sampleText

    [<Fact>]
    let ``Should translate taking POS into account``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ ("water", "вода", Noun)
                                                  ("water", "поливать", Verb) ] sampleText 1
        actualText 
        |> should equal "He gave them some water{вода} as they water{поливать} the plants daily."
    
    [<Fact>]
    let ``Should translate case-insensitively``() = 
        let actualText = applyTranslations sampleTaggedWords [ ("WATER", "вода", Noun) ] sampleText 1
        actualText 
        |> should equal "He gave them some water{вода} as they water{вода} the plants daily."
    
    [<Fact>]
    let ``Should translate using all available translations for POS``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ ("water", "вода", Noun)
                                                  ("water", "мочить", Verb)
                                                  ("water", "поливать", Verb) ] sampleText 2
        actualText 
        |> should equal "He gave them some water{вода} as they water{мочить,поливать} the plants daily."
    
    [<Fact>]
    let ``Should translate using all available translations if no translation for exact Pos available``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ ("water", "мочить", Verb)
                                                  ("water", "поливать", Verb) ] sampleText 2
        actualText 
        |> should equal "He gave them some water{мочить,поливать} as they water{мочить,поливать} the plants daily."
    
    [<Fact>]
    let ``Should apply correct translations``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ ("He", "он", Pronoun)
                                                  ("gave", "дал", Verb)
                                                  ("them", "им", Pronoun)
                                                  ("some", "немного", Noun)
                                                  ("water", "вода", Noun)
                                                  ("as", "так как", Preposition)
                                                  ("they", "они", Pronoun)
                                                  ("water", "поливать", Verb)
                                                  ("the", "этот", Pronoun)
                                                  ("plants", "растения", Noun)
                                                  ("daily", "ежедневно", Adverb) ] sampleText 1
        actualText 
        |> should equal 
               "He{он} gave{дал} them{им} some{немного} water{вода} as{так как} they{они} water{поливать} the{этот} plants{растения} daily{ежедневно}."

    [<Fact>]
    let ``Should not exceed max number of translations for a word``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ ("water", "мочить", Verb)
                                                  ("water", "разливать", Verb)
                                                  ("water", "поливать", Verb) ] sampleText 2
        actualText 
        |> should equal "He gave them some water{мочить,разливать} as they water{мочить,разливать} the plants daily."