module Index
open Elmish
open FsharpMyExtension.ResultExt
open IfEngine.Engine
open IfEngine.SyntaxTree
open IfEngine.Fable.SyntaxTree
open IfEngine.Fable.SavingEngine

type State =
    {
        IfEngineState: Engine<Content, Scenario.LabelName, Scenario.CustomStatement, Scenario.CustomStatementArg, Scenario.CustomStatementOutput>
    }

type Msg =
    | IfEngineMsg of InputMsg<Scenario.CustomStatementOutput>

let init () =
    let st =
        {
            IfEngineState =
                IfEngine.State.init
                    Scenario.beginLoc
                    VarsContainer.empty
                |> Engine.create
                    CustomStatementHandler.empty
                    Scenario.scenario
                |> Result.get
        }
    st, Cmd.none

let update (msg: Msg) (state: State) =
    let updateGame msg =
        let gameState =
            Engine.update msg state.IfEngineState
            |> Result.get

        { state with
            IfEngineState = gameState
        }

    match msg with
    | IfEngineMsg msg ->
        updateGame msg, Cmd.none

let view (state: State) (dispatch: Msg -> unit) =
    IfEngine.Fable.Index.view
        (fun (customStatement: Scenario.CustomStatement) ->
            failwithf "customStatement not implemented"
        )
        state.IfEngineState
        (IfEngineMsg >> dispatch)
