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
    
    let toDirPath (dirPath : string) = 
        match dirPath with
        | "" | null -> InvalidDirPath
        | dp -> 
            if dp.Trim() <> "" then ValidDirPath dirPath
            else InvalidDirPath
    
    let toFilePath (filePath : string) = 
        if System.IO.File.Exists filePath then 
            if filePath.ToLower().EndsWith(".epub") then ValidFilePath filePath
            else InvalidFilePath
        else InvalidFilePath
    
    let unpackBook (bookPath : FilePath) (savePath : string) : EpubBookPath = 
        match bookPath with
        | ValidFilePath filePath -> UnpackedPath(InvalidFilePath, InvalidDirPath)
        | _ -> InvalidPath
