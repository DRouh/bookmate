namespace BookMate.Processing

module Analyse = 
    open POS
    type Tokenizer = bool -> string -> seq<string>
    type Tagger = string -> seq<string*CommonPoS>