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
open Browser
open System
open FSharp.Collections

type Model = {
    Events: Map<EventId, CalendarEvent>
}

type Msg =
    | NoOp
    | CreateEvent of CalendarEvent
    | MoveEvent of EventId * TimeSpan
    | EditEvent of EventId

let init () : Model * Cmd<Msg> =
    let events =
      [ 3, { Title = "Team Meeting"; Start = DateTime.Parse "2025-05-22T11:00:00"; End = DateTime.Parse "2025-05-22T13:00:00" }
      ] |> Map.ofList
    { Events = events }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | NoOp -> model, Cmd.none
    | CreateEvent event ->
        window.alert (SimpleJson.stringify event)
        model, Cmd.none
    | MoveEvent (event, delta) ->
        window.alert (SimpleJson.stringify event)
        window.alert (SimpleJson.stringify delta.TotalDays)
        window.alert (SimpleJson.stringify delta.TotalMinutes)
        model, Cmd.none
    | EditEvent eventId ->
        window.alert eventId
        model, Cmd.none



let CalendarWithDraggable (model: Model) dispatch =
    React.functionComponent(fun () ->
        React.useEffectOnce(fun () ->
            createNew Draggable
                (Browser.Dom.document.getElementById("external-events"),
                    createObj [
                        "itemSelector" ==> ".draggable"
                        "eventData" ==>
                            fun (el: obj) ->
                                console.log el
                                createObj [ "title" ==> $"""{el?getAttribute "data-event-title"} ({el?getAttribute "data-event-typ"})""" ]
                    ])
            |> ignore
        )

        Html.div [
            calendarComponent
                { Events = model.Events
                  CreateEventF =
                    (fun info ->
                        console.log($"CreateEvent", info)
                        let draggedEl = info?draggedEl
                        let event =
                            { Title = draggedEl?getAttribute "data-event-title"
                              Start = DateTime.Parse info?dateStr
                              End = DateTime.Parse info?dateStr
                            }
                        dispatch <|
                            CreateEvent event
                    )
                  MoveEventF =
                    (fun info ->
                        console.log($"MoveEvent", info)
                        dispatch <|
                            MoveEvent
                                ( info?event?_def?publicId,
                                  (TimeSpan.FromMilliseconds info?delta?milliseconds).Add(TimeSpan.FromDays info?delta?days)
                                )
                    )
                  DeleteEventF =
                    (fun info ->
                        console.log($"DeleteEvent", info)
                    )
                  EditEventF =
                    (fun info ->
                        console.log($"EditEvent", info)
                    )
                  ClickEventF =
                    (fun info ->
                        console.log($"ClickEvent", info)
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
                    prop.custom("data-event-title", """My Calendar""")
                    prop.custom("data-event-typ", """Investitionskosten""")
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
                    prop.custom("data-event-title", """My Calendar2""")
                    prop.custom("data-event-typ", """Pflegeleistung""")
                ]
            ]
        ]
        CalendarWithDraggable model dispatch ()
      ]
