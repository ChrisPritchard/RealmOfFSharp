module Model
open System
open Aether
open Aether.Operators

let rnd = new Random ()
let random n = rnd.Next (0, n)

let attacksPerTurn = 3

type Player = {
    health: int
} with
    static member health_ = (fun o -> o.health), (fun h o -> { o with health = h })

let initialPlayer = { health = 35; }

type PlayerAttack = | Stab | Flail | Recover

type Orc = {
    health: int
    weapon: Weapon
} and Weapon = | Club = 0 | Spear = 1 | Whip = 2

let getOrc = { health = 8; weapon = enum<Weapon>(random 2) }

type Battle = {
    player: Player
    orcs: Orc list
    actionsRemaining: int
    targetIndex: int
    gameOver: bool
} with
    static member player_ = (fun o -> o.player), (fun p o -> { o with player = p })

let playerHealth = Battle.player_ >-> Player.health_

let orcAttack orc (player: Player) = 
    if player.health = 0 then player
    else
        match orc.weapon with
        | Weapon.Club -> { player with health = player.health - random 6 } 
        | Weapon.Spear -> { player with health = player.health - (2 + random 3) }
        | _ -> { player with health = player.health - 3 } // Whip

let replace index newItem lst = 
    List.take index lst @ [newItem] @ List.skip (index + 1) lst

let playerAttack attackType battle =
    let btl = { battle with actionsRemaining = battle.actionsRemaining - 1 }
    match attackType with
    | Recover -> 
        let newHealth = Optic.get playerHealth btl |> (+) (random 8) |> max initialPlayer.health
        Optic.set playerHealth newHealth btl
    | Stab ->
        match List.tryItem battle.targetIndex battle.orcs with
        | None -> battle
        | Some o -> 
            let newOrc = { o with health = o.health - random 6 }
            { btl with orcs = replace battle.targetIndex newOrc battle.orcs }
    | Flail ->
        let newOrcs =
            [-1;0;1] |> List.map (fun n -> 
                let index = battle.targetIndex + n
                match List.tryItem index battle.orcs with
                | None -> None
                | Some o -> 
                    Some (n,{ o with health = o.health - random 3 }))
        List.fold (fun b op -> 
            match op with
            | None -> b
            | Some (i, o) -> { b with orcs = replace i o b.orcs }) btl newOrcs