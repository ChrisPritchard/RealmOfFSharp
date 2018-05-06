namespace GameCore

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics;
open Microsoft.Xna.Framework.Input;
open System

type GameCore<'TState> (config: GameConfig<'TState>) as this = 
    inherit Game()

    do new GraphicsDeviceManager(this) |> ignore

    let mutable textureAssets = Map.empty<string, Texture2D>
    let mutable fontAssets = Map.empty<string, SpriteFont>

    let mutable keyboardInfo = { pressed = []; keysDown = []; keysUp = [] }
    let mutable gameState = config.initialState
    let mutable currentView: DrawableImage list * DrawableText list = [],[]

    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let updateKeyboardInfo (keyboard: KeyboardState) existing =
        let pressed = keyboard.GetPressedKeys() |> Set.ofArray
        {
            pressed = pressed |> Set.toList;
            keysDown = Set.difference pressed (existing.pressed |> Set.ofList) |> Set.toList;
            keysUp = Set.difference (existing.pressed |> Set.ofList) pressed |> Set.toList
        }

    let asVector2 (x:float,y:float) = new Vector2(float32 x, float32 y)
    let asRectangle (x,y,width,height) = 
        new Rectangle (x,y,width,height)

    override __.LoadContent() = 
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        textureAssets <- config.loadAssets
            |> List.filter (fun a -> a.assetType = AssetType.Texture) 
            |> List.map (fun t -> t.key, this.Content.Load<Texture2D>(t.path))
            |> Map.ofList
        fontAssets <- config.loadAssets 
            |> List.filter (fun a -> a.assetType = AssetType.Font) 
            |> List.map (fun f -> f.key, this.Content.Load<SpriteFont>(f.path))
            |> Map.ofList

    override __.Update(gameTime) =
        keyboardInfo <- updateKeyboardInfo (Keyboard.GetState()) keyboardInfo
        let runState = { keyboard = keyboardInfo; elapsed = gameTime.TotalGameTime.TotalMilliseconds }
        gameState <- config.updateState runState gameState
        currentView <- config.getView gameState

    override __.Draw(_) =
        this.GraphicsDevice.Clear Color.White
        spriteBatch.Begin()

        currentView
            |> fst
            |> List.map (fun d -> d,textureAssets.[d.textureKey])
            |> List.iter (fun (d,texture) ->
                let sourceRect = 
                    match d.sourceRect with 
                    | None -> Unchecked.defaultof<Nullable<Rectangle>> 
                    | Some r -> asRectangle r |> Nullable
                spriteBatch.Draw(
                    texture, asRectangle d.destRect, 
                    sourceRect, Color.White, 0.0f, Vector2.Zero, 
                    SpriteEffects.None, 0.5f))

        currentView 
            |> snd
            |> List.map (fun d -> d,fontAssets.[d.fontKey])
            |> List.iter (fun (d,font) ->
                spriteBatch.DrawString(
                    font, d.text, asVector2 d.position, Color.Black, 
                    0.0f, Vector2.Zero, float32 d.scale, SpriteEffects.None, 0.5f))

        spriteBatch.End()