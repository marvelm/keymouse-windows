open Gma.System.MouseKeyHook
open System.Windows.Forms
open System.Drawing
open System.Runtime.InteropServices
open WindowsInput

module Native =
    open System

    [<DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)>]
    extern IntPtr GetDesktopWindow();

    [<DllImport("user32.dll", CharSet = CharSet.Auto)>]
    extern IntPtr FindWindow([<MarshalAs(UnmanagedType.LPTStr)>] string lpClassName,[<MarshalAs(UnmanagedType.LPTStr)>] string lpWindowName);

    [<DllImport("user32.dll")>]
    extern IntPtr SetParent(
          IntPtr hWndChild,      // handle to window
          IntPtr hWndNewParent   // new parent window
    );

    [<DllImport("user32.dll", SetLastError = true)>]
    extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [<DllImport("user32.dll", SetLastError = true)>]
    extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    let GWL_EXSTYLE: int = -20;
    let WS_EX_TRANSPARENT: int = 0x20; 

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

let contains x elements = List.exists (fun el -> x = el) elements

let wasd = [Keys.W; Keys.A; Keys.S; Keys.D]
let clickKeys = [Keys.OemSemicolon; Keys.OemQuotes]
let scrollKeys = [Keys.F; Keys.V]
let hotkeys = wasd @ clickKeys @ scrollKeys

let onKeyDown (e: KeyEventArgs) =
    if enabled && contains e.KeyCode hotkeys
    then
        if (contains e.KeyCode wasd) && (e.Modifiers <> Keys.None) then
            ()
        else 
            match e.KeyCode with
            | Keys.W -> moveMouse (fun x y -> (x, y - 5))
            | Keys.A -> moveMouse (fun x y -> (x - 5, y))
            | Keys.S -> moveMouse (fun x y -> (x, y + 5))
            | Keys.D -> moveMouse (fun x y -> (x + 5, y))
            | Keys.OemSemicolon -> mouse.LeftButtonClick() |> ignore
            | Keys.OemQuotes -> mouse.RightButtonClick() |> ignore
            | Keys.F -> mouse.VerticalScroll(1) |> ignore
            | Keys.V -> mouse.VerticalScroll(-1) |> ignore

            e.SuppressKeyPress <- true

    else if contains e.KeyCode [Keys.OemPipe; Keys.OemBackslash]
    then
       if enabled then e.SuppressKeyPress <- true
       enabled <- not enabled
       System.Console.Clear()
       printfn "Enabled: %A" enabled

[<EntryPoint>]
let main argv =
    let hook = Hook.GlobalEvents();
    hook.MouseMove.Add onMouseMove
    hook.KeyDown.Add onKeyDown

    Application.Run();
    0 // return an integer exit code