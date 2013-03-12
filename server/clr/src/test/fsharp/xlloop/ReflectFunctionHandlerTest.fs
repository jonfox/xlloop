namespace Trafigura.XLLoop.Tests

open Trafigura.XLLoop


type TestReflectionType() =
    member this.Method1 (i: int) = i
    // 2 overloads
    member this.Method2 (i: int, j: int) = i + j
    member this.Method2 (i: int, j: int, k: int) = i + j + k
    // 3 overloads
    member this.Method3 (i: int, j: int) = i + j
    member this.Method3 (i: int, j: int, k: int) = i + j + k
    member this.Method3 (i: int, j: int, k: int, l: int) = i + j + k + l

module ReflectFunctionHandlerTests =
    open Xunit

    [<Fact>]
    let singleMethodTest() =
        let reflectionObject = TestReflectionType()
        let rfh = ReflectFunctionHandler("", reflectionObject)
        let result = (rfh :> IFunctionHandler).Execute None "Method1" [| XLInt(1) |]
        Assert.Equal(Some(XLInt(1)), result)

    [<Fact>]
    let doubleMethodTest() =
        let reflectionObject = TestReflectionType()
        let rfh = ReflectFunctionHandler("", reflectionObject)
        let result1 = (rfh :> IFunctionHandler).Execute None "Method2" [| XLInt(1); XLInt(2) |]
        Assert.Equal(Some(XLInt(3)), result1)
        let result2 = (rfh :> IFunctionHandler).Execute None "Method2" [| XLInt(1); XLInt(2); XLInt(3) |]
        Assert.Equal(Some(XLInt(6)), result2)

    [<Fact>]
    let tripleMethodTest() =
        let reflectionObject = TestReflectionType()
        let rfh = ReflectFunctionHandler("", reflectionObject)
        let result1 = (rfh :> IFunctionHandler).Execute None "Method3" [| XLInt(1); XLInt(2) |]
        Assert.Equal(Some(XLInt(3)), result1)
        let result2 = (rfh :> IFunctionHandler).Execute None "Method3" [| XLInt(1); XLInt(2); XLInt(3) |]
        Assert.Equal(Some(XLInt(6)), result2)
        let result3 = (rfh :> IFunctionHandler).Execute None "Method3" [| XLInt(1); XLInt(2); XLInt(3); XLInt(4) |]
        Assert.Equal(Some(XLInt(10)), result3)
