module Controller

open GameCore
open Model
open Microsoft.Xna.Framework.Input

let initialBattle = {
    player = initialPlayer
    orcs = [1..6] |> List.map (fun _ -> getOrc)
    state = turnStart
}

let handlePlayerTurn (runState: RunState) playerState battle =
    if playerState.actionsRemaining = 0 then
        if runState.WasJustPressed Keys.Enter then
            Some { battle with state = OrcTurn { index = 0; lastTick = 0.0 } }
        else
            Some battle
    else
        if runState.WasJustPressed Keys.S then playerAttack Stab playerState battle |> Some
        elif runState.WasJustPressed Keys.F then playerAttack Flail playerState battle |> Some
        elif runState.WasJustPressed Keys.R then playerAttack Recover playerState battle |> Some
        else Some battle

let handleOrcTurn orcState battle =
    Some battle

let updateModel (runState: RunState) currentBattle = 
    match currentBattle with
    | None -> Some initialBattle
    | Some battle ->
        match battle.state with
        | GameOver -> currentBattle
        | PlayerTurn playerState -> handlePlayerTurn runState playerState battle
        | OrcTurn orcState -> handleOrcTurn runState orcState battle