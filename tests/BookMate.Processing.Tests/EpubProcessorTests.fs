namespace BookMate.Processing.Tests

module EpubProcessorTests = 
    open Xunit
    open FsUnit.Xunit
    open BookMate.Processing.EpubIO
    open BookMate.Processing.EpubProcessor
    (*  To do items
        1. Read file
            1.1. Unpack file
            1.2. Read file into memory
        2. Process file
        3. Save file  
    *)

    [<Fact>]
    let ``1 = 1``() = 
        1 = 1 |> should be True
    