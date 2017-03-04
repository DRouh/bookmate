namespace BookMate.Processing

module Translation = 
    open System.Text.RegularExpressions
    open BookMate.Core.Helpers.RegexHelper
    open BookMate.Processing.POS
    open BookMate.Processing.Epub.Domain

    let words (text : string) = 
        [ let mutable start = 0
          let mutable index = 0
          let delim = [| ' ' |]
          index <- text.IndexOfAny(delim, start)
          while index <> -1 do
              if index - start > 0 then yield text.Substring(start, index - start)
              yield text.Substring(index, 1)
              start <- index + 1
              index <- text.IndexOfAny(delim, start)
          if start < text.Length then yield text.Substring(start) ]
    
    let unwords = List.reduce (+)
    let (=~) target regex = System.Text.RegularExpressions.Regex.IsMatch(target, regex)
    let (><) lst value = List.contains value lst
    let (!*) (xs:'a list) (ys:'a list) = xs |> List.exists (fun x -> List.contains x ys)
    let translateWord original translation = original + "{" + translation + "}"
    let toString (xs:string list) = xs |> String.concat ","
    
    let applyTranslations (taggedWords : TaggedWord list) (translations : Translation list) (text : string) = 
        let translationsForPos (translations:(string*CommonPoS) list) (pos:CommonPoS list) = translations |> List.where (fun (_, tag) -> pos >< tag) |> List.map (fst) |> toString

        let mutable processedText = text
        let translationGroupedForWord = 
            translations 
            |> List.map (fun (Word original, Word translation, tags) -> (Word (original.ToLower()), Word (translation.ToLower()), tags))
            |> List.distinct
            |> List.groupBy (fun ((Word original), (Word _), _) -> original)
            |> List.map 
                    (fun (word, ts) -> (word, ts |> List.map (fun ((Word _), (Word translation), tags) -> (translation, tags))))
            
        for (original, ts) in translationGroupedForWord do
            let translationTags = ts |> List.map (snd) 
            let allTranslationsForWord = ts |> List.map (fst) |> toString

            let taggedWordOccurrences = 
                taggedWords
                |> List.filter (fun (w, _) -> w =~ original)
                |> Array.ofList
                |> Array.map (fun (_, tags) -> tags)

            let pickTranslations word c = 
                if word =~ original then
                    let wordTags = taggedWordOccurrences.[c - 1]
                    if wordTags |> !* translationTags then translateWord word (translationsForPos ts wordTags)
                    else word + "{" + allTranslationsForWord + "}"
                else word

            let taggedWordInSentence = 
                processedText
                |> words
                |> List.map (fun w -> (w, if w =~ original then 1 else 0))
                |> List.scan (fun (_, ac) (w, c) -> (w, if w =~ original then ac + 1 else ac)) ("", 0)
                |> List.map (fun (w, c) -> pickTranslations w c)
                |> unwords
            processedText <- taggedWordInSentence
        processedText
    
    let translateText tagWords lookup matcher wordsToTranslate text = 
        let taggedWords = tagWords text //todo refactor and pass as an arguments
        let translations = lookup text //todo refactor and pass as an arguments
        let translatedText = applyTranslations taggedWords translations text //todo refactor and pass as an arguments
        translatedText