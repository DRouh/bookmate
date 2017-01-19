namespace BookMate

module CommonTest = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Core.Configuration

    let expectedConfiguration () : ApplicationConfiguration = 
        {
            YandexTranslateApiEndPoint="1234"
            YandexTranslateApiKey="12345"
            YandexDictionaryApiEndPoint="1234"
            YandexDictionaryApiKey="1234"
            DBConnectionString="1234"
            UserDefinedWordsFilePath="1234"
            StanfordModelFolder="1234"
        }

    [<Fact>]
    let ``Configuration test`` () = 
        let expectedConfiguration = expectedConfiguration()
        expectedConfiguration.YandexTranslateApiEndPoint = "1234" |> should be True 
        
        //BookMate.Core.Configuration.testTest() = 3 |> should be True
        //BookMate.Core.Configuration.applicationConfiguration = BookMate.Core.Configuration.applicationConfiguration |> should be True
        //1=1 |> should be True
        //Configuration.applicationConfiguration.getYandexTranslateApiEndPoint = Configuration.applicationConfiguration.getYandexTranslateApiEndPoint
