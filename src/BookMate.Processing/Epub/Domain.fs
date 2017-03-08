namespace BookMate.Processing.Epub

module Domain = 
  open BookMate.Processing.POS
  
  type FileExtension = 
      | Epub
      | AnyHtml
  
  type FilePath = 
      | EpubFilePath of string
      | AnyHtmlFilePath of string
  
  type OriginalBookLocation = 
      | UnpackedBookPath of string
  
  type ExtractTargetPath = 
      | ExtractTargetPath of string
  
  type BookLocation = FilePath * OriginalBookLocation
  
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
  
  type TaggedWord = Word * CommonPoS list
  
  type Translation = Word * Word * CommonPoS