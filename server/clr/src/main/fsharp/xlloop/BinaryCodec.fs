namespace Trafigura.XLLoop

open System
open System.IO
open System.Net
open System.Net.Sockets

module BinaryCodec =

    // We need big endian format. Default is little endian for binary writer
    let writeDoubleWord (v: int)(bw: BinaryWriter) = bw.Write(IPAddress.HostToNetworkOrder v)

    let rec encode (bw: BinaryWriter)(xloper: XLOper) =
        bw.Write(byte(xloper.Type))
        match xloper with
        | XLError(e) -> bw.Write(int(e))
        | XLBool(b) -> bw.Write(if b then 1uy else 0uy)
        | XLInt(i) -> writeDoubleWord i bw
        | XLNum(d) ->
            let l = BitConverter.DoubleToInt64Bits d
            writeDoubleWord (int(l >>> 32)) bw
            writeDoubleWord (int(l)) bw
        | XLString(s) ->
            let s255 = String255(s)
            bw.Write(byte(s255.ToString().Length))
            for b in s255.ToString() do bw.Write(byte(b))
        | XLArray(value) ->
            writeDoubleWord (value.Rows) bw
            writeDoubleWord (value.Columns) bw
            for i in 0 .. (value.Rows * value.Columns - 1) do encode bw (value.Data.[i])
        | XLSRef(value) ->
            writeDoubleWord value.ColFirst bw
            writeDoubleWord value.ColLast bw
            writeDoubleWord value.RowFirst bw
            writeDoubleWord value.RowLast bw
        | _ -> ()


    let readDoubleWord (br: BinaryReader) = IPAddress.NetworkToHostOrder (br.ReadInt32())

    let rec decode (br: BinaryReader) =
        let intType = int(br.ReadByte())
        if intType = -1 then raise (EndOfStreamException())
        let xlOperType = enum<XLOperType>(intType)
        match xlOperType with
        | XLOperType.xlTypeNum -> XLNum(BitConverter.Int64BitsToDouble (int64(readDoubleWord br) <<< 32 ||| int64(readDoubleWord br)))
        | XLOperType.xlTypeStr ->
            let length = int(br.ReadByte())
            if length = -1 then raise (EndOfStreamException())
            let data = br.ReadBytes(length) |> Array.map (fun b -> char(b))
            XLString(new String(data))
        | XLOperType.xlTypeBool -> XLBool(not(br.ReadByte() = 0uy))
        | XLOperType.xlTypeErr -> XLError(enum<XLErrorType>(readDoubleWord br))
        | XLOperType.xlTypeMulti ->
            let rows = readDoubleWord br
            let columns = readDoubleWord br
            let arr = Array.init (rows * columns) (fun _ -> decode br)
            XLArray({ Rows = rows; Columns = columns; Data = arr })
        | XLOperType.xlTypeMissing -> XLMissing
        | XLOperType.xlTypeNil -> XLNil
        | XLOperType.xlTypeInt -> XLInt(readDoubleWord br)
        | XLOperType.xlTypeSRef -> XLSRef({ ColFirst = readDoubleWord br; ColLast = readDoubleWord br; RowFirst = readDoubleWord br; RowLast = readDoubleWord br })
        | _ ->
            raise (FormatException(String.Format("Unknown xlOperType {0} when decoding binary stream", xlOperType)))


exception InvalidBinaryProtocolVersion of int
exception InvalidBinaryProtocolFormat

type BinaryRequestProtocol(client: TcpClient) =
    let stream = client.GetStream()
    let reader = new BinaryReader(stream)
    let writer = new BinaryWriter(stream)
    let close() =
        try
            stream.Close()
            client.Close()
        with
            | _ -> ()

    let receive() = BinaryCodec.decode reader
    let send = BinaryCodec.encode writer

    let receiveBool() = match receive() with
        | XLBool(b) -> b
        | _ -> raise InvalidBinaryProtocolFormat

    let receiveInt() = match receive() with
        | XLInt(i) -> i
        | _ -> raise InvalidBinaryProtocolFormat

    let receiveString() = match receive() with
        | XLString(s) -> s
        | _ -> raise InvalidBinaryProtocolFormat

    let sendString s = send (XLString(s))

    member this.Receive = receive
    member this.Send = send

    member this.ReceiveBool = receiveBool
    member this.ReceiveInt = receiveInt
    member this.ReceiveString = receiveString

    member this.SendString = sendString

    member this.Close() = close()
