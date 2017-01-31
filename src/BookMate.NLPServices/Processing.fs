namespace BookMate.NLPServices

module Processing = 
    open System
    open System.Collections.Concurrent
    open Domain
    
    let processedEntries = ConcurrentDictionary<Guid, TaggedEntry>()
    
    let ProcessTagEntryWithDefaults entry = 
        let newEntry = 
            { Uuid = Guid.NewGuid()
              Timestamp = DateTimeOffset.UtcNow
              Text = entry.Text }
        newEntry
    
    let handleTagEnry (tagEntry : ProcessTagEntry) (tagText:TextTagger) = 
        async { 
            let taggedWords = tagEntry.Text |> tagText 
            let wordsAndTags = taggedWords |> Array.map (fun (w,t) -> {Word = w; Tag = t})
            return { Uuid = tagEntry.Uuid
                     Tagged = wordsAndTags }
        }
    
    let addToStore taggedEntry = 
        async { 
            let success = processedEntries.TryAdd(taggedEntry.Uuid, taggedEntry)
            if success then ()
            else printfn "Failed to add %A" taggedEntry.Uuid
            return success
        }
    
    let getProcessedTextFromStore (uuid : Guid) = 
        async { 
            match processedEntries.TryGetValue(uuid) with
            | (false, _) -> return None
            | (_, value) -> return Some value
        }
