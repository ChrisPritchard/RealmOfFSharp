module Controller
open Model

let initialModel = { playerIndex = 0 }

let updateModel runState currentModel = 
    match currentModel with
    | None -> Some initialModel
    | Some model -> Some model