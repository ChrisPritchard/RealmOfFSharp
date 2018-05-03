namespace GuessingGameUI

    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics;
    open Microsoft.Xna.Framework.Input;

    type Game1() as this = 
        inherit Game()

        do new GraphicsDeviceManager(this) |> ignore

        let mutable font: SpriteFont = null
        let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

        let mutable counter = 0

        let mutable pressedKeys: Keys list = []
        let mutable keyEvents = Map.empty<Keys, unit -> unit>

        override __.Initialize() =
             base.Initialize()
             
             this.OnPress Keys.Up (fun () -> counter <- counter + 1)
             this.OnPress Keys.Down (fun () -> counter <- counter - 1)
             ()

        override __.LoadContent() = 
            font <- this.Content.Load<SpriteFont>("Content/JuraMedium")
            spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        override __.Update(_) =
            let keyboard = Keyboard.GetState()

            let pressed = keyboard.GetPressedKeys()
            pressed |> Array.filter (fun o -> List.contains o pressedKeys |> not && keyEvents.ContainsKey o)
                |> Array.iter (fun o ->
                    pressedKeys <- o::pressedKeys
                    keyEvents.[o] ())
            pressedKeys <- List.except (pressedKeys |> List.filter (fun o -> Array.contains o pressed |> not)) pressedKeys

            ()

        override __.Draw(_) =
            this.GraphicsDevice.Clear Color.White
            spriteBatch.Begin();
            this.DrawText (sprintf "Count: %i" counter) (new Vector2 (300.0f, 150.0f)) 1.0f
            spriteBatch.End();

        member __.DrawText text position (scale:float32) = 
            let textMiddlePoint = (font.MeasureString: string -> Vector2) (text) / 2.0f;
            spriteBatch.DrawString(font, text, position, Color.Black, 0.0f, textMiddlePoint, scale, SpriteEffects.None, 0.5f)

        member __.OnPress (key: Keys) (run: unit -> unit) =
            keyEvents <- keyEvents.Add (key, run)