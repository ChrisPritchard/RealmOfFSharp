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
        { key = "green"; assetType = AssetType.Texture; path = "Content/green" }
        { key = "red"; assetType = AssetType.Texture; path = "Content/red" }
        { key = "black"; assetType = AssetType.Texture; path = "Content/black" }
        { key = "white"; assetType = AssetType.Texture; path = "Content/white" }
    ]

    let config = { 
        loadAssets = assets; 
        initialState = Unchecked.defaultof<Battle>; 
        updateState = fun _ btl -> btl; 
        getView = getView }

    use game = new GameCore<Battle>(config)
    game.Run()
    0
