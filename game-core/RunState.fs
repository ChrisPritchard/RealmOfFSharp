namespace GameCore
open Microsoft.Xna.Framework.Input

type RunState = {
    elapsed: float
    keyboard: KeyboardInfo
    mouse: MouseInfo
} and KeyboardInfo = {
    pressed: Keys list;
    keysDown: Keys list;
    keysUp: Keys list
} and MouseInfo = {
    position: int * int
    pressed: bool * bool
}
    
type RunState with
    member __.WasJustPressed key = List.contains key __.keyboard.keysDown