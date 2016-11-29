﻿namespace BookMate.Helpers
module IOHelper = 
    open System.IO
    open System.IO.Compression

    let (+/) path1 path2 = Path.Combine(path1, path2)
    let getDirectoryName path = Path.GetDirectoryName(path)
    let getFileNameWithoutExtension path = Path.GetFileNameWithoutExtension(path)

    let writeToFile (fileName : string) (whatToWrite : string) = 
        let wr = new System.IO.StreamWriter(fileName)
        let text = whatToWrite |> wr.Write
        wr.Close()
    
    ///Deletes files with give pattern.
    let rec deleteFiles srcPath pattern includeSubDirs deleteSubDirs =
        if not <| System.IO.Directory.Exists(srcPath) then
            let msg = System.String.Format("Source directory does not exist or could not be found: {0}", srcPath)
            raise (System.IO.DirectoryNotFoundException(msg))

        for file in System.IO.Directory.EnumerateFiles(srcPath, pattern) do
            let tempPath = System.IO.Path.Combine(srcPath, file)
            System.IO.File.Delete(tempPath)

        if includeSubDirs then
            let srcDir = new System.IO.DirectoryInfo(srcPath)
            for subdir in srcDir.GetDirectories() do
                deleteFiles subdir.FullName pattern includeSubDirs deleteSubDirs
            if deleteSubDirs then
                do Directory.Delete(srcPath)
    
    let unzipFile filePath targetPath = 
        let fileName = getFileNameWithoutExtension filePath
        printfn "Uncompressing %s" fileName
        use archive = System.IO.Compression.ZipFile.Open(filePath, ZipArchiveMode.Read)
        do archive.ExtractToDirectory(targetPath)
    
    let zipFile directoryPath targetPath = 
        printfn "Compressing %s" directoryPath
        do System.IO.Compression.ZipFile.CreateFromDirectory(directoryPath, targetPath)

    let createFolder path = Directory.CreateDirectory(path)
