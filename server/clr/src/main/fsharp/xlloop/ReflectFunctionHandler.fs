namespace Trafigura.XLLoop

open Option

module ReflectionHandlerOps =
    let excelMethodName excelNamespace (instanceMethod: InstanceMethod) = excelNamespace + (instanceMethod :> IFunction).Name
    
open ReflectionHandlerOps
// TODO support object registry, and overloaded methods
type ReflectFunctionHandler(methods: Map<string, IFunction>, information: Map<string, FunctionInformation>) =

    let addMethod (name: string)(instanceMethod: InstanceMethod)(methods: Map<string, IFunction>) =
        let finalMethod =
            let existingMethod = methods |> Map.tryFind name
            if existingMethod.IsNone then
                instanceMethod :> IFunction
            else
                match existingMethod.Value with
                | :? InstanceMethod as im -> OverloadedMethod([instanceMethod; im]) :> IFunction
                | :? OverloadedMethod as om -> (om.AddMethod instanceMethod) :> IFunction
                | _ -> failwith "Unknown method class"

        methods |> Map.add name finalMethod

    let addInstanceMethods excelNamespace instance =
        let updatedMethods = InstanceMethod.GetInstanceMethods instance |> List.fold (fun acc im -> acc |> addMethod (excelMethodName excelNamespace im) im) methods
        ReflectFunctionHandler(updatedMethods, Map.empty<string, FunctionInformation>)

    let createFunctionInformation (f: IFunction) =
        match f with
        | :? InstanceMethod as im -> FunctionInformation(f.Name, im.ParameterNames, im.ParameterTypeNames)
        | :? OverloadedMethod as om -> FunctionInformation(f.Name, om.FirstMethod.ParameterNames, om.FirstMethod.ParameterTypeNames)
        | _ -> FunctionInformation(f.Name)
    
    let getFunctions = methods |> Map.toList |> List.map (fun (name, im) -> information.TryFind name |? createFunctionInformation im)
    
    let execute context name args =
        let f = methods.TryFind name
        if f.IsNone then
            raise (RequestException("#Unknown method {0}" + name))
        else
            f.Value.Execute context args

        
    new() = ReflectFunctionHandler(Map.empty<string, IFunction>, Map.empty<string, FunctionInformation>)

    new(excelNamespace, instance) =
        let methods = InstanceMethod.GetInstanceMethods instance |> List.map (fun im -> (excelMethodName excelNamespace im, im :> IFunction)) |> Map.ofList
        ReflectFunctionHandler(methods, Map.empty<string, FunctionInformation>)


    member this.AddInstanceMethods(excelNamespace, instance) = addInstanceMethods excelNamespace instance

    interface IFunctionHandler with
        member this.HasFunction name = methods.ContainsKey name
        member this.Execute context name args = execute context name args

    interface IFunctionProvider with
        member this.GetFunctions = getFunctions
        member this.GetHandler = this :> IFunctionHandler