module View
open GameCore

let playerAnims = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

let getView runState _ = 
    let playerFrame = playerAnims.[(runState.elapsed / 100.0) % 10.0 |> int]
    [
        { textureKey = "player"; destRect = 100,100,100,100; sourceRect = Some playerFrame }
    ], []