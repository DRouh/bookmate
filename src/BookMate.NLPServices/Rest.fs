namespace BookMate.NLPServices

module Rest = 
    open System
    
    type RestResource<'a, 'b, 'c> = 
        { Create : 'a -> 'b
          GetByGuid : Guid -> 'c option
          Exists : Guid -> bool }
