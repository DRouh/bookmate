namespace BookMate.Core

module Configuration = 
    open System.IO
    open Newtonsoft.Json
    open System.Reflection
    
    type ApplicationConfiguration = {
        YandexTranslateApiEndPoint: string
        YandexTranslateApiKey: string
        YandexDictionaryApiEndPoint: string
        YandexDictionaryApiKey: string
        DBConnectionString: string
        UserDefinedWordsFilePath: string
        StanfordModelFolder: string
    }
    let applicationJsonActual = @"appconfiguration.json"

    let private configStr = 
        Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), applicationJsonActual)
        |> File.ReadAllText
    let public config = JsonConvert.DeserializeObject<ApplicationConfiguration>(configStr)

    let getYandexTranslateApiEndPoint = config.YandexTranslateApiEndPoint
    let getYandexTranslateApiKey = config.YandexTranslateApiKey
    let getYandexDictionaryApiEndPoint = config.YandexDictionaryApiEndPoint
    let getYandexDictionaryApiKey = config.YandexDictionaryApiKey
    let getDBConnectionString = config.DBConnectionString |> System.IO.Path.GetFullPath
    let getUserDefinedWordsFilePath = config.UserDefinedWordsFilePath
    let getStanfordModelFolder = config.StanfordModelFolder