namespace Trafigura.XLLoop

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Net.Sockets
open System.Threading

open Option

type FunctionServer(port: int, provider: IFunctionProvider) =

    let handler = CompositeFunctionHandler([ provider.GetHandler; FunctionInformationHandler([], HashSet([ provider ]), None); InitializeHandler() ]) :> IFunctionHandler

    let getArgs (protocol: BinaryRequestProtocol) =
        let argCount = protocol.ReceiveInt()
        Array.init argCount (fun _ -> protocol.Receive())

    let initializeSession (args: XLOper array) =
        match args.Length with
        | 0 -> { User = None; Host = None; Key = None }
        | 1 -> { User = Some(args.[0].ToString()); Host = None; Key = None }
        | 2 -> { User = Some(args.[0].ToString()); Host = Some(args.[1].ToString()); Key = None }
        | 3 -> { User = Some(args.[0].ToString()); Host = Some(args.[1].ToString()); Key = Some(args.[2].ToString()) }
        | _ -> failwith "Invalid initialization array"

    let getContext (protocol: BinaryRequestProtocol)(session: Session) =
        let caller = match protocol.Receive() with
            | XLSRef(ref) -> Some(ref)
            | _ -> None
        let sheetName = match protocol.Receive() with
            | XLString(s) -> Some(s)
            | _ -> None
        FunctionContext(session, caller, sheetName)

    let handleRequest (protocol: BinaryRequestProtocol)(session: Session option ref) =
        try
            let nameOrVersion = protocol.Receive()
            let (name, context) =
                match nameOrVersion with
                | XLInt(version) ->
                    if version = 20 then
                        let context = if protocol.ReceiveBool() then Some(getContext protocol ((!session).Value)) else None
                        let name = protocol.ReceiveString()
                        (name, context)      
                    else
                        raise (InvalidBinaryProtocolVersion(version))
                | XLString(name) ->
                    (name, None)
                | _ -> raise (FormatException())

            let args = getArgs protocol
            if (!session).IsNone && name = BuiltinFunctions.INITIALIZE then session := Some(initializeSession args)
            let context1 = if (!session).IsSome && context.IsNone then Some(FunctionContext((!session).Value, None, None)) else None

            let res = handler.Execute context1 name args |? XLError(XLErrorType.NULL)
            protocol.Send res
            ()
        with
            | InvalidBinaryProtocolVersion(version) ->
                let message = String.Format("#Unknown protocol version {0}", version)
                // TODO log
                protocol.SendString message
                protocol.Close()
            | RequestException(msg) ->
                // TODO log
                try
                    protocol.SendString msg
                with
                | :? SocketException ->
                    protocol.Close()
                | _ -> ()
            | _ ->
                // TODO log
                protocol.Close()


    let mutable serverState: IDisposable = null

    let asyncAccept (listener: TcpListener) = Async.FromBeginEnd(listener.BeginAcceptTcpClient, listener.EndAcceptTcpClient)
            
    let start() =
        let cts = new CancellationTokenSource()
        let listener = new TcpListener(IPAddress.Any, port)
        listener.Start()

        let rec accept() = async {
                let! client = asyncAccept listener
                let protocol = BinaryRequestProtocol(client)
                let session = ref None
                while client.Connected do
                    handleRequest protocol session

                client.Close()
                return! accept()
            }

        Async.Start(accept(), cancellationToken = cts.Token)
        let serverState = { new IDisposable with member d.Dispose() = cts.Cancel(); listener.Stop() }
        ()

    let stop() = if not(serverState = null) then serverState.Dispose()

    new(handler) = FunctionServer(5454, handler)

    member this.Start() = start()
    member this.Stop() = stop()