module View
open GameCore
open Hex

let assets = [
    { key = "font"; assetType = AssetType.Font; path = "Content/JuraMedium" }
    { key = "hex_flat"; assetType = AssetType.Texture; path = "Content/hexFlat" }
    { key = "hex_pointy"; assetType = AssetType.Texture; path = "Content/hexPointy" }
]

let getView runState model =
    let hexes = [0.0..5.0] |> List.collect (fun q -> [0.0..5.0] |> List.map (fun r -> { q = q; r = r }))
    
    let cubeTop = Pointy
    let size = 32.
    let width,height = Cube.width cubeTop size, Cube.height cubeTop size
    
    let points = hexes |> List.map (Hex.toCube >> Cube.toPixel cubeTop size)
    let rectFrom (x,y) = x - width/2.0 |> int, y - height/2.0 |> int, int width, int height 

    let texture = match cubeTop with | Flat -> "hex_flat" | _ -> "hex_pointy"
    let images = points |> List.map (rectFrom >> fun rect -> { textureKey = texture; destRect = rect; sourceRect = None })
         
    images,[]