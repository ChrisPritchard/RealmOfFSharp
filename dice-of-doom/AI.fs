module AI
open Model
open GameCore

let timeBetweenActions = 1000.
let searchDepth = 3

let score board player =
    match winners board with
    | [p] when p = player -> 1.
    | lst when List.contains player lst -> 0.5
    | _ -> 0. 

let rec scoreAll moves depth player = 
    match depth with
    | 0 -> 0.
    | _ ->
        moves |> List.sumBy (fun (_,gt) -> 
            let gta = gt ()
            let boardScore = score gta.board player
            let subScore = 
                match gta.moves with
                | Some m -> scoreAll m (depth - 1) player
                | None -> 0.
            boardScore + subScore)

let aiMove (runState:RunState) model (moves: (Move * (unit -> GameTree)) list) =
    if runState.elapsed-model.lastAIAction < timeBetweenActions then
        model
    else 
        let player = model.gameTree.player
        let (_, gt) = 
            moves |> List.maxBy (fun (_,gt) -> 
            let gta = gt ()
            let boardScore = score gta.board player
            match gta.moves with
            | None -> boardScore
            | Some sm -> boardScore + scoreAll sm searchDepth player)
        { model with lastAIAction = runState.elapsed; gameTree = gt () }