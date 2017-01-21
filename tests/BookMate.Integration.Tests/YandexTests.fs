namespace BookMate.Integration.Tests

module YandexTests = 
    open Xunit
    open FsUnit.Xunit
    open System.Reflection
    open System.IO

    open BookMate.Integration.Tests.YandexMocks
    open BookMate.Integration.Yandex
    open BookMate.Integration.YandexHelper
    

    let dictionaryApiResponseJson = lazy(
        Path.Combine(System.IO.Directory.GetCurrentDirectory(), "exampleDictionaryResponse.json")
        |> File.ReadAllText
    )

    [<Fact>]
    let ``Serialized JSON Dictionary response should match expected example`` () =
      let readJson = dictionaryApiResponseJson.Value
      
      let expected = expectedDictionaryResponse
      let actual = deserializeJsonDictionaryResponse readJson
      
      expected.def = actual.def |> should be True

    [<Fact>]
    let ``Read configuration from JSON text should be equal to expected`` () = 
        let jsonFetcherMock = fun (_:string) -> async { return dictionaryApiResponseJson.Value }
        let jsonReaderMock = fun (_:string) -> expectedDictionaryResponse
        let apiEndpoint = "127.0.0.1"
        let apiKey = "123456789"
        let words = "test, test2, test3"

        let expected = 
          [| ("noun", "время"); ("noun", "час"); ("noun", "эпоха"); ("noun", "век"); ("noun", "такт"); ("noun", "жизнь") 
             ("verb", "приурочивать"); ("verb", "рассчитывать"); ("adjective", "временный"); ("adjective", "повременный") |]

        let actual = 
          askYandexDictionary (jsonFetcherMock) (jsonReaderMock) apiEndpoint apiKey words
          |> Async.RunSynchronously
        
        actual = expected |> should be True       