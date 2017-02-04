namespace BookMate.Core.Monads

module Maybe = 
  let unit value = Some value
  let bind f = function
    | None -> None
    | Some value -> f value

  type MaybeBuilder () =
    member this.Bind(v, f) = bind f v
    member this.Return(v) = unit v
    member this.ReturnFrom(m) = m

  
  let maybe = MaybeBuilder()