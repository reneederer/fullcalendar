module FullCalendarBindings

open Fable.Core
open Fable.Core.JsInterop
open Fable.React

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
    title: string
    start: string
    ``end``: string
}
