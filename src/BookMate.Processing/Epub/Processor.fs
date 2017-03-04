namespace BookMate.Processing.Epub

module Processor = 
    open System
    open System.IO
    open System.Text.RegularExpressions

    open BookMate.Core.Helpers.RegexHelper
    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO   
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
    let (=~) target regex = System.Text.RegularExpressions.Regex.IsMatch(target, regex)
    let (><) lst value = List.contains value lst
    
    let applyTranslations (taggedWords : TaggedWord list) (translations : Translation list) (text : string) = 
        let toExactMatchPattern word = @"\b" + word + @"\b"
        let translateWord original translation = original + "{" + translation + "}"
        
        let translateFuzzy fuzzyTranslations text = 
            let mutable processedText = text
            for (Word original, Word translation, _) in fuzzyTranslations do
                let pattern = toExactMatchPattern original
                let replacement = translateWord original translation
                processedText <- regexReplace processedText pattern replacement
            processedText
        
        let translateExactPos exactPosTranslations text = 
            let mutable processedText = text
            for (Word original, Word translation, pos) in exactPosTranslations do
                let taggedWordOccurrences = 
                    taggedWords
                    |> List.filter (fun (w, _) -> w =~ original)
                    |> List.map (fun (_, tags) -> tags)
                
                let taggedWordInSentence = 
                    processedText
                    |> words
                    |> List.map (fun w -> 
                           (w, 
                            if w =~ original then 1
                            else 0))
                    |> List.scan (fun (_, ac) (w, c) -> 
                           (w, 
                            if c <> 0 then ac + c
                            else ac)) ("", 0)
                    |> List.map (fun (w, c) -> 
                           (w, 
                            if w =~ original then taggedWordOccurrences.[c - 1] >< pos
                            else false))
                    |> List.map (function 
                           | (word, toTranslate) -> 
                               if toTranslate then translateWord original translation
                               else word
                           | _ -> "")
                    |> unwords
                processedText <- taggedWordInSentence
            processedText
        
        let distinctTranslations = translations |> List.distinct
        
        let exactPosTranslations = 
            [ for (word, poss) in taggedWords do
                  for (Word o, Word t, p) in distinctTranslations do
                      if word =~ o && poss >< p then yield (Word o, Word t, p) ]
        
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