namespace Trafigura.XLLoop

open System
open System.Collections.Generic
open System.Linq
open System.Reflection

open log4net

open XLOperOps

type InstanceMethod(excelNamespace: string, instance: obj, methodBase: MethodBase) =
    do
        if not(methodBase.DeclaringType = instance.GetType()) then
            raise (ArgumentException("Method not declared on object instance"))

    let excelMethodName = excelNamespace + (if methodBase.Name.StartsWith("get_") then methodBase.Name.Substring(4) else methodBase.Name)
    let argNames = [ for p in methodBase.GetParameters() -> p.Name ]
    let argTypes = [| for p in methodBase.GetParameters() -> p.ParameterType |]

    let execute context args =
        let objArgs = Array.zip args argTypes |> Array.map (fun (arg, argType) -> fromXLOper argType arg)
        try
            methodBase.Invoke(instance, objArgs)
        with
            | :? ArgumentException as ex -> raise (RequestException(ex.Message))
            | :? MethodAccessException as ex -> raise (RequestException(ex.Message))
            | :? TargetException as ex -> raise (RequestException(ex.Message))
            | :? TargetInvocationException as ex -> raise (RequestException(ex.Message))
            | :? InvalidOperationException as ex -> raise (RequestException(ex.Message))
            | _ -> raise (RequestException("#Error invoking method " + methodBase.Name))

    member this.ExcelMethodName = excelMethodName
    member this.ParameterNames = argNames
    member this.ParameterTypeNames = [ for t in argTypes -> t.Name ]
    member this.Type = instance.GetType()

    static member GetInstanceMethods excelNamespace instance =
        [ for m in instance.GetType().GetMethods() do if m.IsPublic && m.DeclaringType = instance.GetType() then yield InstanceMethod(excelNamespace, instance, m) ]

    interface IFunction with
        member this.Name = methodBase.Name
        member this.Execute context args = Some(execute context args |> toXLOper)

open Option
// TODO support object registry, and overloaded methods
type ReflectFunctionHandler(methods: Map<string, InstanceMethod>, information: Map<string, FunctionInformation>) =

    let logger = LogManager.GetLogger("xlloop.ReflectFunctionHandler")

    let createFunctionInformation (f: IFunction) =
        match f with
        | :? InstanceMethod as im -> FunctionInformation(f.Name, im.ParameterNames, im.ParameterTypeNames)
        | _ -> FunctionInformation(f.Name)
    
    let getFunctions = methods |> Map.toList |> List.map (fun (name, im) -> information.TryFind name |? createFunctionInformation im)
    
    let execute context name args =

        let argsString = String.concat ", " (args |> Array.map(fun a -> a.ToString()))
        logger.InfoFormat(name + " (" + argsString + ")")

        let f = methods.TryFind name
        if f.IsNone then
            raise (RequestException("#Unkown method {0}" + name))
        else
            (f.Value :> IFunction).Execute context args
        
    new() = ReflectFunctionHandler(Map.empty<string, InstanceMethod>, Map.empty<string, FunctionInformation>)
    new(excelNamespace, instance) =
        let methods = InstanceMethod.GetInstanceMethods excelNamespace instance |> Map.createMap (fun im -> im.ExcelMethodName)
        ReflectFunctionHandler(methods, Map.empty<string, FunctionInformation>)

    member this.AddInstanceMethods(excelNamespace, instance) =
        let methods = InstanceMethod.GetInstanceMethods excelNamespace instance |> List.fold (fun acc im -> acc |> Map.add (im.ExcelMethodName) im) methods
        ReflectFunctionHandler(methods, Map.empty<string, FunctionInformation>)

    interface IFunctionHandler with
        member this.HasFunction name = methods.ContainsKey name
        member this.Execute context name args = execute context name args

    interface IFunctionProvider with
        member this.GetFunctions = getFunctions
        member this.GetHandler = this :> IFunctionHandler