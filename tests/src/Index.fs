module Index
open Elmish
open IfEngine.Fable.Utils

type State =
    {
        IfEngineState: IfEngine.Game.State<Text, Scenario.LabelName, Scenario.CustomStatement, Scenario.CustomStatementArg>
    }

type Msg =
    | IfEngineMsg of IfEngine.Game.Msg<Scenario.CustomStatement, Scenario.CustomStatementArg>

let init () =
    let st =
        {
            IfEngineState =
                Scenario.gameState
        }
    st, Cmd.none

let update (msg: Msg) (state: State) =
    let updateGame msg =
        let gameState =
            Scenario.update msg state.IfEngineState

        { state with
            IfEngineState = gameState
        }

    match msg with
    | IfEngineMsg msg ->
        updateGame msg, Cmd.none

let view (state: State) (dispatch: Msg -> unit) =
    IfEngine.Fable.Index.view
        (fun (customStatement: Scenario.CustomStatement) state' dispatch' ->
            failwithf "addon not implemented"
        )
        state.IfEngineState
        (IfEngineMsg >> dispatch)
