namespace BookMate.Integration
module DBHelper = 
    open FSharp.Data.Sql
    //put here a path to System.Data.SQLite.Core
    let [<Literal>] private ResolutionPath = @"d:\coding\bookmate\bookmate\packages\System.Data.SQLite.Core.1.0.103\" 
    //put here a path to translations.sqlite
    let [<Literal>] private ExConnectionString = @"Data Source=d:\coding\bookmate\database\\translations.sqlite;Version=3;Read Only=false;FailIfMissing=True;"
 
//   let private connectionString = BookMate.Core.Configuration.getDBConnectionString
    type SQL = SqlDataProvider<ConnectionString = ExConnectionString, DatabaseVendor = Common.DatabaseProviderTypes.SQLITE, ResolutionPath = ResolutionPath, IndividualsAmount = 1000>
    let loadFromDB = 
        printfn "Loading stored dictionary..." 
        let ctx = SQL.GetDataContext()
        let query1 =
          query {
            for row in ctx.Main.Dictionary do
            select (row.Eng, row.PoS, row.Ru)//sqlite type provder will not be able to discover string columns with length 
          } |> Seq.toArray
        query1

