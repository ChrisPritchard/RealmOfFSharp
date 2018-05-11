open Model
open GameCore

[<EntryPoint>]
let main _ =
    use game = new GameCore<BattleModel>(View.assets, Controller.updateModel, View.getView)
    game.Run()
    0
