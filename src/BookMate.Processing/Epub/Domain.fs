namespace BookMate.Processing.Epub

module Domain = 
  open BookMate.Processing.POS
  
//IO types
  type FileExtension = 
      | Epub
      | AnyHtml
  
  type FilePath = 
      | EpubFilePath of string
      | AnyHtmlFilePath of string
  
  type OriginalBookLocation = UnpackedBookPath of string
  
  type ExtractTargetPath = ExtractTargetPath of string
  
  type BookLocation = FilePath * OriginalBookLocation
  
//Domain types
  type Word = string
  
  type TaggedWord = Word * CommonPoS list
  
  type Translation = Word * Word * CommonPoS

  type AnalysisData = { WordsToTranslate : Word list; TaggedText : TaggedWord list }
  type UserPrefs =  { WordsToTranslate : Word list; }
  type BookFile = 
      { Name : string
        Path : FilePath
        Content : string }
  
  type OriginalFileInBook = BookFile

  type AnalysedBookFile = { File : BookFile; AnalysisData : AnalysisData }
  
  type ProcessedFileInBook = BookFile
  
  type BookFiles<'a> = List<'a>
  
  type OriginalBook = BookFiles<OriginalFileInBook> * BookLocation
  
  type AnalysedBook = BookFiles<AnalysedBookFile> 
  type ProcessedBook = BookFiles<ProcessedFileInBook>

  type AnalyseBook = UserPrefs -> OriginalBook -> AnalysedBook
  type ProcessBook = AnalysedBook -> Translation list -> ProcessedBook
  type TagText = string -> Async<(Word * CommonPoS list option)[] option>