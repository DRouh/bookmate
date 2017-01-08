namespace BookMate.Core

module Configuration = 
    //open FSharp.Data
    
    [<LiteralAttribute>]
    let applicationXMLSample = @"appconfiguration.sample.xml"
    
    [<LiteralAttribute>]
    let applicationXMLActual = @"appconfiguration.xml"
    
   // type private XMLApplicationConfiguration = XmlProvider< Sample=applicationXMLSample, Global=true >
    
 //   let private config = XMLApplicationConfiguration.Load(applicationXMLActual)

    let getYandexTranslateApiEndPoint = ""// config.YandexTranslateApiEndPoint
    let getYandexTranslateApiKey = ""//config.YandexTranslateApiKey
    let getYandexDictionaryApiEndPoint =""// config.YandexDictionaryApiEndPoint
    let getYandexDictionaryApiKey =""// config.YandexDictionaryApiKey
    let getDBConnectionString =""// config.DbConnectionString |> System.IO.Path.GetFullPath
    let getUserDefinedWordsFilePath =""// config.UserDefinedWordsFilePath
    let getStanfordModelFolder =""// config.StanfordModelFolder