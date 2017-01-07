namespace BookMate.Web

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Routing
open System.Web.Optimization
open System.Web.Mvc

type BundleConfig() = 
    static member RegisterBundles(bundles : BundleCollection) = 
        bundles.Add(ScriptBundle("~/bundles/angular-vendor").Include([| "~/app_built/vendor.bundle.js" |]))
        bundles.Add(ScriptBundle("~/bundles/angular-app").Include([| "~/app_built/bundle.js" |]))

/// Route for ASP.NET MVC applications
type Route = 
    { controller : string
      action : string
      id : UrlParameter }

type HttpRoute = 
    { controller : string
      id : RouteParameter }

type Global() = 
    inherit System.Web.HttpApplication()
    
    static member RegisterWebApi(config : HttpConfiguration) = 
        // Configure routing
        config.MapHttpAttributeRoutes()
        config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", 
                                   { controller = "{controller}"
                                     id = RouteParameter.Optional })
        |> ignore
        // Configure serialization
        config.Formatters.XmlFormatter.UseXmlSerializer <- true
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
                                                                                   ()
    
    static member RegisterFilters(filters : GlobalFilterCollection) = filters.Add(new HandleErrorAttribute())
    
    static member RegisterRoutes(routes : RouteCollection) = 
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute("Default", "{*any}", 
                        { controller = "Home"
                          action = "Index"
                          id = UrlParameter.Optional })
        |> ignore
    
    // Additional Web API settings
    member x.Application_Start() = 
        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(Action<_> Global.RegisterWebApi)
        Global.RegisterFilters(GlobalFilters.Filters)
        Global.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles BundleTable.Bundles
