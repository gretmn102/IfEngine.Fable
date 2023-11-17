module Scenario
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree.CommonContent.Helpers
open Farkdown.Experimental.Helpers

module Farkdown =
    module Helpers =
        let textf format =
            Printf.ksprintf text format

open Farkdown.Helpers

type CustomStatementArg = unit

type CustomStatement = unit

type CustomStatementOutput = unit

type LabelName =
    | Menu
    | Authors
    | VariablesDefinition
    | Crossroad
    | LeftRoad
    | RightRoad

let beginLoc = Menu

let applesCount = VarsContainer.createNum "apples"
let isApplesOnRightRoad = VarsContainer.createBool "isApplesOnRightRoad"

let scenario: Scenario<CommonContent.Content, _, CustomStatement> =
    [
        label Menu [
            menu [
                h1 [ text "Тестовая игра про "; italic (bold (text "ёжика")) ] [
                    p [
                        [ img "https://cdn-icons-png.flaticon.com/512/8323/8323738.png" "hedgehog" "hedgehog" ]
                    ]
                ]
            ] [
                choice "Начать игру" [ jump VariablesDefinition ]
                choice "Авторов!" [ jump Authors ]
            ]
        ]

        label Authors [
            menu [
                h1 [ text "Авторы!" ] [
                    ul [
                        li [ text "Ежик1" ] [
                            p [[ text "Простой ежик" ]]
                        ]
                        li [ text "Ежик2" ] [
                            ol [
                                li [ text "Простой" ] []
                                li [ text "Суперпростой" ] [
                                    p [
                                        [ text "(И даже слишком" ]
                                        [ text "простой)"]
                                    ]
                                ]
                            ]

                        ]
                    ]
                ]
            ] [
                choice "Назад" [ jump Menu ]
            ]
        ]

        label VariablesDefinition [
            applesCount := 0
            isApplesOnRightRoad := true
            jump Crossroad
        ]

        label Crossroad [
            menu [
                h1 [ text "Развилка" ] [
                    p [[ text "Ты находишься на развилке в лесу" ]]
                    p [[ img "https://cdn-icons-png.flaticon.com/512/11089/11089428.png" "crossroad" "crossroad" ]]
                ]
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
                    p [[ text "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть." ]]
                ] [
                    choice "Покормить" [
                        update applesCount (fun count -> count - 1)
                        menu [
                            p [[ text "Ты покормил ёжика" ]]
                            p [[ img "https://cdn-icons-png.flaticon.com/512/3370/3370799.png" "hedgehog with apple" "hedgehog with apple" ]]
                        ] [
                            choice "Вернуться в глав меню" [
                                jump Menu
                            ]
                        ]
                    ]
                    choice "Вернуться на развилку" [ jump Crossroad ]
                ]
            ] [
                menu [
                    p [[ text "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть." ]]
                ] [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]

        label RightRoad [
            if' (Var.get isApplesOnRightRoad) [
                menu [
                    p [[ text "По правой дороге ты находишь "; bold (text "яблоко"); text "." ]]
                    p [[ img "https://cdn-icons-png.flaticon.com/512/415/415682.png" "apple" "apple" ]]
                ] [
                    choice "Поднять" [
                        isApplesOnRightRoad := false
                        update applesCount ((+) 1)

                        interSay (fun vars ->
                            [
                                p [[ textf "Теперь у тебя %d яблок." (Var.get applesCount vars) ]]
                            ]
                        )

                        jump RightRoad
                    ]
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ] [
                menu [
                    p [[ text "По правой дороге больше ничего нет." ]]
                ] [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]
    ]
    |> List.map (fun (labelName, body) -> labelName, (labelName, body))
    |> Map.ofList
