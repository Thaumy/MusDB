module App.CLI

open System
open System.Text

type CLI =

    static member InColor color todo : unit =
        Console.ForegroundColor <- color
        todo ()
        Console.ForegroundColor <- ConsoleColor.White


    static member InPosition L R todo : unit =
        Console.SetCursorPosition(L, R)
        todo ()

    static member InRight(text: string) : unit =
        Console.SetCursorPosition(
            Console.WindowWidth
            - Encoding.Default.GetByteCount(text),
            Console.CursorTop
        )

        Console.Write text

    static member Pause(text: 'T) =
        Console.Write text
        Console.ReadLine()


    static member Put(text: 'T) : unit = Console.Write text

    static member Line(text: 'T) : unit = Console.WriteLine text
