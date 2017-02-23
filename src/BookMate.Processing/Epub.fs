namespace BookMate.Processing

module Epub =

    type Extension = Epub | AnyHtml
    
    type FilePath = string
    type EpubFilePath = FilePath of string
    type UnpackedDirPath = UnpackedDirPath of string
    type PackDirPath = PackDirPath of string
    type UnpackedPath = UnpackedPath of EpubFilePath * UnpackedDirPath