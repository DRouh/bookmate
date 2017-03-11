namespace BookMate.Processing

module StanfordTagger = 
    open System
    open BookMate.Processing.StanfordNlp
    open BookMate.Processing.POS
    open BookMate.Core.JsonUtils
    open BookMate.Core.Monads.Maybe
    open BookMate.Integration
    open BookMate.Core
    

    type StanfordTextToTag = {
        Text : string
    }

    type StanfordTaggerResponse = 
        { Uuid : Guid
          Tagged : WordAndTag [] }
    
    and WordAndTag = 
        { Word : string
          Tag : string }
    
    type QueryTokenResponse = 
        { Uuid : string
          Timestamp : DateTimeOffset
          Text : string }
    
    let parseJsonResponse fromJson (jsonText : string) = 
        match fromJson jsonText with
        | Some({ Uuid = u; Tagged = t }) -> Some t
        | _ -> None
    
    let stanfordPosToCommonPos matchStanfordPoS (text : string) = 
        match matchStanfordPoS text with
        | Some t -> t |> stanfordToCommonPos
        | _ -> None
    
    let parseStanfordNlpServiceResponse (response : string) = 
        maybe { 
            let wordWithCommonPos = matchStanfordPoS |> stanfordPosToCommonPos
            let! wordsAndRawTags = parseJsonResponse JsonUtils.fromJson response
            let wts = wordsAndRawTags |> Array.map (fun wt -> (wt.Word, wt.Tag |> wordWithCommonPos))
            return wts
        }
    
    let parseQueryTokenResponse fromJson jsonText = 
        match fromJson jsonText with
        | Some({ Uuid = u; Timestamp = ts; Text = t }) -> Some u
        | _ -> None
    
    let putTextToProcessingComposition (text : string) = 
        async { 
            let endpoint = Configuration.getStanfordTaggerEndpoint()
            let! responseText = RestUtils.PostJsonAsync endpoint text
            let token = parseQueryTokenResponse fromJson responseText
            return token
        }
    
    let queryProcessingTokenComposition (token:string) =
      async {
          let endpoint = sprintf "%s/%s" (Configuration.getStanfordTaggerEndpoint()) token
          let rnd = new System.Random()
          let rec fetchData (awaitNextMs) =
              async { 
                  Async.Sleep(awaitNextMs) |> ignore
                  let! (response,code) = RestUtils.GetJsonAsync endpoint
                  if code <> 404 then return (response |> (parseStanfordNlpServiceResponse))
                  else return! fetchData(rnd.Next(20,70)) //arbitrary figures for delay
                }
          return! fetchData (0)
      }

    let tagText (req: StanfordTextToTag) =
        async {
            let jsonText = req |> JsonUtils.toJson
            let! token = putTextToProcessingComposition jsonText
            if token.IsSome then return! (token |> Option.get |> queryProcessingTokenComposition)
            else return None
        }

    let stanfordTagText (text : string) =
      let textToTag = { Text = text } : StanfordTextToTag
      textToTag |> tagText