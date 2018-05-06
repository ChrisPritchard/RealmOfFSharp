module View
open GameCore

let getView _ = 
    [
        { textureKey = "player"; destRect = 100,100,100,100; sourceRect = Some (0,0,32,32) }
    ], []