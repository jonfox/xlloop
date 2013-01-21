namespace Trafigura.XLLoop

open System

type MyClassInt(i: int, j: int) =
    member this.Sum = i + j
    member this.Difference = i - j
    member this.Product = i * j

type MyClassString(s1: string, s2: string) =
    member this.Concat = s1 + s2
    member this.StringIntBool (s: string)(i: int)(b: bool) = String.Format("{0}:{1}:{2}", s, i, b)

type MyClassCollections() =
    member this.Sum (arr: double array) = arr |> Array.sum
    member this.Sum2(arr: double[,]) = arr |> Array.array2DToJagged |> Array.map (fun a -> a |> Array.sum) |> Array.sum
    member this.SumJ(arr: double array array) = arr |> Array.map (fun a -> a |> Array.sum) |> Array.sum

module Main =

    [<EntryPoint>]
    let main args =
        let myClassInt = MyClassInt(1, 2)
        let myClassString = MyClassString("foo", "bar")
        let myClassCollections = MyClassCollections()

        let rfh = ReflectFunctionHandler("MyClassInt.", myClassInt).AddInstanceMethods("MyClassString.", myClassString).AddInstanceMethods("MyClassCollections.", myClassCollections)
        let fs = FunctionServer(5455, rfh)
        fs.Start()
        printfn "Started server...\nPress enter to terminate"
        Console.Read() |> ignore
        0