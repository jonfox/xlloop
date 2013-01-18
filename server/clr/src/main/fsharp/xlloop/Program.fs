namespace Trafigura.XLLoop

open System

type MyClassInt(i: int, j: int) =
    member this.Sum = i + j
    member this.Difference = i - j
    member this.Product = i * j

type MyClassString(s1: string, s2: string) =
    member this.Concat = s1 + s2
    member this.StringIntBool (s: string)(i: int)(b: bool) = String.Format("{0}:{1}:{2}", s, i, b)

module Main =

    [<EntryPoint>]
    let main args =
        let myClassInt = MyClassInt(1, 2)
        let myClassString = MyClassString("foo", "bar")

        let rfh = ReflectFunctionHandler("MyClassInt.", myClassInt).AddInstanceMethods("MyClassString.", myClassString)
        let fs = FunctionServer(5455, rfh)
        fs.Start()
        printfn "Started server...\nPress enter to terminate"
        Console.Read() |> ignore
        0