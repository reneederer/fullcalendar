module Index

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Elmish
open Elmish.React
open Feliz
open type Feliz.Html
open FullCalendarBindings
open Fable.Core.JS
open Fable.SimpleJson

type Model = {
    Events: CalendarEvent list
}

type Msg =
    | NoOp
    | CreateEvent of obj
    | MoveEvent of obj

let init () : Model * Cmd<Msg> =
    let sampleEvents =
      [ { Id = Some 3; Title = "Team Meeting"; Start = "2025-05-22"; End = "2025-05-22" }
        { Id = Some 4; Title = "Conference"; Start = "2025-05-23"; End = "2025-05-25" }
      ]
    { Events = sampleEvents }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | NoOp -> model, Cmd.none



let CalendarWithDraggable (model: Model) =
    React.functionComponent(fun () ->
        React.useEffectOnce(fun () ->
            let draggable =
                createNew Draggable
                    (Browser.Dom.document.getElementById("external-events"),
                        createObj [
                            "itemSelector" ==> ".draggable"
                            "eventData" ==>
                                fun (el: obj) ->
                                    console.log el
                                    let json = el?getAttribute "data-event"
                                    createObj [ "title" ==> $"""{json}""" ]
                        ])
            None
        )

        Html.div [
            calendarComponent
                { Events = model.Events
                  CreateEventF =
                    (fun info ->
                        console.log($"CreateEvent", info)
                    )
                  MoveEventF =
                    (fun info ->
                        console.log($"MoveEvent", info)
                    )
                  DeleteEventF =
                    (fun info ->
                        console.log($"DeleteEvent", info)
                    )
                  EditEventF =
                    (fun info ->
                        console.log($"EditEvent", info)
                    )
                }
        ]
    )



let view model dispatch =
    React.fragment
      [ Html.div [
            prop.id "external-events"
            prop.children [
                Html.h1 [
                    prop.text "My Calendar"
                    prop.id "my-calendar"
                    prop.classes [ "draggable" ]
                    prop.draggable true
                    prop.style [
                        style.cursor.pointer
                        style.userSelect.none
                        style.padding 10
                        style.backgroundColor "lightblue"
                        style.display.inlineBlock
                    ]
                    prop.custom("data-event", """{ "title": "My Calendar" }""")
                ]
                Html.h1 [
                    prop.text "My Calendar2"
                    prop.id "my-calendar2"
                    prop.classes ["draggable"]
                    prop.draggable true
                    prop.style [
                        style.cursor.pointer
                        style.userSelect.none
                        style.padding 10
                        style.backgroundColor "lightblue"
                        style.display.inlineBlock
                    ]
                    prop.custom("data-event", """{ "title": "My Calendar2" }""")
                ]
            ]
        ]
        CalendarWithDraggable model ()
      ]
