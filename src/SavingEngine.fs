namespace IfEngine.Fable.SavingEngine
open IfEngine
open IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | InputMsgCore of Engine.InputMsg<'CustomStatementArg>
    | Save
    | Load
    | NewGame

[<RequireQualifiedAccess>]
type OutputMsg<'Content, 'CustomStatementOutput> =
    | OutputMsgCore of Engine.OutputMsg<'Content, 'CustomStatementOutput>

type Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> when 'Label: comparison =
    {
        CoreEngine: Engine.Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        CustomStatementHandler: Engine.CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
        Scenario: Scenario<'Content, 'Label, 'VarsContainer, 'CustomStatement>
        InitState: State<'Content, 'Label, 'VarsContainer>
        SavedState: State<'Content, 'Label, 'VarsContainer>
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    let create
        (customStatementHandler: Engine.CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        (scenario: Scenario<'Content, 'Label, 'VarsContainer, 'CustomStatement>)
        (gameState: State<'Content, 'Label, 'VarsContainer>)
        : Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

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
        (engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : OutputMsg<'Content, 'CustomStatementOutput> =

        Engine.Engine.getCurrentOutputMsg
            engine.CoreEngine
        |> OutputMsg.OutputMsgCore

    let update
        (msg: InputMsg<'CustomStatementArg>)
        (engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

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
            Engine.Engine.create
                engine.CustomStatementHandler
                engine.Scenario
                engine.SavedState
            |> Result.map (fun coreEngine ->
                { engine with
                    CoreEngine = coreEngine
                }
            )

        | InputMsg.NewGame ->
            Engine.Engine.create
                engine.CustomStatementHandler
                engine.Scenario
                engine.InitState
            |> Result.map (fun coreEngine ->
                { engine with
                    CoreEngine = coreEngine
                }
            )
