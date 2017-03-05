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
  let (=~) target regex = 
      let exact = @"\b" + regex + @"\b"
      System.Text.RegularExpressions.Regex.IsMatch(target, exact, RegexOptions.IgnoreCase)
  let isIn xs y = List.contains y xs
  let doesIntersectWith xs ys = xs |> List.exists (fun x -> List.contains x ys)
  let toTranslatedForm original translation = original + "{" + translation + "}"
  let toString xs = xs |> String.concat ","

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

  let forTagTranslations wordTranslationAndTags wordTags maxNumOfTranslationsPerWord = 
      wordTranslationAndTags
      |> List.where (fun (_, tag) -> tag |> isIn wordTags)
      |> List.map (fst)
      |> Seq.truncate maxNumOfTranslationsPerWord
      |> toString

  let pickTranslationsForPos originalWord originalWordTags allWordTranslations forTagTranslations wordTags = 
      let translation = 
          if wordTags |> doesIntersectWith originalWordTags then forTagTranslations
          else allWordTranslations
      translation |> toTranslatedForm originalWord

  let toWordTaggedOccurrences taggedWordsInSentence word = 
      taggedWordsInSentence
      |> Array.ofList
      |> Array.filter (fun (currentWord, _) -> currentWord =~ word)
      |> Array.map (fun (_, tags) -> tags)

  let toTranslationsList wordTranslationAndTags maxNumOfTranslationsPerWord = 
      wordTranslationAndTags
      |> List.map (fst)
      |> Seq.truncate maxNumOfTranslationsPerWord
      |> toString

  let applyWordTranslations maxNumOfTranslationsPerWord taggedWordsInSentence text (originalWord, wordTranslationAndTags) = 
      let tagsForOriginalWord = wordTranslationAndTags |> List.map (snd)
      let allTranslationsForWord = toTranslationsList wordTranslationAndTags maxNumOfTranslationsPerWord
      let taggedWordOccurrences = toWordTaggedOccurrences taggedWordsInSentence originalWord 
      
      let pickTranslations (currentWord, count) = 
          if currentWord =~ originalWord then 
              let wordTags = taggedWordOccurrences.[count - 1]
              let forTagTranslations = forTagTranslations wordTranslationAndTags wordTags maxNumOfTranslationsPerWord
              wordTags |> pickTranslationsForPos currentWord tagsForOriginalWord allTranslationsForWord forTagTranslations
          else currentWord
      
      let applyTranslations words = 
          let runningCountOccurrences (_, acc) word = 
              let count = 
                  if word =~ originalWord then acc + 1
                  else acc
              (word, count)
          words
          |> List.scan runningCountOccurrences ("", 0)
          |> List.map pickTranslations
      
      text
      |> words
      |> applyTranslations
      |> unwords
    
  let applyTranslations taggedWords translations text maxNumOfTranslationsPerWord = 
      let applyWordTranslations' = taggedWords |> applyWordTranslations maxNumOfTranslationsPerWord 
      translations
      |> toGroupedTranslation
      |> List.fold applyWordTranslations' text  
      
  let translateText tagWords lookup matcher wordsToTranslate text = 
      let taggedWords = tagWords text //todo refactor and pass as an arguments
      let translations = lookup text //todo refactor and pass as an arguments
      let translatedText = applyTranslations taggedWords translations text 3 //todo refactor and pass as an arguments
      translatedText