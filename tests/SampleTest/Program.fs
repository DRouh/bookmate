namespace SampleTest
open Xunit
open FsUnit.Xunit

module CommonTest = 
    type Point3D = {
      x: float
      y: float
      z: float  
    }
    let (==) = LanguagePrimitives.PhysicalEquality
    let inline (!=) a b = not (a == b)
    
    let a = {x = 1.0; y = 2.0; z = 3.0}
    let b = {x = 1.0; y = 2.0; z = 3.0}
    let c = a 
    
    [<Fact>]
    let ``== operator should show physical equality from two references``() =
        (a == b) |> should be False
        (a == c) |> should be True
    
    [<Fact>]
    let ``!= operator should show physical inequality of two references``() =
        (a != b) |> should be True
        (a != c) |> should be False