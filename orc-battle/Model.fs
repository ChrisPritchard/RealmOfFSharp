module Model
open System

let rnd = new Random ()
let random n = rnd.Next (0, n)

let attacksPerTurn = 3

type Player = {
    health: int
    agility: int
    strength: int
}

let initialPlayer = { health = 35; agility = 35; strength = 35; }

type PlayerAttack = | Stab | Flail | Recover

type Orc = {
    health: int
    weapon: Weapon
} and Weapon = | Club | Spear | Whip

type Battle = {
    player: Player
    orcs: Orc list
    actionsRemaining: int
    targetIndex: int
    gameOver: bool
}

let orcAttack orc (player: Player) = 
    if player.health = 0 then player
    else
        match orc.weapon with
        | Club -> { player with health = player.health - random 6 } 
        | Spear -> { player with health = player.health - random 3; strength = player.agility - random 3 }
        | Whip -> { player with health = player.health - random 3; agility = player.agility - random 3 }

let replace index newItem lst = 
    List.take index lst @ [newItem] @ List.skip (index + 1) lst

let playerAttack attackType battle =
    let btl = { battle with actionsRemaining = battle.actionsRemaining - 1 }
    match attackType with
    | Recover -> 
        { btl with player = { battle.player with health = min (battle.player.health + random 8) initialPlayer.health } }
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
                Some (n,{ o with health = o.health - random 3 })
            )
        List.fold (fun b op -> 
            match op with
            | None -> b
            | Some (i, o) -> { b with orcs = replace i o b.orcs }) btl newOrcs