namespace BookMate.Integration

module Yandex =   
   type YandexTranslateResponse = {
        code: int
        lang: string
        text: string[]
    }

    type YandexDictionaryResponse = {
        ///Result header (not used).
        head : obj
        ///Array of dictionary entries. A transcription of the search word may be provided in the ts attribute
        def : Definition[] }
    and Definition ={
        text: string
        pos: string
        ///Part of speech (may be omitted).
        ts: string 
        ///Array of translations.
        tr: Translation[] 
    }
    and Translation = {
        ///Text of the entry, translation, or synonym (mandatory).
        text: string 
        ///Part of speech (may be omitted).
        pos: string 
        ///Array of translations.
        syn: Synonym[] 
        ///Array of meanings.
        mean: Meaning[] 
        ///Array of examples.
        ex: Example[] 
    }
    and Synonym = {
        ///Text synonym (mandatory).
        text: string 
    }
    and Meaning = {
        ///Text of the entry.
        text: string
    }
    and Example = {
        ///Text of the synonym (mandatory).
        text: string
        ///Array of translations.
        tr : ExampleTranslation[]
    }
    and ExampleTranslation = {
        text: string
    }

   type GetResponseFetcher = string -> Async<string*int>
   type YandexTranslateResponseBuilder = string -> YandexTranslateResponse
   type YandexDictionaryResponseBuilder = string -> YandexDictionaryResponse