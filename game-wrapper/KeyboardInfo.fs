namespace GameWrapper
open Microsoft.Xna.Framework.Input

type KeyboardInfo = {
    pressed: Keys list;
    keysDown: Keys list;
    keysUp: Keys list
}