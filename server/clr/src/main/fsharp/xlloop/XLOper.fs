namespace Trafigura.XLLoop

open System
open System.Text

module XLSRefOps =
    let toColumnName column =
        let t0 = column % 26
        let t1 = column / 26
        let t2 = column / 676
        let secondPrefix = if t2 > 0 then  char(64 + t2).ToString() else ""
        let firstPrefix = if t1 > 0 then char(64 + t1).ToString() else ""
        secondPrefix + firstPrefix + char(65 + t0).ToString()

type XLOperType =
    | xlTypeNum     = 1
    | xlTypeStr     = 2
    | xlTypeBool    = 3
    | xlTypeErr     = 4
    | xlTypeMulti   = 5
    | xlTypeMissing = 6
    | xlTypeNil     = 7
    | xlTypeInt     = 8
    | xlTypeSRef    = 9

type XLErrorType =
    | NULL  = 0
    | DIV0  = 7
    | VALUE = 15
    | REF   = 23
    | NAME  = 29
    | NUM   = 36
    | NA    = 42

type XLSRefType = { ColFirst: int; ColLast: int; RowFirst: int; RowLast: int }

type XLArrayType = { Rows: int; Columns: int; Data: XLOper array }
and XLOper =
    | XLNil
    | XLMissing
    | XLError of XLErrorType
    | XLBool of bool
    | XLInt of int
    | XLNum of double
    | XLString of string
    | XLArray of XLArrayType
    | XLSRef of XLSRefType
    member this.Type = match this with
        | XLNum _    -> XLOperType.xlTypeNum
        | XLString _ -> XLOperType.xlTypeStr
        | XLBool _   -> XLOperType.xlTypeBool
        | XLError _  -> XLOperType.xlTypeErr
        | XLArray _  -> XLOperType.xlTypeMulti
        | XLMissing  -> XLOperType.xlTypeMissing
        | XLNil      -> XLOperType.xlTypeNil
        | XLInt _    -> XLOperType.xlTypeInt
        | XLSRef _   -> XLOperType.xlTypeSRef
    override this.ToString() = match this with
        | XLNil -> ""
        | XLMissing -> ""
        | XLError e -> e.ToString()
        | XLBool b -> b.ToString()
        | XLInt i -> i.ToString()
        | XLNum d -> d.ToString()
        | XLString s -> "\"" + String255(s).ToString() + "\""
        | XLArray value ->
            let (rows, columns, arr) = (value.Rows, value.Columns, value.Data)
            let rowString i = ([(i * columns + 1) .. ((i + 1) * columns - 1)] |> List.fold (fun acc j -> acc + ", " + arr.[j].ToString()) ("[" + arr.[i * columns].ToString())) + "]"
            [0 .. (rows - 1)] |> List.fold (fun acc i -> acc + (rowString i) + ",\n") ""
        | XLSRef value ->
            let cellRef col row = (XLSRefOps.toColumnName col) + (row + 1).ToString()
            if value.RowLast - value.RowFirst > 0 || value.ColLast - value.ColFirst > 0 then
                cellRef (value.ColFirst) (value.RowFirst) + ":" + cellRef (value.ColLast)(value.RowLast)
            else
                cellRef (value.ColFirst) (value.RowFirst)


module XLOperOps =
    // obj -> XLOper
    let toXLOperBase (value: obj) =
        match value with
        | null -> XLNil
        | :? XLOper as op -> op
        | :? string as s -> XLString(s)
        | :? int as i -> XLInt(i)
        | :? single as s -> XLNum(double(s))
        | :? double as d -> XLNum(d)
        | :? bool as b -> XLBool(b)
        | :? DateTime as dt -> XLNum(dt.ToOADate())
        // TODO | :? obj as o -> support object registry
        | _ -> failwith "Unsupported base value type"

    let toXLOperList (f: 'a -> XLOper)(value: 'a list) = XLArray({ Rows = value.Length; Columns = 1; Data = value |> List.map (fun a -> f a) |> Array.ofList })

    let toXLOperTupleList (f: 'a -> XLOper)(value: ('a * 'a) list) = XLArray({ Rows = value.Length ; Columns = 2; Data = value |> List.map (fun t -> [f (fst t); f (snd t)]) |> List.concat |> List.toArray })

    let toXLOperArray (f: 'a -> XLOper)(value: 'a array) = XLArray({ Rows = value.Length; Columns = 1; Data = value |> Array.map (fun a -> f a) })

    let toXLOper2DArray (f: 'a -> XLOper)(value: 'a[,]) = XLArray({ Rows = value |> Array2D.length1; Columns = value |> Array2D.length2; Data = value |> Array.flatten |>  Array.map (fun a -> f a) })

    let toXLOper2DJaggedArray (f: 'a -> XLOper)(value: 'a array array) =
        if (value.Length = 0 || value.[0].Length = 0) then
            XLError(XLErrorType.NA)
        else
            XLArray({ Rows = value.Length; Columns = value.[0].Length; Data = value |> Array.map (fun r -> r |> Array.map (fun a -> f a)) |> Array.flattenJagged XLNil })
      
    let toXLOper (value: obj) =  
        match value with
        | :? (XLOper list) as l                 -> toXLOperList id l
        | :? (XLOper array) as arr              -> toXLOperArray id arr
        | :? (XLOper[,]) as arr                 -> toXLOper2DArray id arr
        | :? (XLOper[][]) as arr                -> toXLOper2DJaggedArray id arr
        | :? (bool list) as l                   -> toXLOperList toXLOperBase l
        | :? (bool array) as arr                -> toXLOperArray toXLOperBase arr
        | :? (bool[,]) as arr                   -> toXLOper2DArray toXLOperBase arr
        | :? (bool[][]) as arr                  -> toXLOper2DJaggedArray toXLOperBase arr
        | :? (int list) as l                    -> toXLOperList toXLOperBase l
        | :? (int array) as arr                 -> toXLOperArray toXLOperBase arr
        | :? (int[,]) as arr                    -> toXLOper2DArray toXLOperBase arr
        | :? (int[][]) as arr                   -> toXLOper2DJaggedArray toXLOperBase arr
        | :? (double list) as l                 -> toXLOperList toXLOperBase l
        | :? ((double * double) list) as l      -> toXLOperTupleList toXLOperBase l
        | :? ((DateTime * double) list) as l    -> toXLOperTupleList toXLOperBase (l |> List.map (fun (dt, d) -> (dt.ToOADate(), d)))
        | :? (double array) as arr              -> toXLOperArray toXLOperBase arr
        | :? (double[,]) as arr                 -> toXLOper2DArray toXLOperBase arr
        | :? (double[][]) as arr                -> toXLOper2DJaggedArray toXLOperBase arr
        | :? (string list) as l                 -> toXLOperList toXLOperBase l
        | :? (string array) as arr              -> toXLOperArray toXLOperBase arr
        | :? (string[,]) as arr                 -> toXLOper2DArray toXLOperBase arr
        | :? (string[][]) as arr                -> toXLOper2DJaggedArray toXLOperBase arr
        | _                                     -> toXLOperBase value


    // XLOper -> obj
    open Types

    let emptyType (hint: Type) =
        match hint.FullName with
        | "System.Int32" -> box 0
        | "System.Int64" -> box 0L
        | "System.Double" -> box 0.0
        | "System.Boolean" -> box false
        | "System.String" -> box ""
        | _ -> null

    let fromXLOperToBool (op: XLOper): bool =
        match op with
        | XLBool(b) -> b
        | XLInt(i) -> not(i = 0)
        | XLNum(d) -> not(d = 0.0)
        | _ -> failwith "Can only convert numeric types to bool"

    let fromXLOperToInt (op: XLOper): int =
        match op with
        | XLBool(b) -> if b then 1 else 0
        | XLInt(i) -> i
        | XLNum(d) -> int(d)
        | _ -> failwith "Can only convert numeric types to int"

    let fromXLOperToInt64 (op: XLOper): int64 =
        match op with
        | XLBool(b) -> if b then 1L else 0L
        | XLInt(i) -> int64(i)
        | XLNum(d) -> int64(d)
        | _ -> failwith "Can only convert numeric types to int64"

    let fromXLOperToDouble (op: XLOper): double =
        match op with
        | XLBool(b) -> if b then 1.0 else 0.0
        | XLInt(i) -> double(i)
        | XLNum(d) -> d
        | _ -> failwith "Can only convert numeric types to double"
        
    let fromXLOperToString (op: XLOper): String =
        match op with
        | XLString(s) -> s
        | XLBool(b) -> b.ToString()
        | XLInt(i) -> i.ToString()
        | XLNum(d) -> d.ToString()
        | _ -> failwith "Cannot parse value to string"

    let xlArrayToArray2D (xlArray: XLArrayType) =
        array2D [ for i in 0 .. xlArray.Rows - 1 -> [ for j in 0 .. xlArray.Columns - 1 -> xlArray.Data.[i * xlArray.Columns + j] ] ]

    let array2DToObj (collectionType: CollectionType)(arr: 'a[,]) =
        match collectionType with
        | Array1D -> arr |> Array.flatten :> obj
        | Array2D -> arr :> obj
        | Array2DJagged -> arr |> Array.array2DToJagged :> obj
        | FSharpList -> arr |> Array.flatten |> List.ofArray :> obj
        | Unknown -> if Array2D.length1 arr = 1 || Array2D.length2 arr = 1 then arr |> Array.flatten :> obj else arr :> obj

    let rec fromXLOperBase (hint: Type)(op: XLOper) =
        if hint = typeof<XLOper> then
            op :> obj
        else
            match op with
            | XLNil -> emptyType hint
            | XLMissing -> emptyType hint
            | XLError(_) -> null
            | XLSRef(_) -> null
            | XLString(s) -> box s
            | XLBool(b) ->
                match hint.FullName with
                | "System.Int32"  -> box (if b then 1 else 0)
                | "System.Int64"  -> box (if b then 1L else 0L)
                | "System.Double" -> box(if b then 1.0 else 0.0)
                | _ -> box b
            | XLInt(i) -> 
                match hint.FullName with
                | "System.Boolean" -> box(not(i = 0))
                | "System.Int64"   -> box(int64(i))
                | "System.Double"  -> box(double(i))
                | _ -> box i
            | XLNum(d) ->
                match hint.FullName with
                | "System.Boolean" -> box(not(d = 0.0))
                | "System.Int32"   -> box(int(d))
                | "System.Int64"   -> box(int64(d))
                | _ -> box d
            | XLArray(arr) ->
                if arr.Rows = 0 || arr.Columns = 0 then
                    null
                else
                    let (collectionType, elementType) = Types.decomposeCollectionType hint
                    let nativeArray = xlArrayToArray2D arr 
                    match elementType.FullName with
                    | "System.Boolean" -> nativeArray |> Array2D.map (fun el -> fromXLOperToBool el) |> array2DToObj collectionType
                    | "System.Int32"   -> nativeArray |> Array2D.map (fun el -> fromXLOperToInt el) |> array2DToObj collectionType
                    | "System.Int64"   -> nativeArray |> Array2D.map (fun el -> fromXLOperToInt64 el) |> array2DToObj collectionType
                    | "System.Double"  -> nativeArray |> Array2D.map (fun el -> fromXLOperToDouble el) |> array2DToObj collectionType
                    | "System.String"  -> nativeArray |> Array2D.map (fun el -> fromXLOperToString el) |> array2DToObj collectionType
                    | _                -> nativeArray |> Array2D.map (fun el -> fromXLOperBase elementType el) |> array2DToObj collectionType
                       
    let fromXLOper (hint: Type)(op: XLOper) =
        let wrappedOp =
            if not(op.Type = XLOperType.xlTypeMulti) && hint.IsArray then
                XLArray({ Rows = 1; Columns = 1; Data = [| op |] })
            else
                op

        fromXLOperBase hint wrappedOp

            

