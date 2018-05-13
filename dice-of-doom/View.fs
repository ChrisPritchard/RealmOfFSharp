module View
open GameCore
open Hex

let assets = 
    [
        "hex_flat", "Content/hexFlat"
        "hex_pointy", "Content/hexPointy"
    ],
    [
        "font", "Content/JuraMedium"
    ]

let getView runState model =
    let hexes = [0.0..5.0] |> List.collect (fun q -> [0.0..5.0] |> List.map (fun r -> { q = q; r = r }))
    
    let cubeTop = Pointy
    let size = 64.
    let width,height = Cube.width cubeTop size, Cube.height cubeTop size
    
    let points = hexes |> List.map (Hex.toCube >> Cube.toPixel cubeTop size)
    let rectFrom (x,y) = x - width/2.0 |> int, y - height/2.0 |> int, ceil width |> int, ceil height |> int 

    let texture = match cubeTop with | Flat -> "hex_flat" | _ -> "hex_pointy"
    let images = points |> List.map (rectFrom >> fun rect -> { textureKey = texture; destRect = rect; sourceRect = None })
         
    images,[]