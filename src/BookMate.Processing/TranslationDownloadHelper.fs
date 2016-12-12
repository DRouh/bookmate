namespace BookMate.Processing
module TranslationDownloadHelper =
    open BookMate.Core.Helpers
    open BookMate.Integration

    let downloadSingleTranslations (toQuery : string []) = 
        let chunkSize = 30
        //progress update
        let progress = (float32 toQuery.Length) / 30.0f
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let splitByChuncks = 
            Array.chunkBySize chunkSize
            >> Array.map (Array.reduce StringHelper.stringify)
            >> Array.ofSeq
        
        let asyncParalleldownload = 
            Array.Parallel.mapi 
                (fun i p -> (p, (YandexHelper.askYaTranslateAsyncf p updateDownloadProgress) |> Async.RunSynchronously))
        
        let form (arg : (string * string []) []) = 
            arg
            |> Array.Parallel.map (fun (p, a) -> (StringHelper.unstringify p, a))
            |> Array.Parallel.map (fun (p, a) -> (p, a, p.Length = a.Length))
            |> Array.Parallel.map (fun (p, a, se) -> 
                   if (se) then (Seq.zip p a)
                   else Seq.empty<string * string>)
            |> Seq.concat
            |> Array.ofSeq
        printfn ""
        toQuery
        |> splitByChuncks
        |> asyncParalleldownload
        |> form
   
    let downloadDictionaryTranslations (toQuery : string []) = 
        //progress update
        printfn ""
        let progress = (float32 toQuery.Length)
        let mutable a = 0
        
        let updateDownloadProgress() = 
            a <- a + 1
            printf "\rDownloading dictionary translations:%i%%" (int (100.0f * (float32 a / progress)))
        
        let asyncParalleldownload = 
            Array.Parallel.map (fun p -> (p, (YandexHelper.askYaDictionaryAsyncf p updateDownloadProgress) |> Async.RunSynchronously))
        
        let oneToMany (ys : (string*string)[]) (x : string) = 
            x
            |> Array.replicate ys.Length
            |> Array.zip ys

        printfn ""
        toQuery
        |> asyncParalleldownload
        |> Array.Parallel.map (fun (x, y) -> oneToMany y x)
        |> Array.concat
        |> Array.Parallel.map (fun ((pos, ru), eng) -> (eng, pos, ru))