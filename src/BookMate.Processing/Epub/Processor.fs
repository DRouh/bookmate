namespace BookMate.Processing.Epub

module Processor =
  open System
  open System.IO
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
  
  let applyTranslations (taggedWords: TaggedWord list) (translations: Translation list) (text:string) = 
    let mutable processedText = text
    for (Word original, Word translation, pos) in translations do
      let pattern = @"\b" + original + @"\b"
      let replacement = original + "{" + translation + "}"
      processedText <- regexReplace processedText pattern replacement 
    processedText

  let translateText tagWords lookup matcher wordsToTranslate text =
    let taggedWords = tagWords text //todo refactor and pass as an arguments
    let translations = lookup text //todo refactor and pass as an arguments
    let translatedText = matcher taggedWords translations text //todo refactor and pass as an arguments
    translatedText