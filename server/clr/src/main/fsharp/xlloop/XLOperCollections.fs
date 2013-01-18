namespace Trafigura.XLLoop

module XLKeyValue =
    open Option
    open XLOperOps

    let createPair (key: string)(value: obj) = (key, toXLOper value)

    let ofList (kvs: (string * obj) list) = kvs |> List.map (fun (k, v) -> (k, toXLOper v))

    let tryCons (key: string)(value: obj option)(l: (string * XLOper) list) = (value |> Option.map (fun v -> [(key, toXLOper v)]) |? []) @ l

    let toXLOper (l: (string * XLOper) list) =
        let arr = l |> List.rev |> List.fold (fun acc (k, v) -> XLString(k) :: v :: acc) [] |> List.toArray
        XLArray({ Rows = arr.Length / 2; Columns = 2; Data = arr })

    let ofXLOper (op: XLArrayType) =
        if not(op.Columns = 2) then failwith "Invalid XLArray dimensions for map, need 2 columns"
        [ for row in 0 .. op.Rows - 1 ->
            let key = match op.Data.[row * 2] with
                | XLString s -> s
                | _ -> failwith "Invalid key type"
            (key, op.Data.[row * 2 + 1])]
