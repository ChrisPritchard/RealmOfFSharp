open Model
open View
open GameCore

[<EntryPoint>]
let main _ =
    let assets = [
        { key = "player"; assetType = AssetType.Texture; path = "Content/MitheralKnight" }
        { key = "orc_club"; assetType = AssetType.Texture; path = "Content/HunterOrc" }
        { key = "orc_spear"; assetType = AssetType.Texture; path = "Content/LuckyOrc" }
        { key = "orc_whip"; assetType = AssetType.Texture; path = "Content/RedOrc" }
    ]

    let config = { 
        loadAssets = assets; 
        initialState = Unchecked.defaultof<Battle>; 
        updateState = fun _ btl -> btl; 
        getView = getView }

    let game = new GameCore<Battle>(config)
    game.Run()
    0
