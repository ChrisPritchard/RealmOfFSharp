module AI
open Model
open GameCore

let timeBetweenActions = 1000.
let searchDepth = 4

let score board player =
    match winners board with
    | [p] when p = player -> 1.
    | lst when List.contains player lst -> 0.5
    | _ -> 0. 

let aiMove (runState:RunState) model (moves: (Move * (unit -> GameTree)) list) =
    if runState.elapsed-model.lastAIAction < timeBetweenActions then
        model
    else 
        let player = model.gameTree.player
        let (_,gt) = moves |> List.maxBy (fun (_,gt) -> score (gt().board) player)
        { model with lastAIAction = runState.elapsed; gameTree = gt () }