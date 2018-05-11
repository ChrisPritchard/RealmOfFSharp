module Controller

open GameCore
open Model
open Microsoft.Xna.Framework.Input

let updateModel (runState: RunState) battle = 
    match battle.state with
    | GameOver -> battle
    | PlayerTurn playerState ->
        if runState.WasJustPressed Keys.Left then
            battle
        else battle
    | OrcTurn orcIndex -> 
        battle