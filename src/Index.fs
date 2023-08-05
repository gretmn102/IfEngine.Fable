module IfEngine.Fable.Index
open Fable.React
open Fable.FontAwesome
open Fulma
open Feliz
open IfEngine
open IfEngine.Engine

open IfEngine.Fable.Utils

let nav dispatch =
    Html.div [
        Tabs.tabs [
            Tabs.IsCentered
        ] [
            Html.ul [
                prop.children [
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.NewGame)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.File ] []
                                    span [] [ str " " ]
                                    Html.text "New Game"
                                ]
                            ]
                        ]
                    ]
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.Save)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.Save ] []
                                    span [] [ str " " ]
                                    Html.text "Save"
                                ]
                            ]
                        ]
                    ]
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.Load)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.Upload ] []
                                    span [] [ str " " ]
                                    Html.text "Load"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let gameView addon (state: Game.State<Text, 'LabelName, 'Addon, 'Arg>) dispatch =
    let print (xs: ReactElement list) =
        Content.content [] xs

    match state.Game with
    | AbstractEngine.Print(xs, _) ->
        Html.div [
            prop.children [
                print xs

                Html.div [
                    prop.style [
                        style.justifyContent.center
                        style.display.flex
                    ]
                    prop.children [
                        Button.button [
                            Button.OnClick (fun _ -> dispatch Game.Next)
                        ] [
                          str "..."
                        ]
                    ]
                ]
            ]
        ]
    | AbstractEngine.End ->
        Html.div [
            prop.style [
                style.justifyContent.center
                style.display.flex
            ]
            prop.text "Конец"
        ]
    | AbstractEngine.Choices(caption, choices, _) ->
        let xs =
            choices
            |> List.mapi (fun i label ->
                Html.div [
                    prop.style [
                        style.justifyContent.center
                        style.display.flex
                    ]
                    prop.children [
                        Button.button [
                            Button.OnClick (fun _ -> dispatch (Game.Choice i))
                        ] [
                          str label
                        ]
                    ]
                ]
            )
        Html.div [
            prop.children (print caption :: xs)
        ]
    | AbstractEngine.AddonAct(arg, _) ->
        addon arg state dispatch
    | AbstractEngine.NextState _ ->
        failwith "failwith NextState"

let view addon (state: Game.State<Text, 'LabelName, 'Addon, 'Arg>) (dispatch: Game.Msg<'Addon, 'Arg> -> unit) =
    Html.div [
        prop.children [
            nav dispatch

            Html.section [
                prop.style [
                    style.padding 20
                ]
                prop.children [
                    Column.column [
                        Column.Width (Screen.All, Column.Is6)
                        Column.Offset (Screen.All, Column.Is3)
                    ] [
                        Box.box' [] [gameView addon state dispatch]
                    ]
                ]
            ]
        ]
    ]
