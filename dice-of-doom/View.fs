module View
open GameCore
open Hex
open Model
open Microsoft.Xna.Framework

let sw,sh = (1024,768)
let hexSize = 64.
let hexWidth,hexHeight = Hex.width cubeTop hexSize, Hex.height cubeTop hexSize

let resolution = Windowed (sw,sh)
let assets = 
    [
        Texture { key = "hex_flat"; path = "Content/hexFlat" }
        Texture { key = "hex_pointy"; path = "Content/hexPointy" }
        Font { key = "font"; path = "Content/JuraMedium" }
        Texture { key = "pointer"; path = "Content/pointer" }
    ]

let getView runState model =
    let hexes = model.gameTree.board |> List.map (fun o -> o.hex)
    let points = hexes |> List.map (fun h -> h, h |> Hex.toPixel cubeTop hexSize)

    let (ox,oy) = (100.,100.)
    let rectFrom (x,y) = (ox + x) - hexWidth/2.0 |> int, (oy + y) - hexHeight/2.0 |> int, ceil hexWidth |> int, ceil hexHeight |> int 

    let (mx,my) = runState.mouse.position
    let mouseHex = Hex.fromPixel cubeTop hexSize (float mx - ox, float my - oy)

    let texture = match cubeTop with | Flat -> "hex_flat" | _ -> "hex_pointy"
    let images = points |> List.map (fun (hex,point) ->
        let drawable = { assetKey = texture; destRect = rectFrom point; sourceRect = None }
        if mouseHex = hex then ColouredImage (Color.Red, drawable) else Image drawable)

    let cursor = Image { assetKey = "pointer"; destRect = mx,my,27,27; sourceRect = Some (0,0,18,18) }         

    images @ [cursor]