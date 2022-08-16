#r "nuget:Spectre.Console,0.44.0"

open Spectre.Console
open System
open System.Diagnostics
open System.Threading
open System.IO

let font = FigletFont.Load "./colossal.flf"
let cursorSymbol = "_"

let write (color : Color) (text : string) =
    FigletText(font, text)
    |> (fun p -> FigletTextExtensions.Color(p, color))
    |> AnsiConsole.Console.Write

let mutable text = ""

let p = Process()
p.StartInfo.UseShellExecute <- false
p.StartInfo.RedirectStandardOutput <- true
p.StartInfo.RedirectStandardInput <- true
p.StartInfo.RedirectStandardError <- true


p.StartInfo.FileName <- "bash"
p.Start()

let rec printFrom (reader : StreamReader, color : Color) () =
    let line = reader.ReadLine ()
    if line <> null then
        write color line
        printFrom (reader, color) ()

Thread(ThreadStart (printFrom(p.StandardOutput, Color.Lime))).Start()
Thread(ThreadStart (printFrom(p.StandardError, Color.Pink3))).Start()

let rec run (): unit =
    let key = Console.ReadKey ()

    match int key.KeyChar with
    | 127 ->
        if text <> "" then
            text <- text.Substring(0, text.Length - 1)
        Console.Clear ()
        write Color.White $"$ {text}{cursorSymbol}"

    | 13 ->
        Console.Clear ()
        write Color.White $"$ {text}"
        p.StandardInput.WriteLine($"{text}\n")
        p.StandardInput.Flush ()
        text <- ""

    | other ->
        text <- text + key.KeyChar.ToString() 
        Console.Clear ()
        write Color.White $"$ {text}{cursorSymbol}"
    run ()

Console.Title <- "Scream Shell"
Console.Clear ()
Console.CursorVisible <- false
write Color.White $"$ {cursorSymbol}"
run()
