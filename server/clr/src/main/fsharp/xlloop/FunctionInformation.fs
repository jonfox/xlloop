namespace Trafigura.XLLoop

open XLKeyValue

type FunctionInformation(name: string, help: string option, category: string option, shortcutText: string option, helpTopic: string option, arguments: string list, argumentHelp: string list, isVolatile: bool) =

    let encode = lazy(
        let coreKV =
            [ createPair "functionName" name ] |>
            tryCons "functionHelp" (help |> Option.map box) |>
            tryCons "category" (category |> Option.map box) |>
            tryCons "shortcutText" (shortcutText |> Option.map box) |>
            tryCons "helpTopic" (helpTopic |> Option.map box)
        let argsKV =
            if arguments.Length = 0 then
                []
            else
                let argsPair = createPair "argumentText" (CSV.toCSV arguments)
                if argumentHelp.Length = 0 then
                    [argsPair]
                else
                    [(createPair "argumentHelp" argumentHelp); argsPair]
        let allKV = (createPair "isVolatile" isVolatile) :: argsKV @ coreKV
        allKV |> List.rev |> toXLOper)

    new(name: string) = FunctionInformation(name, None, None, None, None, [], [], false)
    new(name: string, arguments: string list, argumentHelp: string list) = FunctionInformation(name, None, None, None, None, arguments, argumentHelp, false)

    member this.Name = name
    member this.Help = help
    member this.Category = category
    member this.ShortcutText = shortcutText
    member this.HelpTopic = helpTopic
    member this.Arguments = arguments
    member this.ArgumentHelp = argumentHelp
    member this.IsVolatile = isVolatile
    member this.Encode = encode.Value

    member this.SetCategory (category: string) = FunctionInformation(name, help, Some(category), shortcutText, helpTopic, arguments, argumentHelp, isVolatile)

