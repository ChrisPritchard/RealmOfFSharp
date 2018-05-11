module View
open GameCore

let playerAnims = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

let barElements colourKey rect percent = 
    seq {
        yield { textureKey = "black"; destRect = rect; sourceRect = None }
        let (x,y,width,height) = rect
        yield { textureKey = "white"; destRect = x+2,y+2,width-4,height-4; sourceRect = None }
        let valueWidth = width - 4 |> float |> (*) percent |> int
        yield { textureKey = colourKey; destRect = x+2,y+2,valueWidth,height-4; sourceRect = None }
    } |> Seq.toList

let getView runState _ = 
    let playerFrame = playerAnims.[(runState.elapsed / 100.0) % 10.0 |> int]
    [
        //{ textureKey = "player"; destRect = 100,100,100,100; sourceRect = Some playerFrame }
        barElements "green" (100,100,400,30) 0.5
    ] |> List.concat, []