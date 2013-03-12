namespace Trafigura.XLLoop

open log4net

open Option

module ReflectionHandlerOps =
    let excelMethodName excelNamespace (instanceMethod: InstanceMethod) = excelNamespace + (instanceMethod :> IFunction).Name

    let addMethod (name: string)(instanceMethod: InstanceMethod)(methods: Map<string, IFunction>) =
        let finalMethod =
            let existingMethod = methods |> Map.tryFind name
            if existingMethod.IsNone then
                instanceMethod :> IFunction
            else
                match existingMethod.Value with
                | :? InstanceMethod as im -> OverloadedMethod([instanceMethod; im]) :> IFunction
                | :? OverloadedMethod as om -> (om.AddMethod instanceMethod) :> IFunction
                | _ -> failwith "Unknown method type"

        methods |> Map.add name finalMethod

    let addInstanceMethods excelNamespace instance methods =
        InstanceMethod.GetInstanceMethods instance |> List.fold (fun acc im -> acc |> addMethod (excelMethodName excelNamespace im) im) methods

    
open ReflectionHandlerOps
// TODO support object registry
type ReflectFunctionHandler(methods: Map<string, IFunction>, information: Map<string, FunctionInformation>) =

    let logger = LogManager.GetLogger("xlloop.ReflectFunctionHandler")

    let createFunctionInformation (name: string)(f: IFunction) =
        match f with
        | :? InstanceMethod as im -> FunctionInformation(name, im.ParameterNames, im.ParameterTypeNames)
        | :? OverloadedMethod as om -> FunctionInformation(name, om.FirstMethod.ParameterNames, om.FirstMethod.ParameterTypeNames)
        | _ -> FunctionInformation(name)
    
    let getFunctions = methods |> Map.toList |> List.map (fun (name, im) -> information.TryFind name |? createFunctionInformation name im)
    
    let execute context name args =
        let argsString = String.concat ", " (args |> Array.map(fun a -> a.ToString()))
        logger.InfoFormat(name + " (" + argsString + ")")

        let f = methods.TryFind name
        if f.IsNone then
            raise (RequestException("#Unknown method {0}" + name))
        else
            f.Value.Execute context args

        
    new() = ReflectFunctionHandler(Map.empty<string, IFunction>, Map.empty<string, FunctionInformation>)

    new(excelNamespace, instance) =
        let methods = addInstanceMethods excelNamespace instance Map.empty<string, IFunction>
        ReflectFunctionHandler(methods, Map.empty<string, FunctionInformation>)


    member this.AddInstanceMethods(excelNamespace, instance) =
        let updatedMethods = addInstanceMethods excelNamespace instance methods
        ReflectFunctionHandler(updatedMethods, Map.empty<string, FunctionInformation>)

    interface IFunctionHandler with
        member this.HasFunction name = methods.ContainsKey name
        member this.Execute context name args = execute context name args

    interface IFunctionProvider with
        member this.GetFunctions = getFunctions
        member this.GetHandler = this :> IFunctionHandler