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
    let sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SampleData")
    let sampleFile = Directory.GetFiles(sampleDirectory, "*.epub").[0]
    let getSaveDirPath() = Path.Combine(sampleDirectory, sprintf "%s_%s"  (Path.GetFileNameWithoutExtension(sampleFile)) (Guid.NewGuid().ToString()))
    let toEpubFilePath = toFilePath Epub
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
    let ``Empty translations list should cause to return original text`` () =
        let actualText = applyTranslations sampleTaggedWords [] sampleText
        actualText |> should equal sampleText
    
    [<Fact>]
    let ``Should apply translation to a text``() = 
        let actualText = applyTranslations sampleTaggedWords [ (Word "water", Word "вода", Noun) ] sampleText
        actualText |> should equal "He gave them some water{вода} as they water{вода} the plants daily."
        
    [<Fact>]
    let ``Should translate taking POS into account``() = 
      let actualText = applyTranslations sampleTaggedWords [ (Word "water", Word "вода", Noun); (Word "water", Word "поливать", Verb);] sampleText
      actualText |> should equal "He gave them some water{вода} as they water{поливать} the plants daily."
