namespace BookMate.NLPServices

module Domain = 
    open System
    
    type ProcessTagEntry = 
        { Uuid : Guid
          Timestamp : DateTimeOffset
          Text : string }
    
    type TaggedEntry = 
        { Uuid : Guid
          Tagged : (string * string) [] }
    
    type TextTagger = string -> (string * string) []
