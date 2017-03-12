namespace BookMate.Database

module Mongo = 
  open BookMate.Core.Configuration
  open MongoDB.Driver
  open MongoDB.Bson

  [<CLIMutable>]
  type TranslationDto = 
      { Id : ObjectId
        Original : string
        Translations : (string * string) array }

  let private initializeClient (connectionString : string) = MongoClient(connectionString)
  let private client = lazy (initializeClient (getTranslationsConnectionString()))
  let private database = lazy (client.Value.GetDatabase("translations"))
  let private getDatabase() = database.Value
  let private getCollection<'a> (db : IMongoDatabase) (collectionName : string) = db.GetCollection<'a>(collectionName)

  let private getTranslationCollection (direction : string) = 
      let db = getDatabase()
      getCollection<TranslationDto> db direction

  let addTranslation (item : TranslationDto) = 
      let collection = getTranslationCollection "en-ru"
      collection.InsertOne(item)

  let addTranslationAsync (item : TranslationDto) = 
      async { 
          let collection = getTranslationCollection "en-ru"
          return! (collection.InsertOneAsync(item) |> Async.AwaitTask)
      }

  let findAllTranslationsFor (originalWord : string) = 
      let collection = getTranslationCollection "en-ru"
      collection.Find(fun x -> x.Original = originalWord).ToList() |> List.ofSeq

  let findAsync (originalWord : string) = 
      async { 
          let collection = getTranslationCollection "en-ru"
          let! task = collection.Find(fun x -> x.Original = originalWord).ToListAsync() |> Async.AwaitTask
          return task |> List.ofSeq
      }