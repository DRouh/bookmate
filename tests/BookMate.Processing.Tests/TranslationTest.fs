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
        let actualText = applyTranslations sampleTaggedWords [] sampleText
        actualText 
        |> should equal sampleText

    [<Fact>]
    let ``Should translate taking POS into account``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ (Word "water", Word "вода", Noun)
                                                  (Word "water", Word "поливать", Verb) ] sampleText
        actualText 
        |> should equal "He gave them some water{вода} as they water{поливать} the plants daily."
    
    [<Fact>]
    let ``Should translate case-insensitively``() = 
        let actualText = applyTranslations sampleTaggedWords [ (Word "WATER", Word "вода", Noun) ] sampleText
        actualText 
        |> should equal "He gave them some water{вода} as they water{вода} the plants daily."
    
    [<Fact>]
    let ``Should translate using all available translations for POS``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ (Word "water", Word "вода", Noun)
                                                  (Word "water", Word "мочить", Verb)
                                                  (Word "water", Word "поливать", Verb) ] sampleText
        actualText 
        |> should equal "He gave them some water{вода} as they water{мочить,поливать} the plants daily."
    
    [<Fact>]
    let ``Should translate using all available translations if no translation for exact Pos available``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ (Word "water", Word "мочить", Verb)
                                                  (Word "water", Word "поливать", Verb) ] sampleText
        actualText 
        |> should equal "He gave them some water{мочить,поливать} as they water{мочить,поливать} the plants daily."
    
    [<Fact>]
    let ``Should apply correct translations``() = 
        let actualText = 
            applyTranslations sampleTaggedWords [ (Word "He", Word "он", Pronoun)
                                                  (Word "gave", Word "дал", Verb)
                                                  (Word "them", Word "им", Pronoun)
                                                  (Word "some", Word "немного", Noun)
                                                  (Word "water", Word "вода", Noun)
                                                  (Word "as", Word "так как", Preposition)
                                                  (Word "they", Word "они", Pronoun)
                                                  (Word "water", Word "поливать", Verb)
                                                  (Word "the", Word "этот", Pronoun)
                                                  (Word "plants", Word "растения", Noun)
                                                  (Word "daily", Word "ежедневно", Adverb) ] sampleText
        actualText 
        |> should equal 
               "He{он} gave{дал} them{им} some{немного} water{вода} as{так как} they{они} water{поливать} the{этот} plants{растения} daily{ежедневно}."