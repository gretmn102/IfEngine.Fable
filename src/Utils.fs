module IfEngine.Fable.Utils
open Feliz

open IfEngine.SyntaxTree

let divCenter (xs: seq<Fable.React.ReactElement>) =
    Html.div [
        prop.style [
            style.justifyContent.center
            style.display.flex
        ]

        prop.children xs
    ]

let say (txt: string) =
    Html.p [
        prop.style [
            // style.justifyContent.center
            // style.display.flex
        ]
        prop.text txt
    ]
    |> List.singleton
    |> Say

let says (xs: string list) =
    xs
    |> List.map (fun str ->
        Html.p [
            prop.style [
                // style.justifyContent.center
                // style.display.flex
            ]
            prop.children [
                Html.text str
            ]
        ]
    )
    |> Say

let sayImg (text: string) imgSrc =
    Say [
        Html.p [
            prop.text text
        ]

        Html.img [
            prop.src imgSrc
        ]
    ]

let interSay (getText: VarsContainer -> string) =
    InterpolationSay (fun vars ->
        Html.p [
            prop.text (getText vars)
        ]
        |> List.singleton
    )
