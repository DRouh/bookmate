namespace BookMate.Processing.Epub

module Processor = 
    open System
    open System.IO

    open BookMate.Processing.Epub.Domain
    open BookMate.Processing.Epub.IO 
    open BookMate.Processing.POS

    let getFileName = Path.GetFileNameWithoutExtension
    let readAllText = File.ReadAllText
    
    let toFileInEpub (file : FilePath) = 
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
                |> List.map toFileInEpub
            { Files = files
              Location = unpackedBook } : BookToProcess
            |> Some
        | _ -> None

    let processFileInEpub (rawFile: FileInEpub) : ProcessedFileInEpub = raise (NotImplementedException "Not ready")
    let processEpubBook (rawBook: BookToProcess) : ProcessedBook = raise (NotImplementedException "Not ready")