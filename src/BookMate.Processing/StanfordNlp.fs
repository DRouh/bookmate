namespace BookMate.Processing

module StanfordNlp = 
      open BookMate.Processing.POS

      type StanfordPoS = 
        | CC //Coordinating conjunction
        | CD //Cardinal number
        | DT //Determiner
        | EX //Existential there
        | FW //Foreign word
        | IN //Preposition or subordinating conjunction
        | JJ //Adjective
        | JJR //Adjective, comparative
        | JJS //Adjective, superlative
        | LS //List item marker
        | MD //Modal
        | NN //Noun, singular or mass
        | NNS //Noun, plural
        | NNP //Proper noun, singular
        | NNPS //Proper noun, plural
        | PDT //Predeterminer
        | POS //Possessive ending
        | PRP //Personal pronoun
        | PRP_S //Possessive pronoun
        | RB //Adverb
        | RBR //Adverb, comparative
        | RBS //Adverb, superlative
        | RP //Particle
        | SYM //Symbol
        | TO //to
        | UH //Interjection
        | VB //Verb, base form
        | VBD //Verb, past tense
        | VBG //Verb, gerund or present participle
        | VBN //Verb, past participle
        | VBP //Verb, non-3rd person singular present
        | VBZ //Verb, 3rd person singular present
        | WDT //Wh-determiner
        | WP //Wh-pronoun
        | WP_S //Possessive wh-pronoun
        | WRB //Wh-adverb

      let matchStanfordPoS = 
        function 
        | "CC" -> Some CC
        | "CD" -> Some CD
        | "DT" -> Some DT
        | "EX" -> Some EX
        | "FW" -> Some FW
        | "IN" -> Some IN
        | "JJ" -> Some JJ
        | "JJR" -> Some JJR
        | "JJS" -> Some JJS
        | "LS" -> Some LS
        | "MD" -> Some MD
        | "NN" -> Some NN
        | "NNS" -> Some NNS
        | "NNP" -> Some NNP
        | "NNPS" -> Some NNPS
        | "PDT" -> Some PDT
        | "POS" -> Some POS
        | "PRP" -> Some PRP
        | "PRP$" -> Some PRP_S
        | "RB" -> Some RB
        | "RBR" -> Some RBR
        | "RBS" -> Some RBS
        | "RP" -> Some RP
        | "SYM" -> Some SYM
        | "TO" -> Some TO
        | "UH" -> Some UH
        | "VB" -> Some VB
        | "VBD" -> Some VBD
        | "VBG" -> Some VBG
        | "VBN" -> Some VBN
        | "VBP" -> Some VBP
        | "VBZ" -> Some VBZ
        | "WDT" -> Some WDT
        | "WP" -> Some WP
        | "WP$" -> Some WP_S
        | "WRB" -> Some WRB
        | _ -> None

      let stanfordToCommonPos = 
        function 
        | POS | FW | SYM | NNP | NNPS -> None
        | CC -> Some [ Conjunction ]
        | CD | LS -> Some [ Numeral ]
        | EX | WRB -> Some [ Adverb ]
        | IN -> Some([ Preposition; Conjunction ])
        | JJ | JJR | JJS -> Some [ Adjective ]
        | MD -> Some [ Predicative; Noun; Verb ]
        | NN | NNS -> Some [ Noun ]
        | DT | PDT | WDT -> Some [ Noun; Pronoun; Particle ]
        | PRP | PRP_S -> Some [ Pronoun ]
        | RB | RBR | RBS -> Some [ Adverb ]
        | RP -> Some [ Particle ]
        | TO -> Some [ Preposition ]
        | UH -> Some [ Interjection ]
        | VB | VBD | VBP | VBZ -> Some [ Verb ]
        | VBG | VBN -> Some [ Verb; Participle ]
        | WP | WP_S -> Some [ Pronoun ]

      
      let stanfordPosToString value = 

          function
          | WP_S -> "WP$"
          | PRP_S -> "PRP$"
          | _ -> sprintf "%A" value