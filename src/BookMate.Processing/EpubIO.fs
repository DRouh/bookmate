namespace BookMate.Processing

module EpubIO = 
    open System.IO
    open System.Text.RegularExpressions
    open BookMate.Core.Helpers.RegexHelper
    open System
    open System.Text.RegularExpressions
    open BookMate.Core.Helpers.IOHelper
    
    type FilePath = FilePath of string
    
    type DirPath = 
        | PackDirPath of string
        | UnpackedDirPath of string
        | InvalidDirPath
    
    type EpubBookPath = UnpackedPath of FilePath * DirPath
    
    let toPackDirPath (dirPath : string) = 
        let removeDoubleSlash (p : string) = p.Replace("\\", @"\")
        let isNotRelative = Path.IsPathRooted
        
        let notContainInvalidChars = 
            let invalidPatter = Path.GetInvalidPathChars() |> string
            let invalidCharRegex = new Regex("[" + Regex.Escape(invalidPatter) + "]")
            let notContainInvalidChars = invalidCharRegex.IsMatch >> not
            notContainInvalidChars
        
        let validPatternPath = 
            let validPathRegex = new Regex("^([a-zA-Z]:)?(\\\\[^<>:\"/\\\\|?*]+)+\\\\?$")
            validPathRegex.IsMatch
        
        let isValidPath path = 
            let p = removeDoubleSlash path
            notContainInvalidChars p && validPatternPath p && isNotRelative p
        
        match dirPath with
        | null | "" -> InvalidDirPath
        | dp when isValidPath dirPath -> 
            if not (System.IO.Directory.Exists dirPath) then PackDirPath dirPath
            else InvalidDirPath
        | _ -> InvalidDirPath
    
    let toFilePath (filePath : string) = 
        if System.IO.File.Exists filePath then 
            if filePath.ToLower().EndsWith(".epub") then (FilePath filePath) |> Some
            else None
        else None
        
    let getFileName = Path.GetFileNameWithoutExtension

    let unpackBook (bookPath : FilePath) (savePath : DirPath) : EpubBookPath option = 
            match (bookPath, savePath) with
            | (FilePath fp, PackDirPath pdp) -> 
                        createFolder pdp |> ignore

                        let fileName = getFileName fp
                        let zipFileName = pdp +/ (fileName + ".zip")

                        //set normal attribute to allow deletion
                        do File.Copy(fp, zipFileName)
                        do File.SetAttributes(zipFileName, FileAttributes.Normal) 
                        do unzipFile zipFileName pdp
                        do File.Delete zipFileName

                        UnpackedPath(bookPath, UnpackedDirPath pdp) |> Some
            | _ -> None
