namespace BookMate.Processing

module EpubProcessor = 
    open System
    open System.IO
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO
    
    type BookToProcess = 
        { Files : seq<EpubFile>
          Location : UnpackedPath }
    
    and EpubFile = 
        { name : string
          path : EpubFilePath
          content : string }
    
    let readBook (unpackedBook : UnpackedPath) : BookToProcess = 
        let UnpackedPath (EpubFilePath filePath, UnpackedDirPath dirPath) = unpackedBook
        { Files = 
              [ { name = ""
                  path = EpubFilePath ""
                  content = "" } ]
          Location = unpackedBook }
    //let contentFiles = Directory.GetFiles(dirPath, "*.*html", System.IO.SearchOption.AllDirectories)
    //{Files = [{name= ""; path = x; content = ""}]}
