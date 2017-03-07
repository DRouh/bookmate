namespace BookMate.Processing.Epub

module Domain = 
    open BookMate.Processing.POS
    type Extension = 
        | Epub
        | AnyHtml
    
    type FilePath = 
        | EpubFilePath of string
        | AnyHtmlFilePath of string
    
    type UnpackedDirPath = UnpackedDirPath of string
    
    type PackDirPath = PackDirPath of string
    
    type UnpackedPath = UnpackedPath of FilePath * UnpackedDirPath
    
    type SaveResultPath = SaveResultPath of string

    type FileInEpub = 
        { Name : string
          Path : FilePath
          Content : string }
    
    type RawFileInEpub = FileInEpub
    type ProcessedFileInEpub = FileInEpub

    type BookToProcess = 
        { Files : List<RawFileInEpub>
          Location : UnpackedPath }

    type ProcessedBook = 
        { Files : List<ProcessedFileInEpub> }

    type Word = Word of string
    type TaggedWord = string*(CommonPoS list)
    type Translation = Word*Word*CommonPoS