namespace BookMate.NLPServices

module Program = 
    open Suave
    open Rest
    open System
    open System.Reactive
    open Processing
    open Domain
    open ApiConfiguration
    
    type Agent<'T> = Microsoft.FSharp.Control.MailboxProcessor<'T>
    
    let addTextToProcessing observer nextItem = 
        use subject = new Subjects.Subject<ProcessTagEntry>()
        use o = subject.Subscribe observer
        subject.OnNext nextItem
        nextItem
    
    let getProcessedTextFromStore = Processing.getProcessedTextFromStore >> Async.RunSynchronously
    
    [<EntryPoint>]
    let main argv = 
        let tagActor = 
            new Agent<ProcessTagEntry>(fun inbox -> 
            let rec loop() = async { let! cmd = inbox.Receive()
                                     let! entry = handleTagEnry cmd
                                     let! _ = addToStore entry
                                     return! loop() }
            loop())
        do tagActor.Start()
        let addTextToProcessing' = Observer.Create(Action<ProcessTagEntry> tagActor.Post) |> addTextToProcessing
        
        let taggerApi = 
            { Create = (ProcessTagEntryWithDefaults >> addTextToProcessing')
              GetByGuid = getProcessedTextFromStore
              Exists = getProcessedTextFromStore >> Option.isSome }
        
        let taggerApiEndpoint = "api/tagger/process"
        let taggerProcessQueueWebPart = rest taggerApiEndpoint taggerApi
        startWebServer defaultConfig (choose [ taggerProcessQueueWebPart ])
        0
