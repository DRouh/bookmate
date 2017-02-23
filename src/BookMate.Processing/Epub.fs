namespace BookMate.Processing

module Epub = 
    type Extension = 
        | Epub
        | AnyHtml
    
    type FilePath = 
        | EpubFilePath of string
        | AnyHtmlFilePath of string
    
    type UnpackedDirPath = 
        | UnpackedDirPath of string
    
    type PackDirPath = 
        | PackDirPath of string
    
    type UnpackedPath = 
        | UnpackedPath of FilePath * UnpackedDirPath
    
    type BookToProcess = 
        { Files : List<EpubFile>
          Location : UnpackedPath }
    
    and EpubFile = 
        { Name : string
          Path : FilePath
          Content : string }
