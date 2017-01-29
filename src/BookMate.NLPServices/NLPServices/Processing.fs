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
    
    let handleTagEnry (tagEntry : ProcessTagEntry) = 
        async { 
            printfn "Received:%A." tagEntry.Uuid
            do! Async.Sleep(2000)
            printfn "Processed:%A." tagEntry.Uuid
            return { Uuid = tagEntry.Uuid
                     Tagged = "Many tags" }
        }
    
    let addToStore taggedEntry = 
        async { 
            let success = processedEntries.TryAdd(taggedEntry.Uuid, taggedEntry)
            if success then printfn "Successfully added %A" taggedEntry.Uuid
            else printfn "Failed to add %A" taggedEntry.Uuid
            return success
        }
    
    let getProcessedTextFromStore (uuid : Guid) = 
        async { 
            match processedEntries.TryGetValue(uuid) with
            | (false, _) -> return None
            | (_, value) -> return Some value
        }
