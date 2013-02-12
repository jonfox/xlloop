namespace Trafigura.XLLoop.Tests

open Trafigura.XLLoop

module UtilsTests =
    open Xunit
    open Types

    [<Fact>]
    let decomposeArray1D() =
        let (collectionType, elementType) = decomposeCollectionType (typeof<int array>)
        Assert.Equal(Array1D, collectionType)
        Assert.Equal(typeof<int>, elementType)

    [<Fact>]
    let decomposeArray2D() =
        let (collectionType, elementType) = decomposeCollectionType (typeof<int[,]>)
        Assert.Equal(Array2D, collectionType)
        Assert.Equal(typeof<int>, elementType)

    [<Fact>]
    let decomposeArray2DJagged() =
        let (collectionType, elementType) = decomposeCollectionType (typeof<int array array>)
        Assert.Equal(Array2DJagged, collectionType)
        Assert.Equal(typeof<int>, elementType)

    [<Fact>]
    let decomposeList() =
        let (collectionType, elementType) = decomposeCollectionType (typeof<int list>)
        Assert.Equal(FSharpList, collectionType)
        Assert.Equal(typeof<int>, elementType)
