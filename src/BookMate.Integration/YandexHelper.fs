namespace BookMate.Integration

module YandexHelper = 
    open BookMate.Core.Helpers.StringHelper
    open Newtonsoft.Json
    let private translateApiKey =  BookMate.Core.Configuration.getYandexTranslateApiKey()
    let private translateApiEndPoint = BookMate.Core.Configuration.getYandexTranslateApiEndPoint()
    let private dictionaryApiKey = BookMate.Core.Configuration.getYandexDictionaryApiKey() 
    let private dictionaryApiEndPoint = BookMate.Core.Configuration.getYandexDictionaryApiEndPoint()
    
    type YandexTranslateResponse = {
        code: int
        lang: string
        text: string[]
    }

    type YandexDictionaryResponse = {
        head : obj
        def : Definition[] }
    and Definition ={
        text: string
        pos: string
        ts: string
        tr: Translation[]
    }
    and Translation = {
        text: string
        pos: string
    }

    let private ComposeUrl valuePairs = 
        if Seq.isEmpty valuePairs then ""
        else 
            let values = valuePairs |> Seq.map (fun (k, v) -> sprintf "%s=%s" k v)
            "?" + System.String.Join("&", values)

    let private GetAsync (endpoint:string) (parameters: seq<string*string>) : Async<string>=
        async {
            let query = 
                parameters
                |> ComposeUrl

            let url = endpoint + query

            let uri = new System.Uri(url, System.UriKind.Absolute)
            let client = new System.Net.Http.HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")

            let! response = 
                client.GetAsync(uri)
                |> Async.AwaitTask
            let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return body
        }

    let private askYandexTranslate apiEndpoint apiKey word = 
        async {
            let! response = GetAsync apiEndpoint [ ("key", apiKey); ("text", word); ("lang", "en-ru"); ("format", "plain")] 
            let translateResponse = JsonConvert.DeserializeObject<YandexTranslateResponse>(response)
            let translations = translateResponse.text |> Array.collect ((unstringify) >> (Array.map (fun x -> x.Trim())))
            return translations
        }
        
    let private askYandexDictionary apiEndpoint apiKey words : Async<(string*string)[]> = 
        async {
            let! response = GetAsync apiEndpoint [ ("key", apiKey); ("text", words); ("lang", "en-ru"); ("format", "plain")] 
            let dictionaryResponse = JsonConvert.DeserializeObject<YandexDictionaryResponse>(response)
            let wordsWithTranslations = dictionaryResponse.def |> Array.collect ((fun x -> x.tr) >> (fun x -> x |> Array.map (fun y -> y.pos, y.text)))
            return wordsWithTranslations
        }

//TranslateAPI   
    let askYaTranslateAsync (word:string) : Async<string[]> = 
     async { 
            let apiKey = translateApiKey
            let apiEndpoint = translateApiEndPoint
            let askYa = askYandexTranslate apiEndpoint apiKey
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
            let askYa = askYandexDictionary apiEndpoint apiKey
            let! response = askYa words
            return response
        } 
    let askYaDictionaryAsyncf words f = 
        async { 
            let! translations = askYaDictionaryAsync words
            f()
            return translations
        } 