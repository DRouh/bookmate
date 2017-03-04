namespace BookMate.Processing.Epub

module Processor = 
    open System
    open System.IO
    open System.Text.RegularExpressions

    open BookMate.Core.Helpers.RegexHelper
    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO 
    open BookMate.Processing.POS
    
    let getFileName = Path.GetFileNameWithoutExtension
    let readAllText = File.ReadAllText
    
    let toEpubReadFile (file : FilePath) = 
        let (AnyHtmlFilePath fp) = file
        let fileName = getFileName fp
        let fileContent = readAllText fp
        { Name = fileName
          Path = file
          Content = fileContent }
    
    let toAnyHtml = toFilePath AnyHtml
    
    let readBook (unpackedBook : UnpackedPath) : BookToProcess option = 
        match unpackedBook with
        | UnpackedPath(EpubFilePath filePath, UnpackedDirPath dirPath) -> 
            let files = 
                Directory.GetFiles(dirPath, "*.*html", System.IO.SearchOption.AllDirectories)
                |> Seq.toList
                |> List.choose toAnyHtml
                |> List.map toEpubReadFile
            { Files = files
              Location = unpackedBook }
            |> Some
        | _ -> None
    
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
    let (=~) target regex = System.Text.RegularExpressions.Regex.IsMatch(target, regex, RegexOptions.IgnoreCase)
    let (><) lst value = List.contains value lst
    let (!*) (xs:'a list) (ys:'a list) = xs |> List.exists (fun x -> List.contains x ys)
    let translateWord original translation = original + "{" + translation + "}"
    
    let applyTranslations (taggedWords : TaggedWord list) (translations : Translation list) (text : string) = 
        let toExactMatchPattern word = @"\b" + word + @"\b"
        let distinctTranslations = translations |> List.distinct
        let exactPosTranslations = 
            [ for (word, poss) in taggedWords do
                    for (Word o, Word t, p) in distinctTranslations do
                        if word =~ o && poss >< p then yield (Word o, Word t, p) ]

        let translateFuzzy fuzzyTranslations text = 
            let mutable processedText = text
            for (Word original, Word translation, _) in fuzzyTranslations do
                let pattern = toExactMatchPattern original
                let replacement = translateWord original translation
                processedText <- regexReplace processedText pattern replacement
            processedText
        
        let translateExactPos exactPosTranslations text = 
            let toString (xs:string list) = xs |> String.concat ","
            let translationsForPos (translations:(string*CommonPoS) list) (pos:CommonPoS list) = translations |> List.where (fun (_, tag) -> pos >< tag) |> List.map (fst) |> toString

            let mutable processedText = text
            let translationGroupedForWord = 
                exactPosTranslations
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
                        if wordTags |> !* translationTags then translateWord original (translationsForPos ts wordTags)
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

        let fuzzyTranslations = 
            (Set.difference (Set.ofList distinctTranslations) (Set.ofList exactPosTranslations)) |> List.ofSeq
        text
        |> translateFuzzy fuzzyTranslations
        |> translateExactPos exactPosTranslations
    
    let translateText tagWords lookup matcher wordsToTranslate text = 
        let taggedWords = tagWords text //todo refactor and pass as an arguments
        let translations = lookup text //todo refactor and pass as an arguments
        let translatedText = matcher taggedWords translations text //todo refactor and pass as an arguments
        translatedText