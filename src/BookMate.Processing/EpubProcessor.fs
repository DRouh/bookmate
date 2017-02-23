namespace BookMate.Processing

module EpubProcessor = 
    open System
    open System.IO
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO
    
    type BookToProcess = 
        { Files : List<EpubFile>
          Location : UnpackedPath }
    
    and EpubFile = 
        { name : string
          path : EpubFilePath
          content : string }

    let addToBookToProcess (file: EpubFilePath) = 
        { name = ""; path = file; content = "" }
    
    let toAnyHtml = toFilePath AnyHtml

    let readBook (unpackedBook : UnpackedPath) : BookToProcess = 
        match unpackedBook with
        | UnpackedPath (FilePath filePath, UnpackedDirPath dirPath) ->
            let files = 
                Directory.GetFiles(dirPath, "*.*html", System.IO.SearchOption.AllDirectories)
                |> Seq.toList
                |> List.choose toAnyHtml
                |> List.map addToBookToProcess

            { Files = files
              Location = unpackedBook }