module CLI

open System

type CLI =

    member this.InColor color todo =
        let _d = Console.ForegroundColor = color
        todo ()
        Console.ForegroundColor = ConsoleColor.White

    member this.InPosition left right todo =
        Console.SetCursorPosition(left, right)
        todo ()

    member this.Pause = Console.ReadLine()
    member this.Line(value: 'T) = Console.WriteLine(value)

    member this.Put(value: 'T) = Console.Write(value)
