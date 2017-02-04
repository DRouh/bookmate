namespace BookMate.Web
open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

type Route = 
    { controller : string
      action : string}
type Startup(env: IHostingEnvironment) =
    let configuration = 
        ConfigurationBuilder()
         .SetBasePath(env.ContentRootPath)
         .AddJsonFile("appsettings.json", true)
         .AddJsonFile((sprintf "appsettings%s.json" env.EnvironmentName), true)
         .Build()

    member __.ConfigureServices(container : IServiceCollection) =
        container
         .AddMvcCore() 
         .AddViews()
         .AddRazorViewEngine()
         .AddJsonFormatters()
         |> ignore
    
    member __.ConfigureRoutes(routes: IRouteBuilder) =
       routes.MapRoute("default", "{*url}", { controller = "Home"; action = "Index" }) |> ignore
       ()

    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment)
                        (loggerFactory : ILoggerFactory) =

        loggerFactory.AddConsole(configuration.GetSection("Logging")) |> ignore
        loggerFactory.AddDebug() |> ignore

        //todo set to developments
        if env.IsDevelopment() || env.IsProduction() then
            app.UseDeveloperExceptionPage() |> ignore
        else
            () //todo app.UseExceptionHandler("/Home/Error")
            
        app.UseStaticFiles() |> ignore

        app.UseMvc(System.Action<IRouteBuilder>__.ConfigureRoutes) |> ignore
        