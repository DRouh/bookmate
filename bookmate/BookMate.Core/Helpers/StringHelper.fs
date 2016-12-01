namespace BookMate.Core.Helpers
module StringHelper =
    let split separators (x : string) = x.Split(separators)
    let toLower (x : string) = x.ToLowerInvariant()
    let replace (oldValue : string) (newValue : string) (value : string) = value.Replace(oldValue, newValue)
    let startsWith (value : string) (x : string) = x.StartsWith(value)
    let endsWith (value : string) (x : string) = x.EndsWith(value)
    let substringStart (idx : int) (s : string) = s.Substring(idx)
    let stringify = fun acc elem -> acc + ", " + elem
    let unstringify = split [| ',' |]

    let substringEnd (endIdx : int) (s : string) = 
        if endIdx <= s.Length then s.Substring(0, endIdx)
        else s
    
    let validWordStartingWithApostrophe = [ "'em" ]
    
    let rec removeStartingApostrophe (x : string) = 
        if (startsWith "'" x) then 
            if (List.contains x validWordStartingWithApostrophe) then x
            else removeStartingApostrophe (substringStart 1 x)
        else x
    
    //this one is english specific
    let rec removeNonPossesiveApostrophe x = 
        if (endsWith "'" x) then 
            if (endsWith "s'" x) then x
            else removeNonPossesiveApostrophe (substringEnd (x.Length - 1) x)
        else x
    
    let isEmpty = 
        function 
        | "" -> true
        | _ -> false
    
    let sanitize = 
        removeStartingApostrophe
        >> removeNonPossesiveApostrophe
        >> replace " " ""
        >> replace "." ""
        >> replace "-" ""
        >> replace "\r" ""
        >> replace "\n" ""
        >> (fun x -> 
        if x = "'" then ""
        else x)