module App.CLI

open System


type CLI =

    static member InColor color todo : unit =
        Console.ForegroundColor <- color
        todo ()
        Console.ForegroundColor <- ConsoleColor.White


    static member InPosition L R todo : unit =
        Console.SetCursorPosition(L, R)
        todo ()

    static member Pause(text: 'T) : unit =
        Console.Write text
        Console.ReadLine() |> ignore

    static member Line(text: 'T) : unit = Console.WriteLine text

    static member Put(text: 'T) : unit = Console.Write text
