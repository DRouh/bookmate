namespace BookMate.Core
module Configuration = 
    open FSharp.Data

    [<LiteralAttribute>]let applicationXMLSample = @"appconfiguration.sample.xml"

    [<LiteralAttribute>]let applicationXMLActual = @"appconfiguration.xml"

    type private XMLApplicationConfiguration = XmlProvider<Sample=applicationXMLSample, Global=true>

    let currDirectory = System.IO.Directory.GetCurrentDirectory()
    let private config = XMLApplicationConfiguration.Load(applicationXMLActual)
    //
    //provide appropriate values
    let getYandexTranslateApiEndPoint = config.YandexTranslateApiEndPoint
    let getYandexTranslateApiKey = config.YandexTranslateApiKey
    let getYandexDictionaryApiEndPoint = config.YandexDictionaryApiEndPoint
    let getYandexDictionaryApiKey = config.YandexDictionaryApiKey
    let getDBConnectionString = config.DbConnectionString |> System.IO.Path.GetFullPath
    let getUserDefinedWordsFilePath = config.UserDefinedWordsFilePath
    let getStanfordModelFolder = config.StanfordModelFolder