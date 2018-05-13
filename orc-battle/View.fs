module View
open GameCore
open Model

let assets = 
    [
        "player", "Content/MitheralKnight"
        "orc_club", "Content/HunterOrc"
        "orc_spear", "Content/LuckyOrc"
        "orc_whip", "Content/RedOrc"
        "green", "Content/green"
        "red", "Content/red"
        "black", "Content/black"
        "white", "Content/white"
    ],
    [
        "default", "Content/JuraMedium"
    ]

let idleFrames = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

let barElements colourKey rect percent = 
    seq {
        yield { textureKey = "black"; destRect = rect; sourceRect = None }
        let (x,y,width,height) = rect
        yield { textureKey = "white"; destRect = x+2,y+2,width-4,height-4; sourceRect = None }
        let valueWidth = width - 4 |> float |> (*) percent |> int
        yield { textureKey = colourKey; destRect = x+2,y+2,valueWidth,height-4; sourceRect = None }
    } |> Seq.toList

let unitWithHealth position textureKey frame healthPercent = 
    let (x,y,width,height) = position
    [
        [{ textureKey = textureKey; destRect = position; sourceRect = Some frame }]
        barElements "green" (x,y + height + 10,width,width / 8) healthPercent
    ] |> List.concat

let selected position textureKey = 
    let (x,y,width,height) = position
    [
        { textureKey = textureKey; destRect = position; sourceRect = None }
        { textureKey = "white"; destRect = (x + 2, y + 2, width - 4, height - 4); sourceRect = None }
    ]

let playerPos = (100,50,100,100)
let (ox,oy,owidth,oheight) = (300,50,100,100)
let (ogx,ogy) = 20,40
let orcsPerRow = 3

let getOrcUnit rowIndex colIndex isSelected animFrame orc =
    let (x, y) = (ox + (colIndex * (100 + ogx)), oy + (rowIndex * (100 + ogy)))
    let orcHealth = float orc.health / float orcStartHealth
    let texture = 
        match orc.weapon with
        | Weapon.Club -> "orc_club"
        | Weapon.Spear -> "orc_spear"
        | _ -> "orc_whip"
    let orcFrame = if orc.health > 0 then animFrame else (0,128,32,32)
    seq {
        if isSelected then yield! selected (x,y,100,100) "red"
        yield! unitWithHealth (x,y,100,100) texture orcFrame orcHealth
    } |> Seq.toList

let getView runState battle = 
    let idleFrame = idleFrames.[(runState.elapsed / 100.0) % 10.0 |> int]
    
    let images =
        seq {
            let playerHealth = float battle.player.health / float initialPlayer.health
            let playerFrame = if battle.player.health > 0 then idleFrame else (32*9,128,32,32)
            yield unitWithHealth playerPos "player" playerFrame playerHealth
            
            let targetIndex = 
                match battle.state with
                | PlayerTurn t -> t.target
                | OrcTurn o -> o.index
                | _ -> 0 
            yield! 
                List.chunkBySize orcsPerRow battle.orcs 
                |> List.indexed |> List.collect (fun (ridx,row) -> 
                    row |> List.indexed |> List.map (fun (oidx, orc) -> 
                        let isSelected = (ridx * orcsPerRow) + oidx = targetIndex
                        getOrcUnit ridx oidx isSelected idleFrame orc))
        } |> Seq.toList |> List.concat

    let text = 
        seq {
            match battle.state with
            | OrcTurn _ -> 
                yield { fontKey = "default"; text = "The Orcs Are Attacking!"; position = 80,350; scale = 0.5 }
            | GameOver ->
                yield { fontKey = "default"; text = "Game Over!"; position = 200,350; scale = 1.0 }
                yield { fontKey = "default"; text = "Press R to restart or ESC to exit"; position = 200,410; scale = 0.3 }
            | Victory ->
                yield { fontKey = "default"; text = "Victory!"; position = 200,350; scale = 1.0 }
                yield { fontKey = "default"; text = "Press R to restart or ESC to exit"; position = 200,410; scale = 0.3 }
            | PlayerTurn state ->
                yield { fontKey = "default"; text = "Player Turn"; position = 80,200; scale = 0.5 }
                let actionsStatus = sprintf "Actions Remaining: %i" state.actionsRemaining
                yield { fontKey = "default"; text = actionsStatus; position = 80,240; scale = 0.3 }
                if state.actionsRemaining = 0 then
                    yield { fontKey = "default"; text = "Press ENTER to"; position = 80,260; scale = 0.3 } 
                    yield { fontKey = "default"; text = " end turn"; position = 80,280; scale = 0.3 } 
                else
                    yield { fontKey = "default"; text = "Press S to stab,"; position = 80,260; scale = 0.3 }
                    yield { fontKey = "default"; text = "Press F to flail,"; position = 80,280; scale = 0.3 }
                    yield { fontKey = "default"; text = "Press R to recover,"; position = 80,300; scale = 0.3 }
                    yield { fontKey = "default"; text = "And Left or Right"; position = 80,320; scale = 0.3 }
                    yield { fontKey = "default"; text = " to change target"; position = 80,340; scale = 0.3 }
        } |> Seq.toList

    images, text