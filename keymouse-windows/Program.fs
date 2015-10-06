open Gma.System.MouseKeyHook
open System.Windows.Forms
open System.Drawing

let mutable enabled = true
let mutable mousePos =
    Cursor.Position <- new Point(0, 0);
    new System.Drawing.Point(0, 0)

let onMouseMove (e: MouseEventArgs) =
    mousePos <- e.Location
    printfn "%A" mousePos

[<EntryPoint>]
let main argv = 
    let hook = Hook.GlobalEvents();
    hook.MouseMove.Add onMouseMove

    Application.Run()
    0 // return an integer exit code