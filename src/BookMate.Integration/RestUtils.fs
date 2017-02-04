namespace BookMate.Integration

module RestUtils = 

    type GetAsync = string -> Async<string>
    type PostAsync = string -> string -> Async<string>

    let ComposeUrl endpoint valuePairs = 
        let query = 
            if Seq.isEmpty valuePairs then ""
            else 
                let values = valuePairs |> Seq.map (fun (k, v) -> sprintf "%s=%s" k v)
                "?" + System.String.Join("&", values)
        endpoint + query
    
    let GetJsonAsync(url : string) = 
        async { 
            let uri = new System.Uri(url, System.UriKind.Absolute)
            let client = new System.Net.Http.HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")
            let! response = client.GetAsync(uri) |> Async.AwaitTask
            let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask

            let code = int response.StatusCode
            return (body, code)
        }

    let PostJsonAsync (url : string) (jsonText : string)= 
        async { 
            let uri = new System.Uri(url, System.UriKind.Absolute)
            let client = new System.Net.Http.HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")
            let content = new System.Net.Http.StringContent(jsonText)
            let! response = client.PostAsync(uri, content) |> Async.AwaitTask
            let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return body
        }
