namespace GameCore

type RunState = {
    elapsed: float
    keyboard: KeyboardInfo
} with
    member __.WasJustPressed key = List.contains key __.keyboard.keysDown
    