module CLI

open System


type CLI =

    static member InColor color todo : unit =
        let _d = Console.ForegroundColor = color
        todo ()

        Console.ForegroundColor = ConsoleColor.White
        |> ignore


    static member InPosition left right todo : unit =
        Console.SetCursorPosition(left, right)
        todo ()

    static member Pause : unit = Console.ReadLine() |> ignore

    static member Pause(text: 'T) : unit =
        Console.Write(text)
        Console.ReadLine() |> ignore

    static member Line(text: 'T) : unit = Console.WriteLine(text)

    static member Put(text: 'T) : unit = Console.Write(text)
