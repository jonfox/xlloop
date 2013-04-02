namespace Trafigura.XLLoop.Tests
open System

open Trafigura.XLLoop

module ToXLOperTests =
    open Xunit
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

    [<Fact>]
    let toXLOperBool2DJaggedListTest() =
        let value = [ [ true; false; true ]; [ true; false ] ]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLBool(true); XLBool(false); XLBool(true); XLBool(true); XLBool(false); XLNil |] }), op)

    // Reference type collections
    [<Fact>]
    let toXLOperStringListTest() =
        let value = ["hi"; "there"]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 1; Data = [| XLString("hi"); XLString("there") |] }), op)

    [<Fact>]
    let toXLOperDoubleListTest() =
        let value = [0.15;  0.25; 0.75]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 3; Columns = 1; Data = [| XLNum(0.15); XLNum(0.25); XLNum(0.75) |] }), op)

    [<Fact>]
    let toXLOperDouble2DJaggedListTest() =
        let value = [ [ 0.1; 0.25; 0.5; 0.75; 0.9 ]; [ 0.0225; 0.2; 0.0; -0.01; -0.005 ] ]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 5; Data = [| XLNum(0.1); XLNum(0.25); XLNum(0.5); XLNum(0.75); XLNum(0.9); XLNum(0.0225); XLNum(0.2); XLNum(0.0); XLNum(-0.01); XLNum(-0.005) |] }), op)

    [<Fact>]
    let toXLOperDateTimeDoubleTupleListTest() =
        let value = [(DateTime(2013, 3, 13), 0.15);(DateTime(2013, 3, 14),  0.25); (DateTime(2013, 3, 15), 0.75)]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 3; Columns = 2; Data = [| XLNum(41346.0); XLNum(0.15); XLNum(41347.0); XLNum(0.25); XLNum(41348.0); XLNum(0.75) |] }), op)

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

    [<Fact>]
    let toXLOperString2DJaggedListTest() =
        let value = [ [ "the"; "quick"; "brown" ]; [ "fox"; "jumped" ] ]
        let op = toXLOper value
        Assert.Equal(XLArray({ Rows = 2; Columns = 3; Data = [| XLString("the"); XLString("quick"); XLString("brown"); XLString("fox"); XLString("jumped"); XLNil |] }), op)
