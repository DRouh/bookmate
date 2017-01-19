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
    let private readJsonConfigurationFromFile jsonFileName  = 
        Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), jsonFileName)
        |> File.ReadAllText

    let private loadJsonConfigurationFromFile = readJsonConfigurationFromFile >> JsonConvert.DeserializeObject<ApplicationConfiguration>

    let applicationLoaderFromJsonFile (jsonFileName) = 
        jsonFileName |> loadJsonConfigurationFromFile

    let private applicationConfiguration =  lazy(applicationJsonActual |> applicationLoaderFromJsonFile)

    let public getApplicationConfiguration () = applicationConfiguration.Value

    let getYandexTranslateApiEndPoint = getApplicationConfiguration().YandexTranslateApiEndPoint
    let getYandexTranslateApiKey = getApplicationConfiguration().YandexTranslateApiKey
    let getYandexDictionaryApiEndPoint = getApplicationConfiguration().YandexDictionaryApiEndPoint
    let getYandexDictionaryApiKey = getApplicationConfiguration().YandexDictionaryApiKey
    let getDBConnectionString = getApplicationConfiguration().DBConnectionString |> System.IO.Path.GetFullPath
    let getUserDefinedWordsFilePath = getApplicationConfiguration().UserDefinedWordsFilePath
    let getStanfordModelFolder = getApplicationConfiguration().StanfordModelFolder