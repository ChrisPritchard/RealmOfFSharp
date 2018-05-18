module View
open GameCore
open Hex
open Model
open Microsoft.Xna.Framework

let sw,sh = (1024,768)
let hexSize = 64.
let hexWidth,hexHeight = Hex.width hexTop hexSize, Hex.height hexTop hexSize

let resolution = Windowed (sw,sh)

let assets = 
    [
        Texture { key = "hex_flat"; path = "Content/hexFlat" }
        Texture { key = "hex_pointy"; path = "Content/hexPointy" }
        Font { key = "default"; path = "Content/JuraMedium" }
        Texture { key = "pointer"; path = "Content/pointer" }
    ]

let passButton = (50,50,200,50)

let private rectFrom (ox,oy) (x,y) = 
    (ox + x) - hexWidth/2.0 |> int, 
    (oy + y) - hexHeight/2.0 |> int, 
    ceil hexWidth |> int, 
    ceil hexHeight |> int 

let calculateOffset board = 
    let points = board |> List.map (fun t -> Hex.toPixel hexTop hexSize t.hex)
    let xplane = List.map fst points
    let yplane = List.map snd points
    (float sw - (List.max xplane - List.min xplane)) / 2.,
    (float sh - (List.max yplane - List.min yplane)) / 2.

let getView runState model =

    // hexes
        // colour
        // fading on select / options
    // dice numbers
    // pass button visibility
    // player turn display
    // game over / reset

    let (ox,oy) = calculateOffset model.gameTree.board
    let hexes = 
        model.gameTree.board 
        |> List.map (fun t -> t, Hex.toPixel hexTop hexSize t.hex)
    let texture = match hexTop with | Flat -> "hex_flat" | _ -> "hex_pointy"

    let hexDisplay = hexes |> List.collect (fun (territory,point) ->
        let drawable = { assetKey = texture; destRect = rectFrom (ox,oy) point; sourceRect = None }
        let (px,py) = point
        let labelPos = (int (px + ox), int (py + oy))
        let diceLabel = ColouredText (Color.White, { assetKey = "default";text = string territory.dice;position = labelPos;origin = Centre;scale=1. })
        match territory.owner with
        | 0 -> 
            [ColouredImage (Color.Red, drawable); diceLabel]
        | _ -> 
            [ColouredImage (Color.Blue, drawable); diceLabel])

    let (mx,my) = runState.mouse.position
    let cursor = Image { assetKey = "pointer"; destRect = mx,my,27,27; sourceRect = Some (0,0,18,18) }         

    hexDisplay @ [cursor]