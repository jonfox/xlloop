namespace Trafigura.XLLoop

open System
open System.Reflection

open XLOperOps

type InstanceMethod(instance: obj, methodBase: MethodBase) =
    do
        if not(methodBase.DeclaringType = instance.GetType()) then
            raise (ArgumentException("Method not declared on object instance"))

    let argNames = [ for p in methodBase.GetParameters() -> p.Name ]
    let argTypes = [| for p in methodBase.GetParameters() -> p.ParameterType |]

    let execute context args =
        let objArgs = Array.zip args argTypes |> Array.map (fun (arg, argType) -> fromXLOper argType arg)
        try
            methodBase.Invoke(instance, objArgs)
        with
            | :? ArgumentException as ex -> raise (RequestException(ex.Message))
            | :? MethodAccessException as ex -> raise (RequestException(ex.Message))
            | :? TargetException as ex -> raise (RequestException(ex.InnerException.Message))
            | :? TargetInvocationException as ex -> raise (RequestException(ex.InnerException.Message))
            | :? InvalidOperationException as ex -> raise (RequestException(ex.Message))
            | _ -> raise (RequestException("#Error invoking method " + methodBase.Name))

    let calcArgMatchPercent (arg: XLOper)(t: Type) =
        match arg.Type with
        | XLOperType.xlTypeBool -> if t = typeof<bool> then 100.0 elif t.IsAssignableFrom(typeof<bool>) then 50.0 else 0.0
        | XLOperType.xlTypeInt -> if t = typeof<int> then 100.0 elif t.IsAssignableFrom(typeof<int>) then 50.0 else 0.0
        | XLOperType.xlTypeNum -> if t = typeof<double> then 100.0 elif t.IsAssignableFrom(typeof<double>) || t = typeof<single> || t = typeof<int> || t = typeof<int64> then 50.0 else 0.0
        | XLOperType.xlTypeStr -> if t = typeof<string> then 100.0 elif t.IsAssignableFrom(typeof<string>) then 50.0 else 0.0
        | _ -> 0.0

    let calcMatchPercent (args: XLOper array)(lastArg: int) =
        if lastArg < argTypes.Length then
            0.0
        else
            let parametersMatch = Array.zip (Array.sub args 0 argTypes.Length) argTypes |> Array.map (fun (a, t) -> calcArgMatchPercent a t) |> Array.average
            parametersMatch * (if lastArg = argTypes.Length then 1.0 else 0.50) // Reward exact length match


    member this.ParameterNames = argNames
    member this.ParameterTypeNames = [ for t in argTypes -> t.Name ]
    member this.Type = instance.GetType()
    member this.CalcMatchPercent = calcMatchPercent

    interface IFunction with
        member this.Name = if methodBase.Name.StartsWith("get_") then methodBase.Name.Substring(4) else methodBase.Name
        member this.Execute context args = Some(execute context args |> toXLOper)

    static member GetInstanceMethods instance =
        [ for m in instance.GetType().GetMethods() do if m.IsPublic && not(m.IsStatic) && m.DeclaringType = instance.GetType() then yield InstanceMethod(instance, m) ]
