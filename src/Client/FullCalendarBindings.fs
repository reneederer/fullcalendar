module FullCalendarBindings

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.Core.JS
open System
open FSharp.Collections
open Feliz
open type Feliz.Html

[<ImportDefault("@fullcalendar/react")>]
let FullCalendarComponent: obj = jsNative

[<ImportDefault("@fullcalendar/daygrid")>]
let dayGridPlugin: obj = jsNative

[<ImportDefault("@fullcalendar/interaction")>]
let interactionPlugin: obj = jsNative

[<Import("Draggable", "@fullcalendar/interaction")>]
let Draggable: obj = jsNative

[<ImportDefault("@fullcalendar/timegrid")>]
let timeGridPlugin: obj = jsNative

[<Import("default", "@fullcalendar/core/locales/de")>]
let germanLocale: obj = jsNative

type EventId = int

type CalendarEvent =
    {
        Title: string
        Start: DateTime
        End: DateTime
        ParentId: EventId option
    }

type CalendarSetup =
    {
        Events: Map<EventId, CalendarEvent>
        CreateEventF: obj -> unit
        MoveEventF: obj -> unit
        DeleteEventF: obj -> unit
        EditEventF: obj -> unit
        ClickEventF: obj -> unit
    }

let eventContentRenderer (arg : obj) =
    let title: string = arg?event?title
    let isSub: bool = not (isNullOrUndefined (arg?event?extendedProps?parentId))

    let content =
        if isSub then
            Html.div
              [ prop.style
                  [
                      style.marginLeft 12
                      style.borderLeft (2, borderStyle.solid, "green")
                      style.paddingLeft 8
                      style.color "green"
                      style.fontSize (length.em 0.9)
                  ]
                prop.text title
              ]
        else
            console.log arg
            Html.div
                [
                    prop.title "dein tooltip"
                    prop.children
                      [ Html.strong
                          [ prop.style [ style.fontSize (length.em 1) ]
                            prop.text title
                          ]
                        br []
                        text (string arg?timeText)
                      ]
                ]

    content

let calendarComponent (setup: CalendarSetup) =
    let calendarEvents =
        setup.Events
        |> Map.map (fun eventId event ->
            createObj
                [
                    "id" ==> eventId
                    "title" ==> event.Title
                    "start" ==> event.Start.ToString("o")
                    "end" ==> event.End.ToString("o")
                    "parentId"
                    ==> match event.ParentId with
                        | Some parentId -> parentId
                        | None -> undefined
                ])
        |> Map.values
        |> Seq.toArray

    let calendarProps =
        createObj
            [
                "plugins" ==> [| dayGridPlugin; timeGridPlugin; interactionPlugin |]
                "initialView" ==> "timeGridWeek"
                "editable" ==> true
                "droppable" ==> true
                "events" ==> calendarEvents

                "eventDrop"
                ==> (fun (info: obj) ->
                    console.log ("Event moved:", info)
                    setup.MoveEventF info |> ignore)

                "drop"
                ==> (fun (info: obj) ->
                    console.log ("External element dropped:", info)
                    setup.CreateEventF info |> ignore)

                "eventClick"
                ==> (fun (info: obj) ->
                    let publicId = info?event?_def?publicId
                    console.log ($"Click ({publicId}):", info)
                    setup.ClickEventF publicId |> ignore)
                "eventContent" ==> (fun x -> eventContentRenderer x)
                "locale" ==> "de"
                "locales" ==> [| germanLocale |]
                "slotDuration" ==> "00:15:00"
                "slotLabelInterval" ==> "00:60:00"
                "slotMinTime" ==> "06:00"
                "slotMaxTime" ==> "22:00"
            ]

    ReactBindings.React.createElement (FullCalendarComponent, calendarProps, [])