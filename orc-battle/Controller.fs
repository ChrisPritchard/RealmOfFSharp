module Controller

open GameCore
open Model
open Microsoft.Xna.Framework.Input

let initialBattle = {
    player = initialPlayer
    orcs = [1..6] |> List.map (fun _ -> getOrc ())
    state = turnStart
}

let handlePlayerTurn (runState: RunState) playerState battle =
    let pressed = runState.WasJustPressed
    if playerState.actionsRemaining = 0 then
        if pressed Keys.Enter then
            Some { battle with state = OrcTurn { index = -1; lastTick = 0.0 } }
        else
            Some battle
    else
        let checkForGameOver postAttack = 
            if postAttack.orcs |> List.forall (fun o -> o.health <= 0) 
                then Some { postAttack with state = GameOver }
                else Some postAttack
        if pressed Keys.S then playerAttack Stab playerState battle |> checkForGameOver
        elif pressed Keys.F then playerAttack Flail playerState battle |> checkForGameOver
        elif pressed Keys.R then playerAttack Recover playerState battle |> Some
        else 
            let orcCount = List.length battle.orcs
            if pressed Keys.Left then
                let newIndex = if playerState.target = 0 then orcCount - 1 else playerState.target - 1
                Some { battle with state = PlayerTurn { playerState with target = newIndex } }
            elif pressed Keys.Right then
                let newIndex = if playerState.target = orcCount - 1 then 0 else playerState.target + 1
                Some { battle with state = PlayerTurn { playerState with target = newIndex } }
            else
                Some battle

let handleOrcTurn (runState: RunState) orcState battle =
    if runState.elapsed - orcState.lastTick < timeBetweenOrcs then Some battle
    else
        let orcCount = List.length battle.orcs
        let postAttack = 
            if orcState.index >= 0 then orcAttack battle.orcs.[orcState.index] battle
            else battle
        if postAttack.player.health <= 0 then 
            Some { postAttack with state = GameOver }
        elif orcState.index = orcCount - 1 then 
            Some { postAttack with state = turnStart }
        else
            let nextIndex = orcState.index + 1
            if battle.orcs.[nextIndex].health <= 0 then
                Some { postAttack with state = OrcTurn { index = nextIndex; lastTick = orcState.lastTick } }
            else
                Some { postAttack with state = OrcTurn { index = nextIndex; lastTick = runState.elapsed } }

let updateModel (runState: RunState) currentBattle = 
    match currentBattle with
    | None -> Some initialBattle
    | Some battle ->
        match battle.state with
        | GameOver | Victory -> 
            if runState.WasJustPressed Keys.R then Some initialBattle
            elif runState.WasJustPressed Keys.Escape then None
            else currentBattle
        | PlayerTurn playerState -> handlePlayerTurn runState playerState battle
        | OrcTurn orcState -> handleOrcTurn runState orcState battle