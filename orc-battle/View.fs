module View
open GameCore

let playerAnims = [0..9] |> List.map (fun i -> (i * 32,0,32,32))

let getView runState _ = 
    let playerFrame = playerAnims.[(runState.elapsed / 100.0) % 10.0 |> int]
    [
        //{ textureKey = "player"; destRect = 100,100,100,100; sourceRect = Some playerFrame }
        { textureKey = "black"; destRect = 100,100,400,30; sourceRect = None }
        { textureKey = "green"; destRect = 102,102,300,26; sourceRect = None }
        { textureKey = "white"; destRect = 402,102,96,26; sourceRect = None }
    ], []