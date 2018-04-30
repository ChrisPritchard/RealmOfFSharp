module MyGameEntry

open GuessingGameUI

[<EntryPoint>]
let main _ =
    use game = new Game1()
    game.Run()
    0