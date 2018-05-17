module Controller
open GameCore
open Hex
open Model
open View

let initialModel = {
    source = None
    gameTree = generateTree startTerritories 0 false
}

let checkForMove (runState:RunState) gameModel moves =
    let (left,_) = runState.mouse.pressed
    let (mx, my) = runState.mouse.position
    let hex = Hex.fromPixel cubeTop hexSize (float mx, float my)

    let attacks = List.choose (fun (m,gt) -> 
        match m with | Attack (a,b) -> Some ((a,b),gt) | Pass -> None) moves

    if left then
        let pickSource = List.tryFind (fun ((a,_), _) -> a.hex = hex) attacks
        match pickSource with
        | Some ((a,_),_) -> { gameModel with source = Some a }
        | None when gameModel.source <> None -> 
            let pickTarget = List.tryFind (fun ((_,t), _) -> t.hex = hex) attacks
            match pickTarget with
            | Some (_,gt) -> { gameModel with source = None; gameTree = gt }
            | None -> gameModel
        | None -> 
            let pass = List.tryPick (fun (m,gt) -> match m with | Pass -> Some gt | _ -> None) moves
            match pass with
            | Some gt when Rectangle.contains (mx,my) View.passButton -> 
                { gameModel with source = None; gameTree = gt; }
            | None -> gameModel
    else gameModel

let updateModel runState currentModel = 
    match currentModel with
    | None -> Some initialModel
    | Some model -> 
        match model.gameTree.moves with
        | None -> currentModel
        | Some moves -> 
            checkForMove runState model moves |> Some