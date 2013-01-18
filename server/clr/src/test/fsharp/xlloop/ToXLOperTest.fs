namespace Trafigura.XLLoop.Tests

open Trafigura.XLLoop

open Xunit

module ToXLOperTests =
    open XLOperOps

    // Basic types
    [<Fact>]
    let toXLOperBoolTest() =
        let value = true
        let op = toXLOper value
        Assert.Equal(XLBool(true), op)

    [<Fact>]
    let toXLOperIntTest() =
        let value = 42
        let op = toXLOper value
        Assert.Equal(XLInt(42), op)

    [<Fact>]
    let toXLOperDoubleTest() =
        let value = 3.14159
        let op = toXLOper value
        Assert.Equal(XLNum(3.14159), op)

    [<Fact>]
    let toXLOperStringTest() =
        let value = "string"
        let op = toXLOper value
        Assert.Equal(XLString("string"), op)

    // Value type collections
    [<Fact>]
    let toXLOperBoolListTest() =
        let value = [true; false]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 1; Data = [| XLBool(true); XLBool(false) |] }), op)

    [<Fact>]
    let toXLOperBoolArrayTest() =
        let value = [| true; false |]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 1; Data = [| XLBool(true); XLBool(false) |] }), op)

    [<Fact>]
    let toXLOperBool2DArrayTest() =
        let value = array2D [ [true; false; true ]; [false; true; false] ]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLBool(true); XLBool(false); XLBool(true); XLBool(false); XLBool(true); XLBool(false) |] }), op)

    [<Fact>]
    let toXLOperBool2DJaggedArrayTest() =
        let value = [| [| true; false; true |]; [| true; false |] |]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLBool(true); XLBool(false); XLBool(true); XLBool(true); XLBool(false); XLNil |] }), op)

    // Reference type collections
    [<Fact>]
    let toXLOperStringListTest() =
        let value = ["hi"; "there"]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 1; Data = [| XLString("hi"); XLString("there") |] }), op)

    [<Fact>]
    let toXLOperStringArrayTest() =
        let value = [| "hi"; "there" |]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 1; Data = [| XLString("hi"); XLString("there") |] }), op)

    [<Fact>]
    let toXLOperString2DArrayTest() =
        let value = array2D [ ["the"; "quick"; "brown" ]; ["fox"; "jumped"; "over"] ]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLString("the"); XLString("quick"); XLString("brown"); XLString("fox"); XLString("jumped"); XLString("over") |] }), op)

    [<Fact>]
    let toXLOperString2DJaggedArrayTest() =
        let value = [| [| "the"; "quick"; "brown" |]; [| "fox"; "jumped" |] |]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLString("the"); XLString("quick"); XLString("brown"); XLString("fox"); XLString("jumped"); XLNil |] }), op)

