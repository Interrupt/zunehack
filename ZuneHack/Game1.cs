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

        Camera cam;
        Map map;
        Raycaster raycaster;

        GameManager playstate;

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
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cam = new Camera();
            cam.SetPosition(new Vector2(21.5f, 11.5f));
            cam.Turn((float)(Math.PI * 2) / 4.0f);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewportRect = new Rectangle(0, 0, 320, 240);
            playstate = GameManager.GetInstance().Initialize(cam, Content);
            raycaster = new Raycaster(cam, spriteBatch, -0.35f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            background = new RenderTarget2D(GraphicsDevice, 320, 240, 1, SurfaceFormat.Color);

            playstate.LoadTexture(@"Walls\brick-grey");
            playstate.LoadTexture(@"Walls\brick-mossy");
            playstate.LoadTexture(@"Walls\brick-bloody");
            playstate.LoadTexture(@"Walls\brick-torch");
            playstate.LoadTexture(@"Walls\door");
            playstate.LoadTexture(@"Walls\door-grate");
            playstate.LoadTexture(@"Deco\column");
            playstate.LoadTexture(@"Actors\goblin");
            playstate.LoadTexture(@"Actors\kobold");
            playstate.LoadTexture(@"Actors\rat");
            playstate.LoadTexture(@"background-gradient");

            playstate.LoadFont(@"Gebrider");

            //map = new Map(1, MapType.dungeon);
            map = new Map(1, MapType.dungeon);
            playstate.SetMap(map);
            raycaster.SetMap(map);

            raycaster.BackgroundGradient = playstate.GetTexture("background-gradient");

            cam.SetPosition(map.GetStairUpLoc());

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float timescale = gameTime.ElapsedGameTime.Milliseconds / 100.0f;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update the game, unless we should quit
            if (!playstate.doQuit)
            {
                playstate.Update(timescale);
                raycaster.Update();
            }
            else
            {
                timeWaitedForQuit += 2 * timescale;
                if (timeWaitedForQuit > waitBeforeQuit)
                    this.Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
#if (ZUNE)
            GraphicsDevice.SetRenderTarget(0, background);
#endif
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            raycaster.Draw();
            spriteBatch.End();

            spriteBatch.Begin();
            playstate.Draw(spriteBatch);
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
