namespace BookMate.Processing.Epub

module Domain = 
    open BookMate.Processing.POS
    type FileExtension = 
        | Epub
        | AnyHtml
    
    type FilePath = 
        | EpubFilePath of string
        | AnyHtmlFilePath of string
    
    type UnpackedBookPath = UnpackedBookPath of string
    
    type PackDirPath = PackDirPath of string
    
    type BookLocation = FilePath * UnpackedBookPath
    
    type SaveResultPath = SaveResultPath of string

    type BookFile = 
        { Name : string
          Path : FilePath
          Content : string }
    
    type OriginalFileInBook = BookFile
    type ProcessedFileInBook = BookFile

    type BookFiles<'a> = List<'a>

    type OriginalBook = BookFiles<OriginalFileInBook> * BookLocation
    type ProcessedBook = BookFiles<ProcessedFileInBook>

    type Word = string
    type TaggedWord = Word * (CommonPoS list)
    type Translation = Word * Word * CommonPoS