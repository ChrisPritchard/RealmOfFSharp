module View
open GameCore
open Hex
open Model
open Microsoft.Xna.Framework

let sw,sh = (1024,768)
let hexSize = 48.
let hexWidth,hexHeight = Hex.width hexTop hexSize, Hex.height hexTop hexSize

let resolution = Windowed (sw,sh)

let assets = 
    [
        Texture { key = "hex_flat"; path = "Content/hexFlat" }
        Texture { key = "hex_pointy"; path = "Content/hexPointy" }
        Font { key = "default"; path = "Content/JuraMedium" }
        Texture { key = "pointer"; path = "Content/pointer" }
        Texture { key = "white"; path = "Content/white" }
    ]

let passButton = (50,50,150,60)

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

let private findAttack predicate moves =
    moves |> Option.bind (List.tryPick 
        (fun (m,_) -> match m with | Attack (a,t) when predicate (a,t) -> Some (a,t) | _ -> None))

let getView runState model =

    let fadeIndex = 
        let chunk = (runState.elapsed / 50.) % 50. |> int |> (*) 10
        if chunk > 255 then 255 - (chunk - 255) |> byte else byte chunk

    let (ox,oy) = calculateOffset model.gameTree.board
    let hexes = 
        model.gameTree.board 
        |> List.map (fun t -> t, Hex.toPixel hexTop hexSize t.hex)
    let texture = match hexTop with | Flat -> "hex_flat" | _ -> "hex_pointy"

    let player index = model.gameOptions.players.[index]

    let hexDisplay = hexes |> List.collect (fun (territory,point) ->
        let drawable = { assetKey = texture; destRect = rectFrom (ox,oy) point; sourceRect = None }
        let colour = player territory.owner |> snd
        let colour = 
            match model.source with
            | None when (findAttack (fun (a,_) -> a = territory) model.gameTree.moves) <> None ->
                new Color (colour.R, colour.G, colour.B, fadeIndex)
            | Some source when (findAttack (fun (a,t) -> a = source && t = territory) model.gameTree.moves) <> None ->
                new Color (colour.R, colour.G, colour.B, fadeIndex)
            | _ -> colour
        let hexImage = ColouredImage (colour, drawable)

        let (px,py) = point
        let labelPos = (int (px + ox), int (py + oy))
        let diceLabel = ColouredText (Color.White, { assetKey = "default"; text = string territory.dice; position = labelPos; origin = Centre; scale=1. })

        [hexImage; diceLabel])
    
    let passButton = 
        let canPass = Option.bind (List.tryPick (fun (m,_) -> match m with | Pass -> Some m | _ -> None)) model.gameTree.moves 
        match canPass with
        | None -> []
        | Some _ ->
            let (px,py,pw,ph) = passButton
            [
                ColouredImage (Color.Black, { assetKey = "white"; destRect = passButton; sourceRect = None })
                ColouredImage (Color.White, { assetKey = "white"; destRect = (px+2,py+2,pw-4,ph-4); sourceRect = None })
                Text { assetKey = "default";text = "Pass";position = (px + pw/2,py + ph/2);origin = Centre;scale=0.6 }
            ]

    let (mx,my) = runState.mouse.position
    let cursor = Image { assetKey = "pointer"; destRect = mx,my,27,27; sourceRect = Some (0,0,18,18) }         

    let text = 
        match model.gameTree.moves with
        | None -> 
            let winner = 
                model.gameTree.board
                |> List.groupBy (fun t -> t.owner)
                |> List.map (fun (o,tl) -> o,List.length tl)
                |> List.maxBy (fun (_,l) -> l)
                |> fst
            let winnerName = player winner |> fst
            [
                Text { 
                assetKey = "default"
                text = "Game Over!"
                position = (sw/2,sh/2)
                origin = Centre;scale=2. }
                Text { 
                assetKey = "default"
                text = sprintf "%s Wins" winnerName
                position = (sw/2,sh/2 + 100)
                origin = Centre;scale=1. }
            ]
        | _ -> [Text { 
            assetKey = "default"
            text = sprintf "%s's Turn" (player model.gameTree.player |> fst)
            position = (sw/2,100)
            origin = Centre;scale=1. }]

    hexDisplay @ passButton @ text @ [cursor]