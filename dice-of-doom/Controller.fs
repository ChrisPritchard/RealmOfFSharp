module Controller
open Model

let initialModel = { 
    player = 0
    source = None
    target = None
    gameTree = generateTree startTerritories 0 false
}

let checkForMoves runState gameModel =
    gameModel

let updateModel runState currentModel = 
    match currentModel with
    | None -> Some initialModel
    | Some model -> checkForMoves runState model |> Some