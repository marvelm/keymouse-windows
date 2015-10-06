open Gma.System.MouseKeyHook
open System.Windows.Forms
open System.Drawing
open System.Runtime.InteropServices
open WindowsInput

let mutable enabled = true
let mutable mousePos =
    let p = new Point(0, 0);
    Cursor.Position <- p 
    p

let onMouseMove (e: MouseEventArgs) =
    mousePos <- e.Location

let mouse = new MouseSimulator(new InputSimulator())

let moveMouse f =
    if enabled
    then
        let (x, y) = f mousePos.X mousePos.Y
        let x' = if x < 0 then 0 else x
        let y' = if y < 0 then 0 else y

        mousePos <- new Point(x', y')
        Cursor.Position <- mousePos

let onKeyDown (e: KeyEventArgs) =
    if enabled && List.exists (fun key -> key = e.KeyCode) [Keys.W; Keys.A; Keys.S; Keys.D; Keys.OemSemicolon; Keys.OemQuotes]
    then
        match e.KeyCode with
        | Keys.W -> moveMouse (fun x y -> (x, y - 5))
        | Keys.A -> moveMouse (fun x y -> (x - 5, y))
        | Keys.S -> moveMouse (fun x y -> (x, y + 5))
        | Keys.D -> moveMouse (fun x y -> (x + 5, y))
        | Keys.OemSemicolon -> mouse.LeftButtonClick() |> ignore
        | Keys.OemQuotes -> mouse.RightButtonClick() |> ignore

        e.SuppressKeyPress <- true

    else if e.KeyCode = Keys.OemPipe || e.KeyCode = Keys.OemBackslash
    then
       if enabled then e.SuppressKeyPress <- true
       enabled <- not enabled
       printfn "Enabled: %A" enabled

[<EntryPoint>]
let main argv = 
    let hook = Hook.GlobalEvents();
    hook.MouseMove.Add onMouseMove
    hook.KeyDown.Add onKeyDown

    Application.Run()
    0 // return an integer exit code