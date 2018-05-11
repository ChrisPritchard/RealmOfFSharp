module Controller

open GameCore
open Model
open Microsoft.Xna.Framework.Input

let initialBattle = {
    player = initialPlayer
    orcs = [1..6] |> List.map (fun _ -> getOrc)
    state = turnStart
}

let updateModel (runState: RunState) currentBattle = 
    match currentBattle with
    | None -> Some initialBattle
    | Some battle ->
        match battle.state with
        | GameOver -> currentBattle
        | PlayerTurn playerState ->
            if runState.WasJustPressed Keys.Left then
                Some battle
            else Some battle
        | OrcTurn orcIndex -> 
            Some battle