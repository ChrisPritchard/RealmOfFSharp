namespace GameWrapper

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics;
open Microsoft.Xna.Framework.Input;

type GameWrapper<'TState> (config: GameConfig<'TState>) as this = 
    inherit Game()

    do new GraphicsDeviceManager(this) |> ignore

    let mutable fontAssets = Map.empty<string, SpriteFont>

    let mutable keyboardInfo = { pressed = []; keysDown = []; keysUp = [] }
    let mutable gameState = config.initialState
    let mutable currentView: TextInfo list = []

    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let updateKeyboardInfo (keyboard: KeyboardState) existing =
        let pressed = keyboard.GetPressedKeys() |> Set.ofArray
        {
            pressed = pressed |> Set.toList;
            keysDown = Set.difference pressed (existing.pressed |> Set.ofList) |> Set.toList;
            keysUp = Set.difference (existing.pressed |> Set.ofList) pressed |> Set.toList
        }

    let asVector2 (position: float * float) = new Vector2(fst position |> float32, snd position |> float32)

    override __.LoadContent() = 
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        fontAssets <- config.loadAssets 
            |> List.filter (fun o -> o.assetType = AssetType.Font) 
            |> List.map (fun f -> f.key, this.Content.Load<SpriteFont>(f.path))
            |> Map.ofList

    override __.Update(_) =
        keyboardInfo <- updateKeyboardInfo (Keyboard.GetState()) keyboardInfo
        gameState <- config.updateState keyboardInfo gameState
        currentView <- config.getView gameState

    override __.Draw(_) =
        this.GraphicsDevice.Clear Color.White
        spriteBatch.Begin()
        
        currentView 
            |> List.map (fun d -> d,fontAssets.[d.fontKey])
            |> List.iter (fun (d,font) ->
                spriteBatch.DrawString(
                    font, d.text, asVector2 d.position, Color.Black, 
                    0.0f, Vector2.Zero, float32 d.scale, SpriteEffects.None, 0.5f))

        spriteBatch.End()