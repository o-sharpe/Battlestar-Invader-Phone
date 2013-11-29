using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Windows;

namespace BattlestarInvader
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{

		#region Declarations

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		// FPS
		int totalFrames = 0;
		float elapsedTime = 0.0f;
		int fps = 0;

		// Fonts & Graphix
		SpriteFont pericles14;
		SpriteFont pericles8;
		Texture2D star;
		Texture2D battleStar;
		Texture2D turret;
		Texture2D joystick;
		Texture2D laser;
		Texture2D raider;
		Texture2D baseStar;
		Texture2D boom;

		RenderTarget2D renderTarget;
		Vector2 scale;
		Matrix scalematrix;

		#endregion

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			this.graphics.PreferredBackBufferWidth = 720;
			this.graphics.PreferredBackBufferHeight = 1280;

			scale = new Vector2( (float)System.Windows.Application.Current.Host.Content.ActualWidth / (float)this.graphics.PreferredBackBufferWidth, (float)System.Windows.Application.Current.Host.Content.ActualHeight /(float)this.graphics.PreferredBackBufferHeight);
			scalematrix = Matrix.CreateScale(scale.Y, scale.X, 1f);

			// We set the camera
			Screen.Camera.WorldRectangle = new Rectangle(0, 0, this.graphics.PreferredBackBufferHeight, this.graphics.PreferredBackBufferWidth);
			Screen.Camera.ViewPortWidth = this.graphics.PreferredBackBufferHeight;
			Screen.Camera.ViewPortHeight = this.graphics.PreferredBackBufferWidth;
			Screen.Camera.PhoneHeightAndWidth = new Vector2((float)Application.Current.Host.Content.ActualHeight, (float)Application.Current.Host.Content.ActualWidth);
			Screen.Camera.Scale = scale;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			renderTarget = new RenderTarget2D(GraphicsDevice, this.graphics.PreferredBackBufferHeight, this.graphics.PreferredBackBufferWidth, false,
				SurfaceFormat.Color, DepthFormat.Depth16);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");
			pericles8 = Content.Load<SpriteFont>(@"Fonts\Pericles8");
			star = Content.Load<Texture2D>(@"Graphix\star");
			battleStar = Content.Load<Texture2D>(@"Graphix\battleStar");
			turret = Content.Load<Texture2D>(@"Graphix\turret");
			joystick = Content.Load<Texture2D>(@"Graphix\joystick");
			laser = Content.Load<Texture2D>(@"Graphix\greenLaserRay");
			raider = Content.Load<Texture2D>(@"Graphix\raider");
			baseStar = Content.Load<Texture2D>(@"Graphix\baseStar");
			boom = Content.Load<Texture2D>(@"Graphix\boom");

			// We Initialize the Managers
			Screen.StarField.Initialize(this.graphics.PreferredBackBufferHeight, this.graphics.PreferredBackBufferWidth, 400, Vector2.Zero, star, new Rectangle(0, 0, 2, 2));
			Battlestar.BattleStar.Initialize(battleStar, new Rectangle(0, 0, 300, 109), 1, turret, new Rectangle(0, 0, 44, 30), new Vector2(this.graphics.PreferredBackBufferHeight / 2 - 150, this.graphics.PreferredBackBufferWidth / 3 * 2));
			for (int x = 0; x < Battlestar.BattleStar.TurretSprites.Count; x++)
				HUD.Buttons.AddButton(turret, new Rectangle(0, 0, 44, 30), new Vector2(this.graphics.PreferredBackBufferHeight - 70, 200), pericles14);
			HUD.Joystick.Initialize(joystick, new Rectangle(0, 0, 180, 180), new Vector2(0, 250));
			Weapons.WeaponManager.Initialize(laser, new Rectangle(0, 0, 31, 8));
			EnemyManager.EnemyManager.Initialize(raider, new Rectangle(0, 0, 25, 16), baseStar, new Rectangle(0, 0, 300, 146));
			Screen.Effects.Initialize(star, new Rectangle(0, 0, 2, 2), boom, new Rectangle(0, 0, 64, 64));

			// TEMPS
			EnemyManager.EnemyManager.AddBaseStar(new Vector2(640, 360));
			EnemyManager.EnemyManager.AddRaider(new Vector2(640, 360));
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// TODO: Add your update logic here
			Battlestar.BattleStar.Update(gameTime);
			Weapons.WeaponManager.Update(gameTime);
			EnemyManager.EnemyManager.Update(gameTime);
			Screen.Effects.Update(gameTime);

			// FPS
			elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			// 1 Second has passed
			if (elapsedTime >= 1000.0f)
			{
				fps = totalFrames;
				totalFrames = 0;
				elapsedTime = 0;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{

			// -------------------------------------
			// -- render to the render target buffer
			// -------------------------------------
			GraphicsDevice.SetRenderTarget(renderTarget); // set our target to buffer
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, scalematrix);

			// We Draw the Managers
			Battlestar.BattleStar.Draw(spriteBatch);
			Screen.StarField.Draw(spriteBatch);
			HUD.Buttons.Draw(spriteBatch);
			HUD.Joystick.Draw(spriteBatch);
			Weapons.WeaponManager.Draw(spriteBatch);
			EnemyManager.EnemyManager.Draw(spriteBatch);
			Screen.Effects.Draw(spriteBatch);

			// FPS
			totalFrames++;
			spriteBatch.DrawString(pericles14, string.Format("FPS: {0}", fps), new Vector2(30, 25), Color.White);

			spriteBatch.End();

			// -------------------------------------
			// call the base Draw() method 
			// -------------------------------------
			base.Draw(gameTime);

			// -------------------------------------
			// -- draw the render target buffer to the screen with 90 degree rotation
			// -------------------------------------
			GraphicsDevice.SetRenderTarget(null); // set our target to screen
			spriteBatch.Begin();

			spriteBatch.Draw(renderTarget, new Vector2(240, 400),
				null, Color.White, MathHelper.PiOver2,
				new Vector2(400, 240), 1f, SpriteEffects.None, 0);

			spriteBatch.End();
		}
	}
}
