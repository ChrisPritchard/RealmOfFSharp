open GameCore
open Microsoft.Xna.Framework.Input

(*
    Implementation of 5.4 Guessing Game with a GUI. 
    Uses a monogame library I built that allows the game loop to be orchestrated.
    By so doing, this class (Program.fs) is completely immutable and almost entirely
    (except for the Keys enum and a reference to the font asset) MonoGame agnostic
*)

let assets = [
    { key = "default"; assetType = AssetType.Font; path = "Content/JuraMedium" }
]

type CountingGameModel = {
    guess: int
    upper: int;
    lower: int;
    guesses: int;
    win: bool;
}

let gameStartModel = { guess = 50; upper = 100; lower = 0; guesses = 1; win = false }

let updateModel (runState: RunState) currentModel =
    match currentModel with
    | None -> Some gameStartModel
    | Some gameState ->
        let isPressed = runState.WasJustPressed

        if isPressed Keys.Y && gameState.win then
            Some gameStartModel
        elif isPressed Keys.Escape then
            None
        elif isPressed Keys.C then
            Some { gameState with win = true }
        elif isPressed Keys.Down then
            let newGuess = (gameState.guess - gameState.lower) / 2 + gameState.lower
            Some { gameState with guesses = gameState.guesses + 1; upper = gameState.guess; guess = newGuess }
        elif isPressed Keys.Up then
            let newGuess = (gameState.upper - gameState.guess) / 2 + gameState.guess
            Some { gameState with guesses = gameState.guesses + 1; lower = gameState.guess; guess = newGuess }
        else
            Some gameState

let getView _ model = 
    let baseText = { fontKey = "default"; text = ""; position = (0,0); scale = 0.4 }
    let text = 
        if model.win then
            [
                { baseText with text = "Excellent!"; position = (50,50); scale = 0.8 };
                { baseText with text = sprintf "I took %i guesses" model.guesses; position = (50,100) };
                { baseText with text = "Press 'Y' to play again"; position = (50,130) }
            ]
        else
            [
                { baseText with text = sprintf "My guess is %i" model.guess; position = (50,50) };
                { baseText with text = "Press Up if too low, Down if too high, or 'C' if correct"; position = (50,80); scale = 0.3 };
            ]
    [], text

[<EntryPoint>]
let main _ =
    use game = new GameCore<CountingGameModel>(assets, updateModel, getView)
    game.Run()
    0