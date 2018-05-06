open System
open GameCore
open Microsoft.Xna.Framework.Input

(*
    Implementation of 6.1 Robot Snake
    I have implemented a more traditional snake game here,
    with increasing speeds and only one goo at a time, compared to 
    the source problem which had aging goos
*)

let assets = [
    { key = "default"; assetType = AssetType.Font; path = "Content/JuraMedium" }
    { key = "empty"; assetType = AssetType.Texture; path = "Content/empty" }
    { key = "head"; assetType = AssetType.Texture; path = "Content/head" }
    { key = "snake"; assetType = AssetType.Texture; path = "Content/snake" }
    { key = "goo"; assetType = AssetType.Texture; path = "Content/goo" }
]

type Dim = { x: int; y: int }
let world = { x = 15; y = 15 }
let tileSize = { x = 30; y = 30 }

type GameState = {
    snake: (int * int) list
    dir: Dir
    goo: int * int
    lastTick: float
    timeBetweenTicks: float
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
    timeBetweenTicks = 500.0
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

let checkForDirChange isPressed gameState =
    if List.contains gameState.dir [ Dir.North; Dir.South ] then
        if isPressed Keys.Right then Dir.East
        elif isPressed Keys.Left then Dir.West
        else gameState.dir
    else
        if isPressed Keys.Up then Dir.North
        elif isPressed Keys.Down then Dir.South
        else gameState.dir

let advanceSnake gameState = 
    let next = nextHead gameState
    match next with
    | None -> 
            { gameState with loss = true }
    | Some n when List.contains n gameState.snake -> 
            { gameState with loss = true }
    | Some n -> 
        if n = gameState.goo
        then 
            let newSnake = n::gameState.snake
            let newSpeed = max (gameState.timeBetweenTicks * 0.9) 100.0
            { gameState with snake = newSnake; goo = randomGoo newSnake; timeBetweenTicks = newSpeed }
        else
            let truncated = List.take (List.length gameState.snake - 1) gameState.snake
            { gameState with snake = n::truncated }

let updateState (runState: RunState) gameState = 
    let isPressed key = List.contains key runState.keyboard.keysDown
    if gameState.loss then 
        if isPressed Keys.R 
        then { initialState with goo = randomGoo snakeStart } 
        else gameState
    else
        let dir = checkForDirChange isPressed gameState
        let newState = { gameState with dir = dir }

        if runState.elapsed - gameState.lastTick < gameState.timeBetweenTicks 
        then newState
        else 
            let newState = { newState with lastTick = runState.elapsed }
            advanceSnake newState

let getView gameState =
    let calculatePos (x,y) = 
        10 + (tileSize.x * x) |> float, 10 + (tileSize.y * y) |> float
    let images = 
        [0..world.x - 1] |> List.collect (fun x -> 
        [0..world.y - 1] |> List.map (fun y -> 
            let point = (x,y)
            let key = 
                if gameState.goo = point then "goo"
                elif List.head gameState.snake = point then "head"
                elif List.contains point gameState.snake then "snake"
                else "empty"
            { textureKey = key; position = calculatePos point; size = float tileSize.x, float tileSize.y }))
    
    if gameState.loss then
        images, 
        [ { 
            fontKey = "default";
            text = "You Lose!";
            position = 20.0, (world.y / 2) * tileSize.y |> float;
            scale = 0.5
          };
          {
              fontKey = "default";
              text = "Press 'R' to Restart";
              position = 20.0, (world.y / 2) * tileSize.y + 40 |> float;
              scale = 0.5
          } ]
    else 
        images, []

[<EntryPoint>]
let main _ =
    let config = { loadAssets = assets; initialState = initialState; updateState = updateState; getView = getView }
    use game = new GameCore<GameState> (config)
    game.Run()
    0