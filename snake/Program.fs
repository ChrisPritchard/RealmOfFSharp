open System
open GameWrapper
open Microsoft.Xna.Framework.Input

type Dim = { x: int; y: int }
let world = { x = 20; y = 20 }
let timeBetweenTicks = 1000.0

type GameState = {
    snake: (int * int) list
    dir: Dir
    goo: int * int
    lastTick: float
    loss: bool
} and Dir = | North | East | South | West

let random = new Random ()
let rec randomGoo snake = 
    let newGoo = random.Next world.x, random.Next world.y
    if snake |> List.contains newGoo
    then randomGoo snake else newGoo

let snakeStart = [ world.x / 2, world.y / 2 ]
let initialState = { 
    snake = snakeStart
    dir = Dir.South
    goo = randomGoo snakeStart
    lastTick = 0.0
    loss = false
}

let nextHead state =
    let (cx,cy) = List.head state.snake
    let (nx, ny) = 
        match state.dir with
        | North -> cx, cy - 1
        | East -> cx + 1, cy
        | South -> cx, cy + 1
        | West -> cx - 1, cy
    if nx < 0 || ny < 0 || nx = world.x || ny = world.y
    then None else Some (nx, ny)

let updateState (runState: RunState) gameState = 
    let isPressed key = List.contains key runState.keyboard.keysDown
    if gameState.loss then 
        if isPressed Keys.R then { initialState with goo = randomGoo snakeStart } else gameState
    else
        let dir = 
            if isPressed Keys.Up && gameState.dir <> Dir.North then Dir.North
            elif isPressed Keys.Right && gameState.dir <> Dir.East then Dir.East
            elif isPressed Keys.Down && gameState.dir <> Dir.South then Dir.South
            elif isPressed Keys.Left && gameState.dir <> Dir.West then Dir.West
            else gameState.dir
        let newState = { gameState with dir = dir }
        if runState.elapsed - gameState.lastTick < timeBetweenTicks then newState
        else
            let newState = { newState with lastTick = runState.elapsed }
            let next = nextHead gameState
            match next with
            | None -> { gameState with loss = true }
            | Some n -> 
                if n = gameState.goo
                then 
                    let newSnake = n::gameState.snake
                    { newState with snake = newSnake; goo = randomGoo newSnake }
                else
                    let truncated = List.take (List.length gameState.snake - 1) gameState.snake
                    { newState with snake = n::truncated }

[<EntryPoint>]
let main _ =
    printfn "Hello World from F#!"
    0
