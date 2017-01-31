namespace BookMate.NLPServices

module Domain = 
    open System
    
    type ProcessTagEntry = 
        { Uuid : Guid
          Timestamp : DateTimeOffset
          Text : string }
    
    type TaggedEntry = 
        { Uuid : Guid
          Tagged : WordAndTag[] }
    and WordAndTag = { Word: string; Tag: string }

    type TextTagger = string -> (string*string)[]