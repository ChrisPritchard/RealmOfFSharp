namespace GameCore
open Microsoft.Xna.Framework.Input

type RunState = {
    elapsed: float
    keyboard: KeyboardInfo
} and KeyboardInfo = {
    pressed: Keys list;
    keysDown: Keys list;
    keysUp: Keys list
}
    
type RunState with
    member __.WasJustPressed key = List.contains key __.keyboard.keysDown