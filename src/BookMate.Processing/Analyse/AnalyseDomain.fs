namespace BookMate.Processing.Analyse

module AnalyseDomain = 
  open BookMate.Processing.POS
  type Tokenizer = bool -> string -> seq<string>
  type Tagger = string -> seq<string*CommonPoS>