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
    { index = 0; owner = 1; dice = 1; hex = { q = 1.; r = 0. } }
]

let generateOptions territories player = 
    territories 
        |> List.filter (fun t -> t.owner = player)
        |> List.collect (fun source -> 
            let neighbours = Hex.toCube source.hex |> Cube.neighbours cubeTop |> List.map Cube.toAxial
            let valid = 
                neighbours |> List.map (fun h -> 
                    List.tryFind (fun tn -> tn.hex = h && tn.owner <> player) territories)
                    |> List.choose id
            valid |> List.map (fun target -> Attack (source.index,target.index)))

let generateOutcomes territories attacks = 
    List.map (fun a -> a,territories) attacks

let rec generateTree territories player =
    let options = generateOptions territories player
    if List.isEmpty options then { board = territories; moves = None }
    else
        let nextPlayer = if player + 1 = players then 0 else player + 1
        {
            board = territories
            moves = [Pass,generateTree territories nextPlayer] @ generateOutcomes territories options |> Some
        }