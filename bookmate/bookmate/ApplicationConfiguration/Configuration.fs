namespace BookMate.ApplicationConfiguration
module Configuration = 
    open FSharp.Configuration
    open System.IO

    type private Settings = AppSettings<"App.config">

    let getYandexTranslateApiEndPoint = Settings.YandexTranslateApiEndPoint |> string
    let getYandexDictionaryApiEndPoint = Settings.YandexDictionaryApiEndPoint |> string
    let getYandexDictionaryApiKey = Settings.YandexDictionaryApiKey
    let getYandexTranslateApiKey = Settings.YandexTranslateApiKey
    let getDBConnectionString = Settings.DatabaseConnectionString |> string |> Path.GetFullPath
