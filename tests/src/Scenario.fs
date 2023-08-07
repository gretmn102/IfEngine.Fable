module Scenario
open Feliz
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.Fable.Utils
open IfEngine.Fable.WebEngine

type CustomStatementArg = unit

type CustomStatement = unit

type CustomStatementOutput = unit

type LabelName =
    | VariablesDefinition
    | Crossroad
    | LeftRoad
    | RightRoad

let beginLoc = VariablesDefinition

let applesCount = VarsContainer.createNum "apples"
let isApplesOnRightRoad = VarsContainer.createBool "isApplesOnRightRoad"

let scenario =
    [
        label VariablesDefinition [
            applesCount := 0
            isApplesOnRightRoad := true
            jump Crossroad
        ]

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
                Var.get applesCount vars > 0
            ) [
                menu [
                    Html.text "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть."
                ] [
                    choice "Покормить" [
                        update applesCount (fun count -> count - 1)
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
            if' (Var.get isApplesOnRightRoad) [
                menu [
                    Html.text "По правой дороге ты находишь яблоко."
                ] [
                    choice "Поднять" [
                        isApplesOnRightRoad := false
                        update applesCount ((+) 1)

                        interSay (fun vars ->
                            Var.get applesCount vars
                            |> sprintf "Теперь у тебя %d яблок."
                        )

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
        (scenario: Scenario<Text, _, CustomStatement>)
