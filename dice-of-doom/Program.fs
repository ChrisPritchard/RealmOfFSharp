open GameCore

[<EntryPoint>]
let main _ =
    use game = new GameCore<Model.DiceGameModel> (FullScreen (1920,1080), View.assets, Controller.updateModel, View.getView)
    game.Run()
    0