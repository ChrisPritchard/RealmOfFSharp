open GameCore

[<EntryPoint>]
let main _ =
    use game = new GameCore<Model.DiceGameModel> (Windowed (1024,768), View.assets, Controller.updateModel, View.getView)
    game.Run()
    0