namespace Trafigura.XLLoop.Tests

open Trafigura.XLLoop

module FromXLOperTests =
    open Xunit
    open XLOperOps

    [<Fact>]
    let xlArraytoArray2DTest() =
        let xlArray = { Rows = 2; Columns = 2; Data = [| XLInt(1); XLInt(2); XLInt(3); XLInt(4) |] }
        let nativeArray = xlArrayToArray2D xlArray
        Assert.Equal(array2D [[XLInt(1); XLInt(2)]; [XLInt(3); XLInt(4)]], nativeArray)

    // Basic types
    [<Fact>]
    let fromXLNilIntTest() =
        let op = XLNil
        let value = fromXLOper (typeof<int>) op :?> int
        Assert.Equal(0, value)

    [<Fact>]
    let fromXLNilStringTest() =
        let op = XLNil
        let value = fromXLOper (typeof<string>) op :?> string
        Assert.Equal<string>("", value)

    [<Fact>]
    let fromXLMissingIntTest() =
        let op = XLMissing
        let value = fromXLOper (typeof<int>) op :?> int
        Assert.Equal(0, value)

    [<Fact>]
    let fromXLMissingStringTest() =
        let op = XLMissing
        let value = fromXLOper (typeof<string>) op :?> string
        Assert.Equal<string>("", value)

    [<Fact>]
    let fromXLErrorStringTest() =
        let op = XLError(XLErrorType.DIV0)
        let value = fromXLOper (typeof<string>) op :?> string
        Assert.Equal<string>(null, value)

    [<Fact>]
    let fromXLSRefStringTest() =
        let op = XLSRef({ ColFirst = 1; ColLast = 1; RowFirst = 1; RowLast = 1 })
        let value = fromXLOper (typeof<string>) op :?> string
        Assert.Equal<string>(null, value)

    [<Fact>]
    let fromXLStringStringTest() =
        let op = XLString("string")
        let value = fromXLOper (typeof<string>) op :?> string
        Assert.Equal<string>("string", value)

    [<Fact>]
    let fromXLBoolIntTest() =
        let op = XLBool(false)
        let value = fromXLOper (typeof<int>) op :?> int
        Assert.Equal(0, value)

    [<Fact>]
    let fromXLBoolInt64Test() =
        let op = XLBool(false)
        let value = fromXLOper (typeof<int64>) op :?> int64
        Assert.Equal(0L, value)

    [<Fact>]
    let fromXLBoolBoolTest() =
        let op = XLBool(false)
        let value = fromXLOper (typeof<bool>) op :?> bool
        Assert.False(value)

    [<Fact>]
    let toXLBoolDoubleTest() =
        let op = XLBool(false)
        let value = fromXLOper (typeof<double>) op :?> double
        Assert.Equal(0.0, value)

    [<Fact>]
    let fromXLIntIntTest() =
        let op = XLInt(1)
        let value = fromXLOper (typeof<int>) op :?> int
        Assert.Equal(1, value)

    [<Fact>]
    let fromXLIntInt64Test() =
        let op = XLInt(1)
        let value = fromXLOper (typeof<int64>) op :?> int64
        Assert.Equal(1L, value)

    [<Fact>]
    let fromXLIntBoolTest() =
        let op = XLInt(1)
        let value = fromXLOper (typeof<bool>) op :?> bool
        Assert.True(value)

    [<Fact>]
    let toXLIntDoubleTest() =
        let op = XLInt(1)
        let value = fromXLOper (typeof<double>) op :?> double
        Assert.Equal(1.0, value)

    [<Fact>]
    let fromXLNumIntTest() =
        let op = XLNum(10.0)
        let value = fromXLOper (typeof<int>) op :?> int
        Assert.Equal(10, value)

    [<Fact>]
    let fromXLNumInt64Test() =
        let op = XLNum(10.0)
        let value = fromXLOper (typeof<int64>) op :?> int64
        Assert.Equal(10L, value)

    [<Fact>]
    let fromXLNumBoolTest() =
        let op = XLNum(10.0)
        let value = fromXLOper (typeof<bool>) op :?> bool
        Assert.True(value)

    [<Fact>]
    let toXLNumDoubleTest() =
        let op = XLNum(10.0)
        let value = fromXLOper (typeof<double>) op :?> double
        Assert.Equal(10.0, value)

    // Array tests
    [<Fact>]
    let toXLArrayInt1DNonGenericTest() =
        let op = XLArray({ Rows = 2; Columns = 1; Data = [| XLInt(1); XLInt(2) |] })
        let value = fromXLOper (typeof<obj array>) op :?> obj array
        Assert.Equal<obj array>([| box 1; box 2 |], value)

    [<Fact>]
    let toXLArrayInt1DTest() =
        let op = XLArray({ Rows = 2; Columns = 1; Data = [| XLInt(1); XLInt(2) |] })
        let value = fromXLOper (typeof<int array>) op :?> int array
        Assert.Equal<int array>([| 1; 2 |], value)

    [<Fact>]
    let toXLArrayIntToDouble1DTest() =
        let op = XLArray({ Rows = 2; Columns = 1; Data = [| XLInt(1); XLInt(2) |] })
        let value = fromXLOper (typeof<double array>) op :?> double array
        Assert.Equal<double array>([| 1.0; 2.0 |], value)

    [<Fact>]
    let toXLArrayInt2DTest() =
        let op = XLArray({ Rows = 2; Columns = 2; Data = [| XLInt(1); XLInt(2); XLInt(3); XLInt(4) |] })
        let value = fromXLOper (typeof<int[,]>) op :?> int[,]
        Assert.Equal<int[,]>(array2D [[ 1; 2 ]; [ 3; 4 ]], value)

    [<Fact>]
    let toXLArrayInt2DJaggedTest() =
        let op = XLArray({ Rows = 2; Columns = 2; Data = [| XLInt(1); XLInt(2); XLInt(3); XLInt(4) |] })
        let value = fromXLOper (typeof<int array array>) op :?> int array array
        Assert.Equal<int array array>([| [| 1; 2 |]; [| 3; 4 |] |], value)

    [<Fact>]
    let toXLArrayIntListTest() =
        let op = XLArray({ Rows = 2; Columns = 1; Data = [| XLInt(1); XLInt(2) |] })
        let value = fromXLOper (typeof<int list>) op :?> int list
        Assert.Equal<int list>([ 1; 2 ], value)
