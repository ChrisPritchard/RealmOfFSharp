module Model
open Hex

let hexTop = Pointy

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
    reinforcements: int
    moves: (Move * GameTree) list option
} and Move = 
    | Pass
    | Attack of Territory * Territory

let players = 2
let maxDice = 3

let startTerritories = 
    [0.0..5.] |> List.collect (fun q ->
    [0.0..5.] |> List.map (fun r ->
        {   owner = (if q < 3. then 0 else 1)
            dice = (if (q = 2. || q = 3.) && r % 2. = 0. then 2 else 1)
            hex = { q = q; r = r } }))

let generateAttacks territories player = 
    let isValid hexMatch dice target =
        target.hex = hexMatch && target.owner <> player && target.dice < dice
    territories 
    |> List.filter (fun t -> t.owner = player)
    |> List.collect (fun source -> 
        let neighbours = Hex.neighbours hexTop source.hex
        let validTargets = 
            neighbours 
                |> List.map (fun h -> List.tryFind (isValid h source.dice) territories)
                |> List.choose id
        validTargets |> List.map (fun target -> Attack (source,target)))

let attackAftermath (territories:Territory list) (source,target) = 
    List.except [source;target] territories @ 
        [{ source with dice = 1 }
         { target with owner = source.owner; dice = source.dice - 1 }]

let reinforcePlayer player reinforcements territories = 
    let (candidates,other) = territories |> List.partition (fun t -> t.owner = player && t.dice < maxDice)
    let count = min (List.length candidates) reinforcements
    let updated = List.take count candidates |> List.map (fun t -> { t with dice = t.dice + 1 })
    (updated @ List.skip count candidates @ other, reinforcements - count)

let rec generateTree territories player reinforcements canPass =
    let attacks = generateAttacks territories player
    let moves = if canPass then [Pass] @ attacks else attacks
    if List.isEmpty moves then 
        { board = territories; player = player; reinforcements = reinforcements; moves = None }
    else
        let nextPlayer = if player + 1 = players then 0 else player + 1
        {
            board = territories
            player = player
            reinforcements = reinforcements
            moves = moves 
                |> List.map (fun move ->
                    match move with
                    | Pass -> 
                        let (reinforced, remaining) = reinforcePlayer player reinforcements territories
                        move, generateTree reinforced nextPlayer remaining false
                    | Attack (s,t) -> 
                        let aftermath = attackAftermath territories (s,t)
                        move, generateTree aftermath player reinforcements true) 
                |> Some
        }