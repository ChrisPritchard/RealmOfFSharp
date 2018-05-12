module View
open GameCore

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

let anims = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

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

let getView runState _ = 
    let frame = anims.[(runState.elapsed / 100.0) % 10.0 |> int]
    [
        unitWithHealth (100,100,100,100) "player" frame 0.2
        unitWithHealth (300,100,100,100) "orc_club" frame 0.2
        selected (410,100,100,100) "red"
        unitWithHealth (410,100,100,100) "orc_spear" frame 0.2
        unitWithHealth (520,100,100,100) "orc_whip" frame 0.2
    ] |> List.concat, []