module View
open GameCore

let getView _ = 
    [
        { textureKey = "player"; position = 100.0,100.0; size = 20.0,60.0 }
    ], []