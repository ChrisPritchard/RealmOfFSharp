open Model
open GameCore

(*
    Implementation of 8.1 Orc Battle
    Again, followed an immutable model. Added some animations from OpenGameArt.org
    (credit to Merry Dream Games: https://opengameart.org/content/mitheral-knight)
    Simplified the battle a little according to the source material, and used a component
    structure for differing enemies rather than OO
*)

[<EntryPoint>]
let main _ =
    use game = new GameCore<BattleModel>(View.assets, Controller.updateModel, View.getView)
    game.Run()
    0
