module Controller
open GameCore
open Hex
open Model
open View
open AI
open Microsoft.Xna.Framework


let gameOptions = {
    maxDice = 4
    players = 
    [
        "Red Player", Color.Red, Human
        "Blue Player", Color.Blue, AI
    ]
}

let random = new System.Random (0)
let randomDice () = random.Next(1, gameOptions.maxDice + 1)

let gridSize = 6.
let startTerritories = 
    [0.0..gridSize - 1.] |> List.collect (fun q ->
    [0.0..gridSize - 1.] |> List.map (fun r ->
        {   owner = (if q < gridSize/2. then 0 else 1)
            dice = randomDice ()
            hex = { q = q; r = r } }))

let stopWatch = System.Diagnostics.Stopwatch.StartNew ()
let initialModel = {
    gameOptions = gameOptions
    lastAIAction = 0.
    source = None
    gameTree = generateTree gameOptions startTerritories 0 100 false
}
printfn "Generation Time: %ims" stopWatch.ElapsedMilliseconds

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
            { gameModel with source = None; gameTree = gt (); }
        | _ -> 
            match overAttackSource with
            | Some ((a,_),_) -> { gameModel with source = Some a }
            | None when gameModel.source <> None -> 
                match overAttackTarget with
                | Some (_,gt) -> { gameModel with source = None; gameTree = gt () }
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
            let (_,_,playerType) = model.gameOptions.players.[model.gameTree.player]
            match playerType with
            | Human -> playerMove runState model moves |> Some
            | AI -> aiMove runState model moves |> Some