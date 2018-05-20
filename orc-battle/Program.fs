open Model
open GameCore

[<EntryPoint>]
let main _ =
    use game = new GameCore<BattleModel>(Windowed (800,600), View.assets, Controller.updateModel, View.getView)
    game.Run()
    0
