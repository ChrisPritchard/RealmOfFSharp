module View
open GameCore
open Model

let assets = [
        { key = "player"; assetType = AssetType.Texture; path = "Content/MitheralKnight" }
        { key = "orc_club"; assetType = AssetType.Texture; path = "Content/HunterOrc" }
        { key = "orc_spear"; assetType = AssetType.Texture; path = "Content/LuckyOrc" }
        { key = "orc_whip"; assetType = AssetType.Texture; path = "Content/RedOrc" }
        { key = "green"; assetType = AssetType.Texture; path = "Content/green" }
        { key = "red"; assetType = AssetType.Texture; path = "Content/red" }
        { key = "black"; assetType = AssetType.Texture; path = "Content/black" }
        { key = "white"; assetType = AssetType.Texture; path = "Content/white" }
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
    } |> Seq.toList |> List.concat, []