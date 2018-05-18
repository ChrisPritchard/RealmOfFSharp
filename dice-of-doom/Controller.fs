module Controller
open GameCore
open Hex
open Model
open View

let initialModel = {
    source = None
    reinforcements = 100
    gameTree = generateTree startTerritories 0 false
}

let rectContains (px,py) (x,y,w,h) = 
    px >= x && py >= y && px <= x + w && py <= y + h

let playerMove (runState:RunState) gameModel moves =

    let possibleAttacks = List.choose (fun (m,gt) -> 
        match m with | Attack (a,b) -> Some ((a,b),gt) | Pass -> None) moves
    let canPass = List.tryPick (fun (m,gt) -> 
        match m with | Pass -> Some gt | _ -> None) moves

    let (mx, my) = runState.mouse.position
    let (ox, oy) = calculateOffset gameModel.gameTree.board
    let mouseHex = Hex.fromPixel hexTop hexSize (float mx - ox, float my - oy)
    let overAttackSource = List.tryFind (fun ((a,_), _) -> a.hex = mouseHex) possibleAttacks
    let overAttackTarget = List.tryFind (fun ((_,t), _) -> t.hex = mouseHex) possibleAttacks

    if fst runState.mouse.pressed then
        match canPass with
        | Some gt when rectContains (mx,my) View.passButton -> 
            { gameModel with source = None; gameTree = gt; }
        | _ -> 
            match overAttackSource with
            | Some ((a,_),_) -> { gameModel with source = Some a }
            | None when gameModel.source <> None -> 
                match overAttackTarget with
                | Some (_,gt) -> { gameModel with source = None; gameTree = gt }
                | None -> gameModel
            | None -> gameModel
    else gameModel

let updateModel runState currentModel = 
    match currentModel with
    | None -> Some initialModel
    | Some model -> 
        match model.gameTree.moves with
        | None -> currentModel
        | Some moves -> 
            playerMove runState model moves |> Some