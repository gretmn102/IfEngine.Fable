namespace IfEngine.Fable.SyntaxTree

type Content = Fable.React.ReactElement list
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Content =
    open IfEngine.SyntaxTree
    open Farkdown.Experimental.SyntaxTree

    module Farkdown =
        module Markdown =
            open Feliz
            open FsharpMyExtension
            open Farkdown.Experimental.SyntaxTree

            module Html =
                let hx level (xs: Fable.React.ReactElement list) =
                    match level with
                    | 1 -> Html.h1 xs
                    | 2 -> Html.h2 xs
                    | 3 -> Html.h3 xs
                    | 4 -> Html.h4 xs
                    | 5 -> Html.h5 xs
                    | 6 -> Html.h6 xs
                    | _ -> Html.h6 xs

                let htmlList isOrdered (xs: Fable.React.ReactElement list) =
                    if isOrdered then
                        Html.orderedList xs
                    else
                        Html.unorderedList xs

            module LineElement =
                let rec toReact (lineElement: LineElement) : Fable.React.ReactElement =
                    match lineElement with
                    | LineElement.Bold lineElement ->
                        Html.strong (
                            toReact lineElement
                        )
                    | LineElement.Italic lineElement ->
                        Html.em (
                            toReact lineElement
                        )
                    | LineElement.Strikeout lineElement ->
                        Html.del (
                            toReact lineElement
                        )
                    | LineElement.Underline lineElement ->
                        Html.u (
                            toReact lineElement
                        )
                    | LineElement.Text s ->
                        Html.text s
                    | LineElement.Comment lineElement ->
                        Html.none
                    | LineElement.Url(href, title, body) ->
                        Html.a [
                            prop.href href
                            prop.title title
                            prop.children (List.map toReact body)
                        ]
                    | LineElement.Image(src, title, alt) ->
                        Html.img [
                            prop.src src
                            prop.title title
                            prop.alt alt
                        ]

            module Line =
                let toReact (line: Line) : Fable.React.ReactElement list =
                    line
                    |> List.map LineElement.toReact

            module ListItem =
                let toReact statementToReact ((line, body): ListItem) : Fable.React.ReactElement =
                    let item =
                        Html.p (
                            Line.toReact line
                        )
                    let body = List.collect statementToReact body
                    Html.li (
                        item :: body
                    )

            module Statement =
                let rec toReact (statement: Statement) : Fable.React.ReactElement list =
                    match statement with
                    | Statement.Paragraph lines ->
                        Html.p (
                            lines
                            |> List.map Line.toReact
                            |> List.sepBy [ Html.br [] ]
                            |> List.concat
                        )
                        |> List.singleton
                    | Statement.Header(level, line, body) ->
                        let header =
                            Html.hx level (
                                Line.toReact line
                            )
                        let rest =
                            body |> List.collect toReact
                        header::rest
                    | Statement.Comment(_) -> []
                    | Statement.List(isOrdered, items) ->
                        items
                        |> List.map (ListItem.toReact toReact)
                        |> Html.htmlList isOrdered
                        |> List.singleton

    let ofCommon (content: CommonContent.Content) : Content =
        content
        |> List.collect (
            Farkdown.Markdown.Statement.toReact
        )
