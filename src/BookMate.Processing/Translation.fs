namespace BookMate.Processing

module Translation = 
  open System.Text.RegularExpressions
  open BookMate.Core.Helpers.RegexHelper
  open BookMate.Processing.POS
  open BookMate.Processing.Epub.Domain
  let words (text : string) = 
      [ let mutable start = 0
        let mutable index = 0
        let delim = [| ' '; '.'; '!'; '?' |]
        index <- text.IndexOfAny(delim, start)
        while index <> -1 do
            if index - start > 0 then yield text.Substring(start, index - start)
            yield text.Substring(index, 1)
            start <- index + 1
            index <- text.IndexOfAny(delim, start)
        if start < text.Length then yield text.Substring(start) ]

  let unwords = List.reduce (+)
  let (=~) target regex = System.Text.RegularExpressions.Regex.IsMatch(target, regex, RegexOptions.IgnoreCase)
  let isIn xs y = List.contains y xs
  let doIntersectWith (xs : 'a list) (ys : 'a list) = xs |> List.exists (fun x -> List.contains x ys)
  let toTranslatedForm original translation = original + "{" + translation + "}"
  let toString (xs : string list) = xs |> String.concat ","

  let toGroupedTranslation translations = 
      let normalizeTranslation = function 
          | (Word original, Word translation, tags) -> (Word(original.ToLower()), Word(translation.ToLower()), tags)
      let mapOriginalToAllTranslations (word, ts) = 
          (word, ts |> List.map (fun ((Word _), (Word translation), tags) -> (translation, tags)))
      
      let translationGroupedForWord = 
          translations
          |> List.map normalizeTranslation
          |> List.distinct
          |> List.groupBy (fun ((Word original), (Word _), _) -> original)
          |> List.map mapOriginalToAllTranslations
      translationGroupedForWord

  let pickTranslationsForPos translations tags = 
      translations
      |> List.where (fun (_, tag) -> tag |> isIn tags)
      |> List.map (fst)
      |> toString

  let applyWordTranslations taggedWordsInSentence text (originalWord, wordTranslationAndTags) = 
      let tagsForOriginalWord = wordTranslationAndTags |> List.map (snd)
      
      let allTranslationsForWord = 
          wordTranslationAndTags
          |> List.map (fst)
          |> toString
      
      let taggedWordOccurrences = 
          taggedWordsInSentence
          |> Array.ofList
          |> Array.filter (fun (w, _) -> w =~ originalWord)
          |> Array.map (fun (_, tags) -> tags)
      
      let pickTranslations word c = 
          if word =~ originalWord then 
              let wordTags = taggedWordOccurrences.[c - 1]
              if wordTags |> doIntersectWith tagsForOriginalWord then 
                  wordTags
                  |> pickTranslationsForPos wordTranslationAndTags
                  |> toTranslatedForm word
              else allTranslationsForWord |> toTranslatedForm word
          else word
      
      let applyTranslations words = 
          let runningCountOccurrences (_, acc) word = 
              let count = 
                  if word =~ originalWord then acc + 1
                  else acc
              (word, count)
          words
          |> List.scan runningCountOccurrences ("", 0)
          |> List.map (fun (word, count) -> pickTranslations word count)
      
      text
      |> words
      |> applyTranslations
      |> unwords

  let applyTranslations (taggedWords : TaggedWord list) (translations : Translation list) (text : string) = 
      let translationGroupedForWord = toGroupedTranslation translations
      let applyWordTranslations' = taggedWords |> applyWordTranslations
      translationGroupedForWord |> List.fold applyWordTranslations' text
  let translateText tagWords lookup matcher wordsToTranslate text = 
      let taggedWords = tagWords text //todo refactor and pass as an arguments
      let translations = lookup text //todo refactor and pass as an arguments
      let translatedText = applyTranslations taggedWords translations text //todo refactor and pass as an arguments
      translatedText