namespace BookMate.Processing
module TranslationDownloadHelper =
    open BookMate.Core.Helpers
    open BookMate.Integration
    open BookMate.Core.Helpers.StringHelper

    let downloadSingleTranslations (toQuery : string []) = 
        let chunkSize = 30
        //progress update
        let progress = (float32 toQuery.Length) / 30.0f
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let splitByChuncks = Array.chunkBySize chunkSize >> Array.map (Array.reduce stringify)
        let downloadTranslations = 
            Array.Parallel.map (fun w -> 
                let ts = (YandexHelper.askYaTranslateAsyncf <| w <| updateDownloadProgress) |> Async.RunSynchronously
                w, ts)
        
        let flatten (arg : (string * string []) []) = 
            arg
            |> Array.Parallel.choose (fun (p, a) -> 
                   let ss = unstringify <| p
                   match ss.Length = a.Length with
                   | true -> Some(ss |> Array.zip a)
                   | _ -> None)
            |> Array.concat
              
        printfn ""
        toQuery
        |> splitByChuncks
        |> downloadTranslations
        |> flatten
   
    let downloadDictionaryTranslations (toQuery : string []) = 
        //progress update
        printfn ""
        let progress = (float32 toQuery.Length)
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading dictionary translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let downloadTranslations = 
            Array.Parallel.map (fun w -> 
                let ts = (YandexHelper.askYaDictionaryAsyncf <| w <| updateDownloadProgress) |> Async.RunSynchronously
                w, ts)

        let oneToMany (ys : 'a[]) = 
            Array.replicate ys.Length
            >> Array.zip ys

        printfn ""
        [|("","","")|]
        // toQuery
        // |> downloadTranslations
        // |> Array.Parallel.collect (fun (x, y) -> oneToMany y x)
        // |> Array.Parallel.map (fun ((pos, ru), eng) -> (eng, pos, ru))