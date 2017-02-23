namespace BookMate.Processing

module EpubIO = 
    open System
    open System.IO
    open System.Text.RegularExpressions
    open BookMate.Core.Helpers.RegexHelper
    open BookMate.Core.Helpers.IOHelper
    open BookMate.Processing.Epub
    
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
        | null | "" -> None
        | dp when isValidPath dirPath -> 
            if not (System.IO.Directory.Exists dirPath) then Some(PackDirPath dirPath)
            else None
        | _ -> None
    
    let (|IsEpub|_|) = 
        function 
        | (_, "") -> None
        | (Epub, fp) when fp.ToLower().EndsWith(".epub") -> Some IsEpub
        | _ -> None
    
    let (|IsAnyHtml|_|) = 
        function 
        | (_, "") -> None
        | (AnyHtml, fp) when isMatch "^.*\.(html|xhtml)$" (fp.ToLower()) -> Some IsAnyHtml
        | _ -> None
    
    let toFilePath (fileExt : Extension) (filePath : string) : FilePath option = 
        match (fileExt, filePath) with
        | _ when filePath |> (System.IO.File.Exists >> not) -> None
        | IsEpub -> EpubFilePath filePath |> Some
        | IsAnyHtml -> AnyHtmlFilePath filePath |> Some
        | _ -> None
    
    let getFileName = Path.GetFileNameWithoutExtension
    
    let unpackBook (bookPath : FilePath) (savePath : PackDirPath) : UnpackedPath option = 
        match (bookPath, savePath) with
        | (EpubFilePath fp, PackDirPath pdp) -> 
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
