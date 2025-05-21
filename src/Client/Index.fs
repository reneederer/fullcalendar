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

type CalendarEvent = {
    Title: string
    Start: string
    End: string
}

type Model = {
    Events: CalendarEvent list
}

type Msg =
    | NoOp

let init () : Model * Cmd<Msg> =
    let sampleEvents = [
        { Title = "Team Meeting"; Start = "2025-05-22"; End = "2025-05-22" }
        { Title = "Conference"; Start = "2025-05-23"; End = "2025-05-25" }
    ]
    { Events = sampleEvents }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | NoOp -> model, Cmd.none

let calendarComponent (events: CalendarEvent list) =
    let calendarEvents =
        events
        |> List.map (fun ev ->
            createObj [
                "title" ==> ev.Title
                "start" ==> ev.Start
                "end" ==> ev.End
            ])
        |> List.toArray

    let calendarProps =
        createObj [
            "plugins" ==> [| dayGridPlugin; timeGridPlugin; interactionPlugin |]
            "initialView" ==> "timeGridWeek"
            "editable" ==> true
            "droppable" ==> true
            "events" ==> [|
                createObj [ "title" ==> "Meeting"; "start" ==> "2025-05-22" ]
                createObj [ "title" ==> "Workshop"; "start" ==> "2025-05-23" ]
            |]
            // optional callbacks
            "eventDrop" ==> (fun (info: obj) ->
                console.log("Event moved:", info)
            )
            "drop" ==> (fun (info: obj) ->
                console.log("External element dropped:", info)
            )
            "locale" ==> "de"
            "locales" ==> [| germanLocale |]
            "slotMinTime" ==> "06:00"
            "slotMaxTime" ==> "22:00"
        ]
        

    ReactBindings.React.createElement(FullCalendarComponent, calendarProps, [])

let view model dispatch =
    
    let onDragStart (ev: Browser.Types.DragEvent) =
        ev.dataTransfer.setData("text/plain", "My Calendar Drag")

        // Create a transparent 1x1 pixel canvas to use as drag image
        let canvas = Browser.Dom.document.createElement("canvas") :?> Browser.Types.HTMLCanvasElement
        canvas.width <- 1
        canvas.height <- 1
        let ctx = canvas.getContext_2d()
        ctx.fillStyle <- U3.Case1 "rgba(0,0,0,0)"
        ctx.fillRect(0., 0., 1., 1.)

        ev.dataTransfer.setDragImage(canvas, 0, 0)

        printfn "Drag started!"

    let onDragEnd (ev: Browser.Types.DragEvent) =
        printfn "Drag ended!"

    div
      [ div [
            h1 [
                prop.text "My Calendar"
                prop.draggable true
                prop.onDragStart onDragStart
                prop.onDragEnd onDragEnd
                prop.style [
                    style.cursor.pointer
                    style.userSelect.none
                    style.padding 10
                    style.backgroundColor "lightblue"
                    style.display.inlineBlock
                ]
            ]
        ]
        calendarComponent model.Events
      ]
