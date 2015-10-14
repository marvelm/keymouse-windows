# keymouse-windows

`keymouse-windows` allows you to control your mouse with your keyboard.

The primary difference between this project and `keymouse` is that the
latter isn't able to supress the keystrokes used for moving the cursor.
It is difficult to implement this feature in a platform agnostic way
with regard to OSX and seemingly impossible for X.

There is also a significant reduction in memory usage because it runs
on .NET and does not need to load the Clojure jar. I have not yet benchmarked
the application.

### Usage

Use the "WASD" keys to move the mouse,

"\" to toggle the application,

";" to left click, "'" (single quote) to right click,

'f' to scroll up and 'v' to scroll down.