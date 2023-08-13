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
    | OutputMsgCore of Engine.OutputMsg<Content, 'CustomStatementOutput>

type WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> when 'Label: comparison =
    {
        CoreEngine: Engine.Engine<Content, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        CustomStatementHandler: Engine.CustomStatementHandler<Content, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        Scenario: Scenario<Content, 'Label, 'CustomStatement>
        InitState: State<Content, 'Label, 'CustomStatement>
        SavedState: State<Content, 'Label, 'CustomStatement>
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module WebEngine =
    let create
        (customStatementHandler: Engine.CustomStatementHandler<Content, 'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        (scenario: Scenario<Content, 'Label, 'CustomStatement>)
        (gameState: State<Content, 'Label, 'CustomStatement>)
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
