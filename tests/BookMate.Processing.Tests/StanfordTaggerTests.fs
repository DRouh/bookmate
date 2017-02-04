namespace BookMate.Processing.Tests

module StanfordTaggerTests = 
    open System
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.StanfordTagger
    open BookMate.Processing.POS
    
    [<Theory>]
    [<InlineData("abcd-1234-defg-5678"); InlineData(""); InlineData("   ")>]
    let ``parseQueryTokenResponse should correctly extract token from response`` (token : string) = 
        let fromJson' (_ : 'a) = 
            Some({ Uuid = token
                   Timestamp = DateTimeOffset.Now
                   Text = "" })
        
        let jsonText = String.Empty
        let actual = parseQueryTokenResponse fromJson' jsonText
        actual.IsSome |> should be True
        actual.Value = token |> should be True
    
    [<Fact>]
    let ``123``() = 
        let jsonExample = """
            {
                "uuid": "42401cf8-5d4b-49a1-a761-668a11468318",
                "tagged": [
                    {
                    "word": "Hit",
                    "tag": "VB"
                    },
                    {
                    "word": "the",
                    "tag": "DT"
                    },
                    {
                    "word": "Send",
                    "tag": "VB"
                    },
                    {
                    "word": "button",
                    "tag": "NN"
                    },
                    {
                    "word": "to",
                    "tag": "TO"
                    },
                    {
                    "word": "get",
                    "tag": "VB"
                    },
                    {
                    "word": "a",
                    "tag": "DT"
                    },
                    {
                    "word": "response",
                    "tag": "NN"
                    }
                ]
            }
        """
        
        let expected = 
            Some [| ("Hit", Some [ Verb ])
                    ("the", Some [ Noun; Pronoun; Particle ])
                    ("Send", Some [ Verb ])
                    ("button", Some [ Noun ])
                    ("to", Some [ Preposition ])
                    ("get", Some [ Verb ])
                    ("a", Some [ Noun; Pronoun; Particle ])
                    ("response", Some [ Noun ]) |]
        
        let actual = parseStanfordNlpServiceResponse jsonExample
        actual = expected |> should be True
