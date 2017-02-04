open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

[<EntryPoint>]
let main argv = 
    let cwd = Directory.GetCurrentDirectory();
    let web = if Path.GetFileName(cwd) = "server" then "../public" else "public"

    do WebHostBuilder()
        .UseContentRoot(cwd)
        .UseWebRoot(web)
        .UseKestrel()
        .UseUrls("http://localhost:6001")
        .UseIISIntegration()
        .UseStartup<BookMate.Web.Startup>()
        .Build()
        .Run()
    0