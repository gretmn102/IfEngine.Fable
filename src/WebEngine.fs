namespace IfEngine.Fable.WebEngine
open IfEngine
open IfEngine.SyntaxTree

open IfEngine.Fable.SyntaxTree

[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | InputMsgCore of Engine.InputMsg<'CustomStatementArg>
    | Save
    | Load
    | NewGame

[<RequireQualifiedAccess>]
type OutputMsg<'CustomStatementOutput> =
    | OutputMsgCore of Engine.OutputMsg<Text, 'CustomStatementOutput>

type WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> when 'Label: comparison =
    {
        CoreEngine: Engine.Engine<Text, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        CustomStatementHandler: Engine.CustomStatementHandler<Text, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        Scenario: Scenario<Text, 'Label, 'CustomStatement>
        InitState: State<Text, 'Label, 'CustomStatement>
        SavedState: State<Text, 'Label, 'CustomStatement>
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module WebEngine =
    let create
        (customStatementHandler: Engine.CustomStatementHandler<Text, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        (scenario: Scenario<Text, 'Label, 'CustomStatement>)
        (gameState: State<Text, 'Label, 'CustomStatement>)
        : Result<WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

        Engine.Engine.create
            customStatementHandler
            scenario
            gameState
        |> Result.map (fun coreEngine ->
            {
                CoreEngine =
                    coreEngine
                CustomStatementHandler = customStatementHandler
                Scenario = scenario
                InitState = gameState
                SavedState = gameState
            }
        )

    let getCurrentOutputMsg
        (engine: WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : OutputMsg<'CustomStatementOutput> =

        Engine.Engine.getCurrentOutputMsg
            engine.CoreEngine
        |> OutputMsg.OutputMsgCore

    let update
        (msg: InputMsg<'CustomStatementArg>)
        (engine: WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : Result<WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

        match msg with
        | InputMsg.InputMsgCore msg ->
            Engine.Engine.update msg engine.CoreEngine
            |> Result.map (fun coreEngine ->
                { engine with
                    CoreEngine = coreEngine
                }
            )
        | InputMsg.Save ->
            { engine with
                SavedState = engine.CoreEngine.GameState
            }
            |> Ok
        | InputMsg.Load ->
            create
                engine.CustomStatementHandler
                engine.Scenario
                engine.SavedState
        | InputMsg.NewGame ->
            create
                engine.CustomStatementHandler
                engine.Scenario
                engine.InitState
