namespace Trafigura.XLLoop

open System

module Option =
    let (|?) value defaultValue = defaultArg value defaultValue

    let ofNullable value = if value = null then None else Some value

    let t1: int[,] = array2D [[1; 2]; [3; 4]]
    let t2: int[][] = [|[|1; 2|]; [|3; 4|]|]

module List =
    let inline flatten (ll: ^a list list) = [ for l in ll do for a in l -> a ]

module Array =
    let array2DToJagged (arr: 'a[,]) = [| for i in 0 .. (arr |> Array2D.length1) - 1 -> [| for j in 0 .. (arr |> Array2D.length2) - 1 -> arr.[i, j] |] |]
        
    let flatten (arr: 'a[,]) = [| for i in 0 .. (arr |> Array2D.length1) - 1 do for j in 0 .. (arr |> Array2D.length2) - 1 -> arr.[i, j] |]

    let flattenJagged (defaultValue: 'a)(arr: 'a array array) =
        let length1 = arr.Length
        if length1 = 0 then
            Array.empty<'a>
        else
            let length2 = arr.[0].Length
            if length2 = 0 then
                Array.empty<'a>
            else
                [| for i in 0 .. length1 - 1 do for j in 0 .. length2 - 1-> if (j < arr.[i].Length) then arr.[i].[j] else defaultValue |]

module Map =
    let createMap (f: 'a -> 'b)(s: 'a seq) = s |> Seq.map (fun a -> (f a, a)) |> Map.ofSeq

module CSV =
    let toCSV (l: 'a list) = l |> List.fold (fun acc a -> acc + "," + a.ToString()) ""

module Types =
    type CollectionType =
        | Unknown
        | Array1D
        | Array2D
        | Array2DJagged
        | FSharpList

    let (|TypeSuffix|_|) (p: string)(s: string) =
        let index = s.IndexOf(p)
        if index = -1 then
            None
        else
            let typeName = s.Substring(0, index)
            Some(Type.GetType(typeName))

    let (|Prefix|_|) (p: string)(s: string) =
        if s.StartsWith(p) then
            Some(s.Substring(p.Length))
        else
            None

    let decomposeCollectionType (t: Type) =
        match t.FullName with
        | TypeSuffix "[][]" elType -> (Array2DJagged, elType)
        | TypeSuffix "[,]" elType -> (Array2D, elType)
        | TypeSuffix "[]" elType -> (Array1D, elType)
        | Prefix "Microsoft.FSharp.Collections.FSharpList`1" _ -> (FSharpList, t.GenericTypeArguments.[0])
        | _ -> (Unknown, typeof<obj>)

type String255(str: string) =
    let internalString =
        if str = null then
            ""
        else
            if str.Length > 255 then
                str.Substring(0, 255)
            else
                str

    override this.ToString() = internalString


