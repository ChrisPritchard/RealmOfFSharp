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

type GameModel = {
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
let gameStart = { 
    snake = snakeStart
    dir = Dir.South
    goo = randomGoo snakeStart
    lastTick = 0.0
    timeBetweenTicks = 500.0
    loss = false
}

let nextHead model =
    let (cx,cy) = List.head model.snake
    let (nx, ny) = 
        match model.dir with
        | North -> cx, cy - 1
        | East -> cx + 1, cy
        | South -> cx, cy + 1
        | West -> cx - 1, cy
    if nx < 0 || ny < 0 || nx = world.x || ny = world.y
    then None else Some (nx, ny)

let checkForDirChange isPressed model =
    if List.contains model.dir [ Dir.North; Dir.South ] then
        if isPressed Keys.Right then Dir.East
        elif isPressed Keys.Left then Dir.West
        else model.dir
    else
        if isPressed Keys.Up then Dir.North
        elif isPressed Keys.Down then Dir.South
        else model.dir

let advanceSnake model = 
    let next = nextHead model
    match next with
    | None -> 
            { model with loss = true }
    | Some n when List.contains n model.snake -> 
            { model with loss = true }
    | Some n -> 
        if n = model.goo
        then 
            let newSnake = n::model.snake
            let newSpeed = max (model.timeBetweenTicks * 0.9) 100.0
            { model with snake = newSnake; goo = randomGoo newSnake; timeBetweenTicks = newSpeed }
        else
            let truncated = List.take (List.length model.snake - 1) model.snake
            { model with snake = n::truncated }

let updateModel (runState: RunState) currentModel = 
    match currentModel with
    | None -> Some gameStart
    | Some model ->
        let isPressed key = List.contains key runState.keyboard.keysDown
        if model.loss then 
            if isPressed Keys.R 
            then Some { gameStart with goo = randomGoo snakeStart } 
            elif isPressed Keys.Escape
            then None
            else currentModel
        else
            let dir = checkForDirChange isPressed model
            let newState = { model with dir = dir }

            if runState.elapsed - model.lastTick < model.timeBetweenTicks 
            then Some newState
            else 
                let newState = { newState with lastTick = runState.elapsed }
                advanceSnake newState |> Some

let getView _ model =
    let calculatePos (x,y) = 
        10 + (tileSize.x * x), 10 + (tileSize.y * y), 
        tileSize.x, tileSize.y
    let images = 
        [0..world.x - 1] |> List.collect (fun x -> 
        [0..world.y - 1] |> List.map (fun y -> 
            let point = (x,y)
            let key = 
                if model.goo = point then "goo"
                elif List.head model.snake = point then "head"
                elif List.contains point model.snake then "snake"
                else "empty"
            { textureKey = key; destRect = calculatePos point; sourceRect = None }))
    
    if model.loss then
        images, 
        [ { 
            fontKey = "default";
            text = "You Lose!";
            position = 20.0, (world.y / 2) * tileSize.y |> float;
            scale = 0.5
          };
          {
              fontKey = "default";
              text = "Press 'R' to Restart or Escape to exit";
              position = 20.0, (world.y / 2) * tileSize.y + 40 |> float;
              scale = 0.4
          } ]
    else 
        images, []

[<EntryPoint>]
let main _ =
    use game = new GameCore<GameModel> (assets, updateModel, getView)
    game.Run()
    0