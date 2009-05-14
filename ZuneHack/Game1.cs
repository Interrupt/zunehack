using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ZuneHack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D background;
        Rectangle viewportRect;

        GameManager gameManager;

        float waitBeforeQuit = 100;
        float timeWaitedForQuit = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content/Assets";

#if (ZUNE)
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 240;
#else
            graphics.PreferredBackBufferHeight = 240;
            graphics.PreferredBackBufferWidth = 320;
#endif

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        /// <summary>
        /// Initializes the game and loads any components
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewportRect = new Rectangle(0, 0, 320, 240);
            gameManager = GameManager.GetInstance().Initialize(Content);

            base.Initialize();
        }

        /// <summary>
        /// Loads any initial content needed for the game
        /// </summary>
        protected override void LoadContent()
        {
            background = new RenderTarget2D(GraphicsDevice, 320, 240, 1, SurfaceFormat.Color);

            gameManager.LoadTexture(@"Walls\brick-grey");
            gameManager.LoadTexture(@"Walls\brick-mossy");
            gameManager.LoadTexture(@"Walls\brick-bloody");
            gameManager.LoadTexture(@"Walls\brick-torch");
            gameManager.LoadTexture(@"Walls\rockwall");
            gameManager.LoadTexture(@"Walls\dirtwall");
            gameManager.LoadTexture(@"Walls\door");
            gameManager.LoadTexture(@"Walls\door-grate");
            gameManager.LoadTexture(@"Deco\column");
            gameManager.LoadTexture(@"Deco\up");
            gameManager.LoadTexture(@"Deco\down");
            gameManager.LoadTexture(@"background-gradient");
            gameManager.LoadTexture(@"Items\moneybag");
            gameManager.LoadTexture(@"Items\potion-red");
            gameManager.LoadTexture(@"Items\sword");
            gameManager.LoadFont(@"Gebrider");

            gameManager.PushState(new PlayState(gameManager));

            base.LoadContent();
        }

        /// <summary>
        /// Any created content should be unloaded here
        /// </summary>
        protected override void UnloadContent()
        {
        }

        // Updates the game with the elapsed time since the last tick
        protected override void Update(GameTime gameTime)
        {
            float timescale = gameTime.ElapsedGameTime.Milliseconds / 100.0f;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update the game, unless we should quit
            if (!gameManager.doQuit)
            {
                gameManager.Update(timescale);
            }
            else
            {
                timeWaitedForQuit += 2 * timescale;
                if (timeWaitedForQuit > waitBeforeQuit)
                    this.Exit();
            }

            base.Update(gameTime);
        }

        // Draws the game elements
        protected override void Draw(GameTime gameTime)
        {
#if (ZUNE)
            GraphicsDevice.SetRenderTarget(0, background);
#endif
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            gameManager.Draw(spriteBatch);
            spriteBatch.End();

#if (ZUNE)
            GraphicsDevice.SetRenderTarget(0, null);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            spriteBatch.Draw(background.GetTexture(), new Vector2(140,160), null, Color.White, -1.57079633f, new Vector2(160,140), 1, SpriteEffects.None, 1 );
            spriteBatch.End();
#endif

            base.Draw(gameTime);
        }

        // Fake cube mapping //
        /*spriteBatch.Draw(brushtex, new Rectangle(screenWidth * x, -lineHeight / 2 + 240 / 2, screenWidth, lineHeight),
            new Rectangle((texWidth * x), 0, texWidth, brushtex.Height), multColor, 0, new Vector2(), SpriteEffects.None, lineHeight);
         */
    }
}
