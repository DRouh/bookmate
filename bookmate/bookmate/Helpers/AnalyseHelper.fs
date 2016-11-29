namespace BookMate.Helpers
module AnalyseHelper = 
    open StringHelper
    open RegexHelper

    let getWordStats text = 
        printfn "Calculating statistics..."
        text
        |> split [| ' ' |]
        |> Array.Parallel.map (sanitize)
        |> Array.filter (isEmpty >> not)
        |> Array.Parallel.map (toLower)
        |> Array.filter (fun x -> 
                match wordFilter x with
                | Some x -> true
                | _ -> false)
        |> Array.groupBy (id)
        |> Array.Parallel.map (fun (key, values) -> (key, values.Length))
        |> Array.sortByDescending (fun (k, v) -> v)