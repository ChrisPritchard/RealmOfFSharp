(*
    Implementation of 2.1 Guessing Game, with some differences. 
    Notably, is immutable compared to the presented Racket version
    Also runs as a console app, rather than a REPL only program
    (it can be run in the REPL as well of course)
*)
open System

let rec guess lower upper guesses =
    let attempt = (upper - lower) / 2 + lower
    printfn "\nMy guess is %i" attempt
    printfn "Type 'Higher', 'Lower' or 'Correct' and hit ENTER"

    match Console.ReadLine() with
    | "Higher" -> guess attempt upper (guesses + 1)
    | "Lower" -> guess lower attempt (guesses + 1)
    | "Correct" ->
        printfn "Excellent :) I took %i guesses" guesses
    | _ -> 
        printfn "Invalid input, please try again"
        guess lower upper guesses

let rec runGame () = 
    printfn "Think of a number between 0 and 100"
    printfn "Then type 'Go' and hit ENTER to start"

    while Console.ReadLine() <> "Go" do
        printfn "Invalid input. Type 'Go' to continue"

    guess 0 100 0 |> ignore

    printfn "Press 'q' to quit or any key to restart"
    if Console.ReadKey(true).KeyChar <> 'q' then runGame ()

[<EntryPoint>]
let main _ =
    runGame ()
    0
