namespace BookMate.Web.Controllers

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Mvc

type WeatherForecast =  {DateFormatted: string; TemperatureC : int; Summary: string}

type HomeController() =
    inherit Controller()
    member __.Index() : IActionResult = __.View("Index") :> IActionResult