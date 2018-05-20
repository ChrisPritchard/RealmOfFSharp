module Model
open Hex
open Microsoft.Xna.Framework

let hexTop = Pointy

type DiceGameModel = {
    source: Territory option
    gameOptions: GameOptions
    gameTree: GameTree
} and Territory = {
    owner: int
    dice: int
    hex: Hex
} and GameOptions = {
    maxDice: int
    players: (string * Color) list
} and GameTree = {
    board: Territory list
    player: int
    reinforcements: int
    moves: (Move * (Unit -> GameTree)) list option
} and Move = 
    | Pass
    | Attack of Territory * Territory

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

let reinforcePlayer gameOptions player reinforcements territories = 
    let (candidates,other) = territories |> List.partition (fun t -> t.owner = player && t.dice < gameOptions.maxDice)
    let count = min (List.length candidates) reinforcements
    let updated = List.take count candidates |> List.map (fun t -> { t with dice = t.dice + 1 })
    (updated @ List.skip count candidates @ other, reinforcements - count)

let rec generateTree gameOptions territories player reinforcements canPass =
    let attacks = generateAttacks territories player
    let moves = if canPass then [Pass] @ attacks else attacks
    if List.isEmpty moves then 
        { board = territories; player = player; reinforcements = reinforcements; moves = None }
    else
        let playerCount = gameOptions.players.Length
        let nextPlayer = if player + 1 = playerCount then 0 else player + 1
        {
            board = territories
            player = player
            reinforcements = reinforcements
            moves = moves 
                |> List.map (fun move ->
                    match move with
                    | Pass -> 
                        let (reinforced, remaining) = reinforcePlayer gameOptions player reinforcements territories
                        move, fun () -> generateTree gameOptions reinforced nextPlayer remaining false
                    | Attack (s,t) -> 
                        let aftermath = attackAftermath territories (s,t)
                        move, fun () -> generateTree gameOptions aftermath player reinforcements true) 
                |> Some
        }

let winners territories = 
    territories 
        |> List.groupBy (fun o -> o.owner)
        |> List.map (fun (o,lst) -> (o, List.length lst))
        |> List.groupBy (fun (_,score) -> score)
        |> List.map (fun (score, lst) -> score, lst |> List.map (fun (o,_) -> o))
        |> List.sortByDescending (fun (score,_) -> score)
        |> List.head |> snd