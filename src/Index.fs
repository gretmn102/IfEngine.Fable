module IfEngine.Fable.Index
open Fable.React
open Fable.FontAwesome
open Fulma
open Feliz
open IfEngine

open IfEngine.Fable.WebEngine

let nav dispatch =
    Html.div [
        Tabs.tabs [
            Tabs.IsCentered
        ] [
            Html.ul [
                prop.children [
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch InputMsg.NewGame)
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
                            prop.onClick (fun _ -> dispatch InputMsg.Save)
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
                            prop.onClick (fun _ -> dispatch InputMsg.Load)
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

let gameView
    handleCustomStatement
    (state: WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
    (dispatch: InputMsg<'CustomStatementArg> -> unit) =

    let print (xs: ReactElement list) =
        Content.content [] xs

    match WebEngine.getCurrentOutputMsg state with
    | OutputMsg.OutputMsgCore coreMsg ->
        match coreMsg with
        | Engine.OutputMsg.Print xs ->
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
                                Button.OnClick (fun _ ->
                                    dispatch (InputMsg.InputMsgCore Engine.InputMsg.Next)
                                )
                            ] [
                                str "..."
                            ]
                        ]
                    ]
                ]
            ]
        | Engine.OutputMsg.End ->
            Html.div [
                prop.style [
                    style.justifyContent.center
                    style.display.flex
                ]
                prop.text "Конец"
            ]
        | Engine.OutputMsg.Choices(caption, choices) ->
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
                                Button.OnClick (fun _ ->
                                    dispatch (InputMsg.InputMsgCore (Engine.InputMsg.Choice i))
                                )
                            ] [
                                str label
                            ]
                        ]
                    ]
                )
            Html.div [
                prop.children (print caption :: xs)
            ]
        | Engine.OutputMsg.CustomStatement customStatementOutput ->
            handleCustomStatement customStatementOutput

let view
    handleCustomStatement
    (state: WebEngine<'Label, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
    (dispatch: InputMsg<'CustomStatementArg> -> unit) =

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
                        Box.box' [] [ gameView handleCustomStatement state dispatch ]
                    ]
                ]
            ]
        ]
    ]
