module FullCalendarBindings

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.Core.JS

[<ImportDefault("@fullcalendar/react")>]
let FullCalendarComponent: obj = jsNative

[<ImportDefault("@fullcalendar/daygrid")>]
let dayGridPlugin: obj = jsNative

[<ImportDefault("@fullcalendar/interaction")>]
let interactionPlugin: obj = jsNative

[<Import("Draggable", "@fullcalendar/interaction")>]
let Draggable: obj  = jsNative

[<ImportDefault("@fullcalendar/timegrid")>]
let timeGridPlugin: obj = jsNative

[<Import("default", "@fullcalendar/core/locales/de")>]
let germanLocale: obj = jsNative



type CalendarEvent = {
    Id : int option
    Title: string
    Start: string
    End: string
}

type CalendarSetup =
    { Events : CalendarEvent list
      CreateEventF : obj -> unit
      MoveEventF : obj -> unit
      DeleteEventF : obj -> unit
      EditEventF : obj -> unit
    }


let calendarComponent (setup : CalendarSetup) =
    let calendarEvents =
        setup.Events
        |> List.map (fun event ->
            createObj [
                "title" ==> event.Title
                "start" ==> event.Start
                "end" ==> event.End
                "id" ==> event.Id
            ])
        |> List.toArray

    let calendarProps =
        createObj [
            "plugins" ==> [| dayGridPlugin; timeGridPlugin; interactionPlugin |]
            "initialView" ==> "timeGridWeek"
            "editable" ==> true
            "droppable" ==> true
            "events" ==> calendarEvents

            "eventDrop" ==> (fun (info: obj) ->
                console.log("Event moved:", info)
                setup.MoveEventF info |> ignore
            )

            "drop" ==> (fun (info: obj) ->
                console.log("External element dropped:", info)
                setup.CreateEventF info |> ignore
            )
            "locale" ==> "de"
            "locales" ==> [| germanLocale |]
            "slotMinTime" ==> "06:00"
            "slotMaxTime" ==> "22:00"
        ]
        

    ReactBindings.React.createElement(FullCalendarComponent, calendarProps, [])
