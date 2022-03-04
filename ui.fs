module ui

open System
open System.Text

let inColor color todo =
    Console.ForegroundColor <- color
    let result = todo ()
    Console.ForegroundColor <- ConsoleColor.White
    result

let inCoordinate L R todo : unit =
    Console.SetCursorPosition(L, R)
    todo ()

let inRight (text: string) : unit =
    Console.SetCursorPosition(
        Console.WindowWidth
        - Encoding.Default.GetByteCount(text),
        Console.CursorTop
    )

    Console.Write text

let pause (text: 'T) =
    Console.Write text
    Console.ReadLine()


let put (text: 'T) : unit = Console.Write text

let line (text: 'T) : unit = Console.WriteLine text

let newLine () = Console.WriteLine ""
