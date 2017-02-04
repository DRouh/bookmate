namespace BookMate.Core

module JsonUtils =
  open Newtonsoft.Json
  open Newtonsoft.Json.Serialization
  let fromJson<'a> (jsonText: string) : 'a option= 
    try 
      let r' = JsonConvert.DeserializeObject(jsonText, typeof<'a>) :?> 'a
      r' |> Some 
    with
    | :? JsonException as e -> None

  let toJson<'a> (v:'a) : string =
      let jsonSerializerSettings = new JsonSerializerSettings()
      jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
      JsonConvert.SerializeObject(v, jsonSerializerSettings)