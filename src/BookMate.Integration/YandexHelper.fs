namespace BookMate.Integration

module YandexHelper = 
    open BookMate.Core.Helpers.StringHelper
    open FSharp.Data
    
    let private translateApiKey = BookMate.Core.Configuration.getYandexTranslateApiKey
    let private translateApiEndPoint =  BookMate.Core.Configuration.getYandexTranslateApiEndPoint
    let private dictionaryApiKey = BookMate.Core.Configuration.getYandexDictionaryApiKey
    let private dictionaryApiEndPoint = BookMate.Core.Configuration.getYandexDictionaryApiEndPoint
    
    type private TranslateYandexResponse = JsonProvider< """ {"code": 200,"lang": "en-ru","text": [ "раз, два"]} """ >
    type private DictionaryYandexResponse = JsonProvider< """ {"head":{},"def":[{"text":"one","pos":"verb","ts":"fʌk","tr":[{"text":"раз","pos":"verb","asp":"несов","syn":[{"text":"два","pos":"verb","asp":"сов"},{"text":"три","pos":"verb","asp":"сов"},{"text":"четыре","pos":"verb"}],"mean":[{"text":"shag"},{"text":"have"}]},{"text":"пять","pos":"verb","asp":"сов","syn":[{"text":"шесть","pos":"verb","asp":"сов"}]}]},{"text":"one","pos":"adverb","ts":"fʌk","tr":[{"text":"к черту","pos":"adverb"}]}]} """ >
    
    let private askYandexTranslate apiEndpoint apiKey words = 
        Http.AsyncRequestString(apiEndpoint, httpMethod = "GET", 
                                query = [ "key", apiKey
                                          "text", words
                                          "lang", "en-ru"
                                          "format", "plain" ], headers = [ "Accept", "application/json" ])

    let private askYandexDictionary apiEndpoint apiKey words = 
        Http.AsyncRequestString(apiEndpoint, httpMethod = "GET", 
                    query = [ "key", apiKey
                              "text", words
                              "lang", "en-ru" ], headers = [ "Accept", "application/json" ])
//TranslateAPI   

    let askYaTranslateAsync words = 
        async { 
            let apiKey = translateApiKey
            let apiEndpoint = translateApiEndPoint
            let askYa = askYandexTranslate apiEndpoint apiKey
            let! response = askYa words
            let text = response |> TranslateYandexResponse.Parse
            let split = text.Text |> Array.collect ((unstringify) >> (Array.map (fun x -> x.Trim())))
            return split
        }
         
    let askYaTranslateAsyncf words f = 
        async { 
            let! split = askYaTranslateAsync words
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
            let text = response |> DictionaryYandexResponse.Parse
            let translations = text.Def |> Array.collect ((fun x -> x.Tr) >> (fun x -> x |> Array.map (fun y -> y.Pos, y.Text)))
            return translations
        } 
    let askYaDictionaryAsyncf words f = 
        async { 
            let! translations = askYaDictionaryAsync words
            f()
            return translations
        } 