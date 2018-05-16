module View
open GameCore
open Hex
open Microsoft.Xna.Framework

let assets = 
    [
        Texture { key = "hex_flat"; path = "Content/hexFlat" }
        Texture { key = "hex_pointy"; path = "Content/hexPointy" }
        Font { key = "font"; path = "Content/JuraMedium" }
        Texture { key = "pointer"; path = "Content/pointer" }
    ]

let getView runState model =
    let hexes = [0.0..5.0] |> List.collect (fun q -> [0.0..5.0] |> List.map (fun r -> { q = q; r = r }))
    
    let cubeTop = Pointy
    let size = 64.
    let width,height = Cube.width cubeTop size, Cube.height cubeTop size
    
    let points = hexes |> List.map (fun h -> h, Hex.toCube h |> Cube.toPixel cubeTop size)
    let (ox,oy) = (100.,100.)
    let rectFrom (x,y) = (ox + x) - width/2.0 |> int, (oy + y) - height/2.0 |> int, ceil width |> int, ceil height |> int 

    let (mx,my) = runState.mouse.position
    let mouseHex = Cube.fromPixel cubeTop size (float mx - ox, float my - oy) |> Cube.toAxial

    let texture = match cubeTop with | Flat -> "hex_flat" | _ -> "hex_pointy"
    let images = points |> List.map (fun (hex,point) ->
        let drawable = { assetKey = texture; destRect = rectFrom point; sourceRect = None }
        if mouseHex = hex then ColouredImage (Color.Red, drawable) else Image drawable)

    let cursor = Image { assetKey = "pointer"; destRect = mx,my,27,27; sourceRect = Some (0,0,18,18) }         

    images @ [cursor]