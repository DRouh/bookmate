namespace BookMate.Integration

module YandexHelper = 
    open BookMate.Core.Helpers.StringHelper
    open Newtonsoft.Json
    open RestHelper
    open Yandex
    
    let private translateApiKey =  BookMate.Core.Configuration.getYandexTranslateApiKey()
    let private translateApiEndPoint = BookMate.Core.Configuration.getYandexTranslateApiEndPoint()
    let private dictionaryApiKey = BookMate.Core.Configuration.getYandexDictionaryApiKey() 
    let private dictionaryApiEndPoint = BookMate.Core.Configuration.getYandexDictionaryApiEndPoint()
    
    let deserializeJsonDictionaryResponse = JsonConvert.DeserializeObject<YandexDictionaryResponse>
    let deserializeJsonTranslateResponse = JsonConvert.DeserializeObject<YandexTranslateResponse>

    let askYandexTranslate (getJsonFetcher: string -> Async<string>) 
                           (jsonReader: string -> YandexTranslateResponse) 
                           apiEndpoint apiKey word = 
        async {
            let url = ComposeUrl apiEndpoint [ ("key", apiKey); ("text", word); ("lang", "en-ru"); ("format", "plain")] 
            let! response = getJsonFetcher url

            let translateResponse = jsonReader(response) 
            let translations = translateResponse.text |> Array.collect ((unstringify) >> (Array.map (fun x -> x.Trim())))
            return translations
        }
        
    let askYandexDictionary (getJsonFetcher: string -> Async<string>) 
                            (jsonReader: string -> YandexDictionaryResponse) 
                            (apiEndpoint) (apiKey) (words) : Async<(string*string)[]> = 
        async {
            let url = ComposeUrl apiEndpoint [ ("key", apiKey); ("text", words); ("lang", "en-ru"); ("format", "plain")] 
            let! response = getJsonFetcher url

            let dictionaryResponse = jsonReader(response)
            let wordsWithTranslations = dictionaryResponse.def |> Array.collect ((fun x -> x.tr) >> (fun x -> x |> Array.map (fun y -> y.pos, y.text)))
            return wordsWithTranslations
        }

//TranslateAPI   
    let askYaTranslateAsync (word:string) : Async<string[]> = 
     async { 
            let apiKey = translateApiKey
            let apiEndpoint = translateApiEndPoint
            let askYa = askYandexTranslate (GetJsonAsync) (deserializeJsonTranslateResponse) apiEndpoint apiKey
            let! response = askYa word
            return response
    }

    let askYaTranslateAsyncf (word:string) f : Async<string[]>= 
        async { 
            let! split = askYaTranslateAsync word
            f()
            return split
        }

//DictionaryAPI
    let askYaDictionaryAsync words = 
        async { 
            let apiKey = dictionaryApiKey
            let apiEndpoint = dictionaryApiEndPoint
            let askYa = askYandexDictionary (GetJsonAsync) (deserializeJsonDictionaryResponse) apiEndpoint apiKey
            let! response = askYa words
            return response
        } 

    let askYaDictionaryAsyncf words f = 
        async { 
            let! translations = askYaDictionaryAsync words
            f()
            return translations
        } 