module Model
open Hex

let cubeTop = Pointy

type DiceGameModel = {
    player: int
    source: int option
    target: int option
    gameTree: GameTree
} and Territory = {
    index: int
    owner: int
    dice: int
    hex: Hex
} and GameTree = {
    board: Territory list
    moves: (Move * GameTree) list option
} and Move = 
    | Pass
    | Attack of int * int

let reinforcementPool = 10
let players = 2

let startTerritories = [
    { index = 0; owner = 0; dice = 2; hex = { q = 0.; r = 0. } }
    { index = 0; owner = 0; dice = 2; hex = { q = 0.; r = 1. } }
    { index = 1; owner = 1; dice = 1; hex = { q = 1.; r = 0. } }
    { index = 1; owner = 1; dice = 2; hex = { q = 2.; r = 0. } }
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
            validTargets |> List.map (fun target -> Attack (source.index,target.index)))
    if canPass then [Pass] @ attacks else attacks

let private replace index newItem lst = 
    List.take index lst @ [newItem] @ List.skip (index + 1) lst

let generateOutcomes (territories:Territory list) (attacks: Move list) = 
    attacks |> List.map (fun move -> 
        match move with
        | Pass -> (Pass, territories)
        | Attack (a,b) ->
            let source = territories.[a]
            let target = territories.[b]
            let aftermath = 
                territories 
                |> replace a { source with dice = 1 } 
                |> replace b { target with owner = source.owner; dice = source.dice - 1 }
            (Attack (a,b), aftermath))

let rec generateTree territories player canPass =
    let options = generateMoves territories player canPass
    if List.isEmpty options then { board = territories; moves = None }
    else
        let nextPlayer = if player + 1 = players then 0 else player + 1
        {
            board = territories
            moves = generateOutcomes territories options 
                |> List.map (fun (m,t) -> 
                    match m with
                    | Pass -> m,generateTree t nextPlayer false
                    | _ -> m,generateTree t player true) |> Some
        }