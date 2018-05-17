module Model
open Hex

let cubeTop = Pointy

type DiceGameModel = {
    source: Territory option
    gameTree: GameTree
} and Territory = {
    owner: int
    dice: int
    hex: Hex
} and GameTree = {
    board: Territory list
    player: int
    moves: (Move * GameTree) list option
} and Move = 
    | Pass
    | Attack of Territory * Territory

let reinforcementPool = 10
let players = 2

let startTerritories = [
    { owner = 0; dice = 2; hex = { q = 0.; r = 0. } }
    { owner = 0; dice = 2; hex = { q = 0.; r = 1. } }
    { owner = 1; dice = 1; hex = { q = 1.; r = 0. } }
    { owner = 1; dice = 2; hex = { q = 2.; r = 0. } }
    { owner = 1; dice = 2; hex = { q = 1.; r = 1. } }
]

let generateMoves territories player canPass = 
    let isValid hexMatch dice target =
        target.hex = hexMatch && target.owner <> player && target.dice < dice
    let attacks = 
        territories 
        |> List.filter (fun t -> t.owner = player)
        |> List.collect (fun source -> 
            let neighbours = Hex.neighbours cubeTop source.hex
            let validTargets = 
                neighbours 
                    |> List.map (fun h -> List.tryFind (isValid h source.dice) territories)
                    |> List.choose id
            validTargets |> List.map (fun target -> Attack (source,target)))
    if canPass then [Pass] @ attacks else attacks

let generateOutcomes (territories:Territory list) (attacks: Move list) = 
    attacks |> List.map (fun move -> 
        match move with
        | Pass -> (Pass, territories)
        | Attack (source,target) ->
            let aftermath = 
                List.except [source;target] territories 
                @ [{ source with dice = 1 };{ target with owner = source.owner; dice = source.dice - 1 }]
            (Attack (source,target), aftermath))

let rec generateTree territories player canPass =
    let options = generateMoves territories player canPass
    if List.isEmpty options then { board = territories; player = player; moves = None }
    else
        let nextPlayer = if player + 1 = players then 0 else player + 1
        {
            board = territories
            player = player
            moves = generateOutcomes territories options 
                |> List.map (fun (m,t) -> 
                    match m with
                    | Pass -> m,generateTree t nextPlayer false
                    | _ -> m,generateTree t player true) |> Some
        }