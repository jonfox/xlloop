namespace Trafigura.XLLoop

open XLOperOps

type OverloadedMethod(methods: InstanceMethod list) =

    let firstMethod = methods.Head

    let execute context (args: XLOper array) =
        let validIndex = args |> Array.rev |> Array.findIndex (fun a -> not(a.Type = XLOperType.xlTypeNil || a.Type = XLOperType.xlTypeMissing)) 
        let lastArg = args.Length - validIndex
        let bestMethod = methods |> List.maxBy (fun m -> m.CalcMatchPercent args lastArg)
        (bestMethod :> IFunction).Execute context args


    member this.FirstMethod = firstMethod
    member this.AddMethod (im: InstanceMethod) = OverloadedMethod(im :: methods)

    interface IFunction with
        member this.Name = (firstMethod :> IFunction).Name
        member this.Execute context args = execute context args

