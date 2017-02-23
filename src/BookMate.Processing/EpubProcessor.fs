namespace BookMate.Processing

module EpubProcessor = 
    open System
    open System.IO
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO
    
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
