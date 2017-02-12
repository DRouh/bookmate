namespace BookMate.Processing

module EpubProcessor = 
    type FilePath = 
        | ValidFilePath of string
        | InvalidFilePath
    
    type DirPath = 
        | ValidDirPath of string
        | InvalidDirPath
    
    type EpubBookPath = 
        | UnpackedPath of FilePath * DirPath
        | InvalidPath
    
    let toFilePath (filePath : string) = 
        if System.IO.File.Exists filePath then ValidFilePath filePath
        else InvalidFilePath
    
    let unpackBook (bookPath : FilePath) : EpubBookPath = 
        match bookPath with
        | ValidFilePath filePath -> UnpackedPath(InvalidFilePath, InvalidDirPath)
        | _ -> InvalidPath
