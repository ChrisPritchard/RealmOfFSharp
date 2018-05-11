module Model
open System

let rnd = new Random ()
let random n = rnd.Next (0, n)

let attacksPerTurn = 3

type Player = {
    health: int
}

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
    state: State
}
  and State = | PlayerTurn of PlayerState | OrcTurn of int | GameOver
  and PlayerState = { actionsRemaining: int; target: int }

let orcAttack orc battle = 
    let health = battle.player.health
    if health <= 0 then battle
    else
        let setHealth newHealth = 
            { battle with player = { battle.player with health = newHealth } }
        match orc.weapon with
        | Weapon.Club -> setHealth (health - random 6)
        | Weapon.Spear -> setHealth (health - (2 + random 3))
        | _ -> setHealth (health - 3) // Whip

let replace index newItem lst = 
    List.take index lst @ [newItem] @ List.skip (index + 1) lst

let playerAttack attackType playerState battle =
    let btl = { battle with state = PlayerTurn { playerState with actionsRemaining = playerState.actionsRemaining - 1 } }
    match attackType with
    | Recover -> 
        let newHealth = btl.player.health |> (+) (random 8) |> max initialPlayer.health
        { btl with player = { btl.player with health = newHealth } }
    | Stab ->
        match List.tryItem playerState.target battle.orcs with
        | None -> battle
        | Some o -> 
            let newOrc = { o with health = o.health - random 6 }
            { btl with orcs = replace playerState.target newOrc battle.orcs }
    | Flail ->
        let newOrcs =
            [-1;0;1] |> List.map (fun n -> 
                let index = playerState.target + n
                match List.tryItem index battle.orcs with
                | None -> None
                | Some o -> 
                    Some (n,{ o with health = o.health - random 3 }))
        List.fold (fun b op -> 
            match op with
            | None -> b
            | Some (i, o) -> { b with orcs = replace i o b.orcs }) btl newOrcs