namespace BookMate.Processing

module EpubProcessor = 
    open System.IO
    open System.Text.RegularExpressions
    open BookMate.Core.Helpers.RegexHelper

    type FilePath = FilePath of string
    
    type DirPath = 
        | ToPackDirPath of string
        | UnpackedDirPath of string
        | InvalidDirPath
    
    type EpubBookPath = UnpackedPath of FilePath * DirPath
    
    let invalidPatter = "" //"[" + Regex.Escape(Path.GetInvalidPathChars()) + "]"

    let isPlausibleDirPath = function
        | Regex invalidPatter v -> false
        | _ -> false

    let toPackDirPath (dirPath : string) = 
        match dirPath with
        | "" | null -> InvalidDirPath
        | dp when dp.Trim() <> ""-> 
                if not (System.IO.Directory.Exists dirPath) then ToPackDirPath dirPath
                else InvalidDirPath
        | _ -> InvalidDirPath
    
    let toFilePath (filePath : string) = 
        if System.IO.File.Exists filePath then 
            if filePath.ToLower().EndsWith(".epub") then (FilePath filePath) |> Some
            else None
        else None
    
    let unpackBook (bookPath : FilePath) (savePath : string) : EpubBookPath option = 
        match bookPath with
        | FilePath filePath -> 
            UnpackedPath(bookPath, UnpackedDirPath savePath) |> Some
        | _ -> None
