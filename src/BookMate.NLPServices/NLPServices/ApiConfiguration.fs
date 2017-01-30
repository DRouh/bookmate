namespace BookMate.NLPServices

module ApiConfiguration = 
    open System
    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.RequestErrors
    open SuaveUtils
    open Rest
    
    let rest resourceName resource = 
        let resourcePath = "/" + resourceName
        let resourceGuidPath = new PrintfFormat<string -> string, unit, string, string, string>(resourcePath + "/%s")
        let badRequest = BAD_REQUEST "Resource not found"
        
        let handleResource requestError = 
            function 
            | Some r -> r |> JSON
            | _ -> requestError
        
        let guidConvert str = 
            match Guid.TryParse(str) with
            | (true, guid) -> Some guid
            | (false, _) -> None
        
        let getResourceByGuid str = 
            match guidConvert str with
            | Some v -> v |> (resource.GetByGuid >> handleResource (NOT_FOUND "Resource not found"))
            | None -> None |> handleResource (NOT_FOUND "Resource not found")
        
        let doesExist str = 
            match guidConvert str with
            | Some v -> 
                if resource.Exists v then OK ""
                else NOT_FOUND ""
            | None -> None |> handleResource (NOT_FOUND "Resource not found")
        
        choose [ path resourcePath >=> choose [ POST >=> request (getResourceFromReq
                                                                  >> resource.Create
                                                                  >> JSON) ]
                 GET >=> pathScan resourceGuidPath getResourceByGuid
                 HEAD >=> pathScan resourceGuidPath doesExist ]
