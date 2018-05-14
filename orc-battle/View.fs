module View

open GameCore
open Model

let assets = 
    [
        Texture { key = "player"; path = "Content/MitheralKnight" }
        Texture { key = "orc_club"; path = "Content/HunterOrc" }
        Texture { key = "orc_spear"; path = "Content/LuckyOrc" }
        Texture { key = "orc_whip"; path = "Content/RedOrc" }
        Texture { key = "green"; path = "Content/green" }
        Texture { key = "red"; path = "Content/red" }
        Texture { key = "black"; path = "Content/black" }
        Texture { key = "white"; path = "Content/white" }
        Font { key = "default"; path = "Content/JuraMedium" }
    ]

let idleFrames = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

let barElements colourKey rect percent = 
    seq {
        yield { assetKey = "black"; destRect = rect; sourceRect = None }
        let (x,y,width,height) = rect
        yield { assetKey = "white"; destRect = x+2,y+2,width-4,height-4; sourceRect = None }
        let valueWidth = width - 4 |> float |> (*) percent |> int
        yield { assetKey = colourKey; destRect = x+2,y+2,valueWidth,height-4; sourceRect = None }
    } |> Seq.toList

let unitWithHealth position assetKey frame healthPercent = 
    let (x,y,width,height) = position
    [
        [{ assetKey = assetKey; destRect = position; sourceRect = Some frame }]
        barElements "green" (x,y + height + 10,width,width / 8) healthPercent
    ] |> List.concat

let selected position assetKey = 
    let (x,y,width,height) = position
    [
        { assetKey = assetKey; destRect = position; sourceRect = None }
        { assetKey = "white"; destRect = (x + 2, y + 2, width - 4, height - 4); sourceRect = None }
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
        } |> Seq.toList |> List.concat |> List.map Image

    let text = 
        seq {
            match battle.state with
            | OrcTurn _ -> 
                yield { assetKey = "default"; text = "The Orcs Are Attacking!"; position = 80,350; scale = 0.5 }
            | GameOver ->
                yield { assetKey = "default"; text = "Game Over!"; position = 200,350; scale = 1.0 }
                yield { assetKey = "default"; text = "Press R to restart or ESC to exit"; position = 200,410; scale = 0.3 }
            | Victory ->
                yield { assetKey = "default"; text = "Victory!"; position = 200,350; scale = 1.0 }
                yield { assetKey = "default"; text = "Press R to restart or ESC to exit"; position = 200,410; scale = 0.3 }
            | PlayerTurn state ->
                yield { assetKey = "default"; text = "Player Turn"; position = 80,200; scale = 0.5 }
                let actionsStatus = sprintf "Actions Remaining: %i" state.actionsRemaining
                yield { assetKey = "default"; text = actionsStatus; position = 80,240; scale = 0.3 }
                if state.actionsRemaining = 0 then
                    yield { assetKey = "default"; text = "Press ENTER to"; position = 80,260; scale = 0.3 } 
                    yield { assetKey = "default"; text = " end turn"; position = 80,280; scale = 0.3 } 
                else
                    yield { assetKey = "default"; text = "Press S to stab,"; position = 80,260; scale = 0.3 }
                    yield { assetKey = "default"; text = "Press F to flail,"; position = 80,280; scale = 0.3 }
                    yield { assetKey = "default"; text = "Press R to recover,"; position = 80,300; scale = 0.3 }
                    yield { assetKey = "default"; text = "And Left or Right"; position = 80,320; scale = 0.3 }
                    yield { assetKey = "default"; text = " to change target"; position = 80,340; scale = 0.3 }
        } |> Seq.toList |> List.map Text

    images @ text