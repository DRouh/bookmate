namespace BookMate.Core.Helpers
module RegexHelper =
    open System.Text.RegularExpressions

    let (|Regex|_|) pattern input = 
        let m = Regex.Match(input, pattern)
        if (m.Success) then Some m.Groups.[1].Value
        else None
    
    let wordFilter str = 
        match str with
        | Regex @"^[a-z']*$" v -> Some str
        | _ -> None
    
    let isMatch pattern str =
        match str with
        | Regex pattern a -> true
        | _ -> false

    let regexReplace (input:string) (pattern:string) (replacement:string) = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase)