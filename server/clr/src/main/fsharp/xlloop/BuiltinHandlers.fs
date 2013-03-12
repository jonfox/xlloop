namespace Trafigura.XLLoop

open System
open System.Collections.Generic

open XLOperOps


type InitializeHandler() =
    interface IFunctionHandler with
        member this.HasFunction name = name = BuiltinFunctions.INITIALIZE
        member this.Execute _ _ _ = None


type FunctionInformationHandler(functions: FunctionInformation list, functionProviders: HashSet<IFunctionProvider>, category: string option) =

    let mapCategory (fi: FunctionInformation) = if category.IsSome then fi.SetCategory(category.Value) else fi

    let toXLOper =
        let allFunctions = functions :: [ for fp in functionProviders -> fp.GetFunctions ] |> List.flatten
        let allFunctionsWithCategory = allFunctions |> List.map (fun fi -> mapCategory fi)
        [ for fi in allFunctionsWithCategory -> fi.Encode ] |> toXLOper

    interface IFunctionHandler with
        member this.HasFunction name = name = BuiltinFunctions.GET_FUNCTIONS
        member this.Execute _ _ _ = Some(toXLOper)

        
type CompositeFunctionHandler(handlers: IFunctionHandler list) =

    let findHandler name = handlers |> List.tryFind (fun h -> h.HasFunction name)
   
    let execute context name args =
        let handler = findHandler name
        if handler.IsSome then
            handler.Value.Execute context name args
        else
            let message = String.Format("#Unknown function: " + name)
            // TODO Log it
            raise (RequestException(message))

    interface IFunctionHandler with
        member this.HasFunction name = (findHandler name).IsSome
        member this.Execute context name args = execute context name args


//type DebugFunctionHandler(handler: IFunctionHandler, label: string) =
// TODO Log properly