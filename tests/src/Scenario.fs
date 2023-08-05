module Scenario
open Feliz
open IfEngine
open IfEngine.Utils
open IfEngine.Types
open IfEngine.Fable.Utils
open IfEngine.Fable.WebEngine

type CustomStatementArg = unit

type CustomStatement = unit
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatement =
    let apply state stack (arg: CustomStatementArg) customStatement =
        failwithf "not implemented yet"

    let handle subIndex customStatement =
        failwithf "not implemented yet"

type CustomStatementOutput = unit

type LabelName =
    | Crossroad
    | LeftRoad
    | RightRoad

let beginLoc = Crossroad

let scenario, vars =
    let vars = Map.empty
    let getApplesCount, updateApplesCount, vars = createNumVar "apples" 0 vars
    let getRoadApplesCount, updateRoadApplesCount, vars = createNumVar "applesOnRoad" 1 vars

    [
        label Crossroad [
            menu [
                Html.text "Ты стоишь на развилке в лесу."
            ] [
                choice "пойти влево" [ jump LeftRoad ]
                choice "пойти вправо" [ jump RightRoad ]
            ]
        ]

        label LeftRoad [
            if' (fun vars ->
                getApplesCount vars > 0
            ) [
                menu [
                    Html.text "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть."
                ] [
                    choice "Покормить" [
                        updateApplesCount (fun count -> count - 1)
                        say "Ты покормил ёжика"
                    ]
                    choice "Вернуться на развилку" [ jump Crossroad ]
                ]
            ] [
                menu [
                    Html.text "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть."
                ] [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]

        label RightRoad [
            if' (fun vars ->
                getRoadApplesCount vars > 0
            ) [
                menu [
                    Html.text "По правой дороге ты находишь яблоко."
                ] [
                    choice "Поднять" [
                        updateRoadApplesCount (fun x -> x - 1)
                        updateApplesCount ((+) 1)

                        jump RightRoad
                    ]
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ] [
                menu [
                    Html.text "По правой дороге больше ничего нет."
                ] [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]
    ]
    |> List.map (fun (labelName, body) -> labelName, (labelName, body))
    |> Map.ofList
    |> fun scenario ->
        (scenario: Scenario<Text, _, CustomStatement>), vars
