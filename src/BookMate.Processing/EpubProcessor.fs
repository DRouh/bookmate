namespace BookMate.Processing

module EpubProcessor =
    
    open BookMate.Processing.Epub
    open BookMate.Processing.EpubIO

    // type BookToProcess = {Files: seq<EpubFile>; Location: PackDirPath}
    // and EpubFile = { name: string; path: EpubFilePath; content: string}