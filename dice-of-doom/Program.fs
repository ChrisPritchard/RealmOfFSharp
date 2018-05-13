open GameCore

[<EntryPoint>]
let main _ =
    use game = new GameCore<Model.DiceGameModel> (View.assets, Controller.updateModel, View.getView)
    game.Run()
    0