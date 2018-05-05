module MyGameEntry

(*
    Implementation of 5.4 Guessing Game with a GUI. 
    Uses a monogame library I built that allows the game loop to be orchestrated.
    By so doing, this class (Program.fs) is completely immutable and almost entirely
    (except for the Keys enum and a reference to the font asset) MonoGame agnostic
*)

open GameWrapper
open Microsoft.Xna.Framework.Input

let assets = [
    { key = "default"; assetType = AssetType.Font; path = "Content/JuraMedium" }
]

type CountingGameState = {
    guess: int
    upper: int;
    lower: int;
    guesses: int;
    win: bool;
}

let initialState = { guess = 50; upper = 100; lower = 0; guesses = 1; win = false }

let updateState (runState: RunState) gameState =
    let isPressed key = List.contains key runState.keyboard.keysDown

    if isPressed Keys.Y && gameState.win then
        initialState
    elif isPressed Keys.C then
        { gameState with win = true }
    elif isPressed Keys.Down then
        let newGuess = (gameState.guess - gameState.lower) / 2 + gameState.lower
        { gameState with guesses = gameState.guesses + 1; upper = gameState.guess; guess = newGuess }
    elif isPressed Keys.Up then
        let newGuess = (gameState.upper - gameState.guess) / 2 + gameState.guess
        { gameState with guesses = gameState.guesses + 1; lower = gameState.guess; guess = newGuess }
    else
        gameState

let getView gameState = 
    let baseText = { fontKey = "default"; text = ""; position = (0.0,0.0); scale = 0.4 }
    if gameState.win then
        [
            { baseText with text = "Excellent!"; position = (50.0,50.0); scale = 0.8 };
            { baseText with text = sprintf "I took %i guesses" gameState.guesses; position = (50.0,100.0) };
            { baseText with text = "Press 'Y' to play again"; position = (50.0,130.0) }
        ]
    else
        [
            { baseText with text = sprintf "My guess is %i" gameState.guess; position = (50.0,50.0) };
            { baseText with text = "Press Up if too low, Down if too high, or 'C' if correct"; position = (50.0,80.0); scale = 0.3 };
        ]

[<EntryPoint>]
let main _ =
    let config = { loadAssets = assets; initialState = initialState; updateState = updateState; getView = getView }
    use game = new GameWrapper<CountingGameState> (config)
    game.Run()
    0