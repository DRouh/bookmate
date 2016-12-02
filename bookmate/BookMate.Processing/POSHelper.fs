﻿namespace BookMate.Processing

module POSHelper = 
    //use for referencehttp://www.ling.upenn.edu/courses/Fall_2003/ling001/penn_treebank_pos.html
    //Reflects Stanford definition of Part of Speech
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
    
    let matchStanfordPoS (word : string) : StanfordPoS option = 
        match word with
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
    
    let stanfordPosToString (x : StanfordPoS) = 
        match x with
        | WP_S -> "WP$"
        | PRP_S -> "PRP$"
        | _ -> sprintf "%A" x
    
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
    
    let matchCommonPos (p: string) =
        match p.ToLower() with
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

//Explanation of each tag from the documentation :
//
//CC: conjunction, coordinating
//    & 'n and both but either et for less minus neither nor or plus so
//    therefore times v. versus vs. whether yet
//CD: numeral, cardinal
//    mid-1890 nine-thirty forty-two one-tenth ten million 0.5 one forty-
//    seven 1987 twenty '79 zero two 78-degrees eighty-four IX '60s .025
//    fifteen 271,124 dozen quintillion DM2,000 ...
//DT: determiner
//    all an another any both del each either every half la many much nary
//    neither no some such that the them these this those
//EX: existential there
//    there
//FW: foreign word
//    gemeinschaft hund ich jeux habeas Haementeria Herr K'ang-si vous
//    lutihaw alai je jour objets salutaris fille quibusdam pas trop Monte
//    terram fiche oui corporis ...
//IN: preposition or conjunction, subordinating
//    astride among uppon whether out inside pro despite on by throughout
//    below within for towards near behind atop around if like until below
//    next into if beside ...
//JJ: adjective or numeral, ordinal
//    third ill-mannered pre-war regrettable oiled calamitous first separable
//    ectoplasmic battery-powered participatory fourth still-to-be-named
//    multilingual multi-disciplinary ...
//JJR: adjective, comparative
//    bleaker braver breezier briefer brighter brisker broader bumper busier
//    calmer cheaper choosier cleaner clearer closer colder commoner costlier
//    cozier creamier crunchier cuter ...
//JJS: adjective, superlative
//    calmest cheapest choicest classiest cleanest clearest closest commonest
//    corniest costliest crassest creepiest crudest cutest darkest deadliest
//    dearest deepest densest dinkiest ...
//LS: list item marker
//    A A. B B. C C. D E F First G H I J K One SP-44001 SP-44002 SP-44005
//    SP-44007 Second Third Three Two * a b c d first five four one six three
//    two
//MD: modal auxiliary
//    can cannot could couldn't dare may might must need ought shall should
//    shouldn't will would
//NN: noun, common, singular or mass
//    common-carrier cabbage knuckle-duster Casino afghan shed thermostat
//    investment slide humour falloff slick wind hyena override subhumanity
//    machinist ...
//NNS: noun, common, plural
//    undergraduates scotches bric-a-brac products bodyguards facets coasts
//    divestitures storehouses designs clubs fragrances averages
//    subjectivists apprehensions muses factory-jobs ...
//NNP: noun, proper, singular
//    Motown Venneboerger Czestochwa Ranzer Conchita Trumplane Christos
//    Oceanside Escobar Kreisler Sawyer Cougar Yvette Ervin ODI Darryl CTCA
//    Shannon A.K.C. Meltex Liverpool ...
//NNPS: noun, proper, plural
//    Americans Americas Amharas Amityvilles Amusements Anarcho-Syndicalists
//    Andalusians Andes Andruses Angels Animals Anthony Antilles Antiques
//    Apache Apaches Apocrypha ...
//PDT: pre-determiner
//    all both half many quite such sure this
//POS: genitive marker
//    ' 's
//PRP: pronoun, personal
//    hers herself him himself hisself it itself me myself one oneself ours
//    ourselves ownself self she thee theirs them themselves they thou thy us
//PRP$: pronoun, possessive
//    her his mine my our ours their thy your
//RB: adverb
//    occasionally unabatingly maddeningly adventurously professedly
//    stirringly prominently technologically magisterially predominately
//    swiftly fiscally pitilessly ...
//RBR: adverb, comparative
//    further gloomier grander graver greater grimmer harder harsher
//    healthier heavier higher however larger later leaner lengthier less-
//    perfectly lesser lonelier longer louder lower more ...
//RBS: adverb, superlative
//    best biggest bluntest earliest farthest first furthest hardest
//    heartiest highest largest least less most nearest second tightest worst
//RP: particle
//    aboard about across along apart around aside at away back before behind
//    by crop down ever fast for forth from go high i.e. in into just later
//    low more off on open out over per pie raising start teeth that through
//    under unto up up-pp upon whole with you
//SYM: symbol
//    % & ' '' ''. ) ). * + ,. < = > @ A[fj] U.S U.S.S.R * ** ***
//TO: "to" as preposition or infinitive marker
//    to
//UH: interjection
//    Goodbye Goody Gosh Wow Jeepers Jee-sus Hubba Hey Kee-reist Oops amen
//    huh howdy uh dammit whammo shucks heck anyways whodunnit honey golly
//    man baby diddle hush sonuvabitch ...
//VB: verb, base form
//    ask assemble assess assign assume atone attention avoid bake balkanize
//    bank begin behold believe bend benefit bevel beware bless boil bomb
//    boost brace break bring broil brush build ...
//VBD: verb, past tense
//    dipped pleaded swiped regummed soaked tidied convened halted registered
//    cushioned exacted snubbed strode aimed adopted belied figgered
//    speculated wore appreciated contemplated ...
//VBG: verb, present participle or gerund
//    telegraphing stirring focusing angering judging stalling lactating
//    hankerin' alleging veering capping approaching traveling besieging
//    encrypting interrupting erasing wincing ...
//VBN: verb, past participle
//    multihulled dilapidated aerosolized chaired languished panelized used
//    experimented flourished imitated reunifed factored condensed sheared
//    unsettled primed dubbed desired ...
//VBP: verb, present tense, not 3rd person singular
//    predominate wrap resort sue twist spill cure lengthen brush terminate
//    appear tend stray glisten obtain comprise detest tease attract
//    emphasize mold postpone sever return wag ...
//VBZ: verb, present tense, 3rd person singular
//    bases reconstructs marks mixes displeases seals carps weaves snatches
//    slumps stretches authorizes smolders pictures emerges stockpiles
//    seduces fizzes uses bolsters slaps speaks pleads ...
//WDT: WH-determiner
//    that what whatever which whichever
//WP: WH-pronoun
//    that what whatever whatsoever which who whom whosoever
//WP$: WH-pronoun, possessive
//    whose
//WRB: Wh-adverb
//    how however whence whenever where whereby whereever wherein whereof why
