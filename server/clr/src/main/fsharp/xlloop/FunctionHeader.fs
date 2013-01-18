namespace Trafigura.XLLoop

exception RequestException of string

type Session = { User: string option; Host: string option; Key: string option }

module BuiltinFunctions =
    let INITIALIZE = "org.boris.xlloop.Initialize"
    let GET_FUNCTIONS = "org.boris.xlloop.GetFunctions"
    let GET_MENU = "org.boris.xlloop.GetMenu"
    let EXEC_COMMAND = "org.boris.xlloop.ExecuteCommand"

type IFunctionHandler =
    abstract HasFunction: string -> bool
    abstract Execute: FunctionContext option -> string -> XLOper array -> XLOper option
and FunctionContext(handler: IFunctionHandler, session: Session, caller: XLSRefType option, sheetName: string option) =
    member this.Host = session.Host
    member this.User = session.User
    member this.UserKey = session.Key
    member this.Caller = caller
    member this.SheetName = sheetName
    //member this.Execute name args = handler.Execute (Some(this)) name args

type IFunctionProvider =
    abstract GetFunctions: FunctionInformation list
    abstract GetHandler: IFunctionHandler

type IFunction =
    abstract Name: string
    abstract Execute: FunctionContext option -> XLOper array -> XLOper option
