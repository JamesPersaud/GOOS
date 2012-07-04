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
using GOOS.JFX.Core;
using GOOS.JFX.Entity;
using GOOS.JFX.Game;
using GOOS.JFX.Level;
using GOOS.JFX.Scripting;
using GOOS.JFX.Game.Components;
using GOOS.JFX.UI;
using GOOS.JFX.UI.Controls;
using GOOS.MonsterMaze.Forms;

namespace GOOS.MonsterMaze
{
	/// <summary>
	/// 
	/// 3D Creature Cave by Games out of Space
	/// 
	/// A remake of 3D Monster Maze (sort of)
	/// 
	/// Game Programming & Design
	/// James Persaud
	/// 
	/// JFX Engine
	/// James Persaud
	/// 
	/// Graphics 
	/// James Persaud
	/// 
	/// Additional Textures 
	/// Benjamin Röhling
	/// Herbert Fahrnholz
	/// 
	/// Music
	/// Angus Arnold
	/// 
	/// Sound Effects
	/// Partners In Rhyme
	/// 
	/// Original Concept
	/// Malcolm Evans
	/// 3D Monster Maze (1981)
	/// 
	/// TODO:
	/// 
    /// 
	/// Options screen:
	/// 
	/// What should the options be?
	/// 
	/// display mode		(slider) fullscreen	(checkbox)
	/// 
	/// view distance		(slider) 1-100
	/// bloom intensity		(slider) 1-100 off  (checkbox)
	/// particle density	(slider) 1-100	
	/// texture Filtering	checklist (none, point, linear, anisotropic)
	/// 
	/// music volume		(slider) 1-100 mute (checkbox)
	/// sfx volume			(slider) 1-100 mute (checkbox)
	/// 
	/// 
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		#region Members



		#region particle system fields
		//Particle system test
		// This sample uses five different particle systems.
		//ParticleSystem explosionParticles;
		//ParticleSystem explosionSmokeParticles;
		//ParticleSystem projectileTrailParticles;
		ParticleSystem smokePlumeParticles;
		ParticleSystem fireParticles;
		AntiFireParticleSystem antifireParticles;


		// The sample can switch between three different visual effects.
		enum ParticleState
		{
			Explosions,
			SmokePlume,
			RingOfFire,
		};

		ParticleState currentState = ParticleState.SmokePlume;

		// The explosions effect works by firing projectiles up into the
		// air, so we need to keep track of all the active projectiles.
		List<Projectile> projectiles = new List<Projectile>();

		TimeSpan timeToNextProjectile = TimeSpan.Zero;			

		#endregion

		//Game options
		ExtraConfig ExtraConfigFile;
		float OptionsMusicVolume;
		float OptionsSFXVolume;
		bool OptionsMuteMusic;
		bool OptionsMuteSFX;
		float OptionsDisplayModeWidth;
		float OptionsDisplayModeHeight;
		bool OptionsFullscreen;
		float OptionsViewDistance;
		float OptionsBloomLevel;
		bool OptionsBloomOn;
		float OptionsTextureSampler;
		float OptionsParticleDensity;

		//Bloom
		BloomComponent bloom;
		int bloomSettingsIndex = 0;		

		//SFX
		SoundEffect Wind;
		SoundEffect Heart;
		SoundEffect Scream;
		SoundEffect NearMonster;
		SoundEffectInstance Ambient_Wind;
		SoundEffectInstance Ambient_Monster;
		SoundEffectInstance Ambient_Heart;
		SoundEffectInstance Event_Scream;

		//XNA
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		GraphicsDevice device;
		BasicEffect basicEffect;
		SpriteFont font;
		SpriteFont smallfont;
        DisplayModeCollection AvailableModes;
        List<DisplayMode> ModeList;

		//Materials and Lights
		Vector2 safeBounds;
		Material BasicMaterial;
		Material WallMaterial;
		Material FloorMaterial;
		Material CeilingMaterial;
		PointLight PlayerLight;
		PointLight MonsterLight;
		PointLight TeleporterLight;
		Matrix meshRotation;
		Matrix projection;
		QuakeCamera fpsCam;
		string shaderVersionString;
		int numLights;
		private Effect baseEffect;
		private EffectParameter viewParameter;
		private EffectParameter projectionParameter;
		private EffectParameter cameraPositionParameter;

		//JFX
		StageManager Stage;
		Actor monster;
		Actor demoZombie;
		UIComponent UI;
		HiScoreTable HiScores;
		ScoreInfo CurrentScoreInfo;

		//jumping
		float currentjump;
		bool jumping;
		bool falling;

		//Sound
		bool MusicPlaying;
		Song BackgroundMusic;
		Song Music2;

		//Torch
		float flickerdelay = 0;

		//random numbers
		Random random;

		//Testmodels
		Model testmodel;
		Model monsterModel;

		//minimap
		Texture2D MiniMap_Open, MiniMap_Closed, MiniMap_Player, MiniMap_Monster;
		List<Point> VisitedGridSquares;
		Point LastVisitedSquare;

		//compass
		Texture2D CompassTexture;
		Texture2D CompassSkull;
		Texture2D CompassArrow;
		Texture2D CompassPointer;

		//banner
		Texture2D Banner;

		//Point spriters
		Effect pointSpritesEffect;
		VertexPositionColor[] spriteArray;
		VertexDeclaration vertexPosColDecl;

		//Teleporter test
		Vector3 TeleporterLocation;
		Vector3 TeleporterRenderLocation;
		Vector2 TeleporterGridLocation;

		//Warnings
		bool HearMonster;
		bool SeeMonster;

		//Score --- for each level the score*level is awarded. Each level makes the monster faster
		int Level;
		int Score;
		int ScorePerLevel = 100;
		int TimeBonusTarget = 200;
		float MonsterSpeedupFactor = 0.008f;
		int TimeAtLevelStart =0;
		int TimeOnLevel =0;
		int TimeInGame = 0;

		//pause
		float lastpause=0;

		#endregion

		#region Constructor

		public Game1()
		{
			//General XNA setup
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
			graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

			//Enumerate the available display modes
			// graphicsadapter has supporteddisplaymodes

			GraphicsAdapter gdi = GraphicsAdapter.DefaultAdapter;
            AvailableModes = gdi.SupportedDisplayModes;

            ModeList = new List<DisplayMode>();
            foreach (DisplayMode d in AvailableModes)
            {
                ModeList.Add(d);
            }

            //((DisplayMode)Modes[0]).Width
			

			//Load configuration
			Stage = StageManager.CreateFromConfigFile("monstermaze.config");
			graphics.PreferredBackBufferWidth = Stage.ConfigurationSettings.width;
			graphics.PreferredBackBufferHeight = Stage.ConfigurationSettings.Height;

            //graphics.PreferredBackBufferWidth = 1600;
            //graphics.PreferredBackBufferHeight = 1200;

			graphics.IsFullScreen = Stage.ConfigurationSettings.Fullscreen;

			OptionsFullscreen = Stage.ConfigurationSettings.Fullscreen;
			OptionsDisplayModeHeight = Stage.ConfigurationSettings.Height;
			OptionsDisplayModeWidth = Stage.ConfigurationSettings.width;
            
			try
			{
                //Load the extra config file.
				ExtraConfigFile = ExtraConfig.LoadFromFile("CCExtra.config");
			}
			catch
			{
                //If the extra config file isn't found (or can't be loaded) create a new one with default settings and save that.
				ExtraConfigFile = new ExtraConfig();
				ExtraConfigFile.ExtraBoolSettings = new Dictionary<string, bool>();
				ExtraConfigFile.ExtraFloatSettings = new Dictionary<string, float>();
				ExtraConfigFile.SaveTofile("CCExtra.config");
			}

			//floats
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("ViewDistance"))
				ExtraConfigFile.ExtraFloatSettings["ViewDistance"] = 50.0f;
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("SoundVolume"))
				ExtraConfigFile.ExtraFloatSettings["SoundVolume"] = 50.0f;
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("MusicVolume"))
				ExtraConfigFile.ExtraFloatSettings["MusicVolume"] = 40.0f;
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("BloomLevel"))
				ExtraConfigFile.ExtraFloatSettings["BloomLevel"] = 50.0f;
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("ParticleDensity"))
				ExtraConfigFile.ExtraFloatSettings["ParticleDensity"] = 100.0f;
			if (!ExtraConfigFile.ExtraFloatSettings.Keys.Contains("TextureSampler"))
				ExtraConfigFile.ExtraFloatSettings["TextureSampler"] = 99.0f;

			OptionsBloomLevel = ExtraConfigFile.ExtraFloatSettings["BloomLevel"];
			OptionsParticleDensity = ExtraConfigFile.ExtraFloatSettings["ParticleDensity"];
			OptionsSFXVolume = ExtraConfigFile.ExtraFloatSettings["SoundVolume"];
			OptionsTextureSampler = ExtraConfigFile.ExtraFloatSettings["TextureSampler"];
			OptionsViewDistance = ExtraConfigFile.ExtraFloatSettings["ViewDistance"];
			OptionsMusicVolume = ExtraConfigFile.ExtraFloatSettings["MusicVolume"];

			//bools
			if (!ExtraConfigFile.ExtraBoolSettings.Keys.Contains("BloomOn"))
				ExtraConfigFile.ExtraBoolSettings["BloomOn"] = true;
			if (!ExtraConfigFile.ExtraBoolSettings.Keys.Contains("SFXMute"))
				ExtraConfigFile.ExtraBoolSettings["SFXMute"] = false;
			if (!ExtraConfigFile.ExtraBoolSettings.Keys.Contains("MusicMute"))
				ExtraConfigFile.ExtraBoolSettings["MusicMute"] = false;

			OptionsBloomOn = ExtraConfigFile.ExtraBoolSettings["BloomOn"];
			OptionsMuteMusic = ExtraConfigFile.ExtraBoolSettings["MusicMute"];
			OptionsMuteSFX = ExtraConfigFile.ExtraBoolSettings["SFXMute"];			
					

			graphics.PreferredDepthStencilFormat = DepthFormat.Depth32;
			
			random = new Random();
			for(int i=0;i<10;i++)
				random.Next(1,1000);			

			//Particles
			#region Particle system test
			// Construct our particle system components.
			//explosionParticles = new ExplosionParticleSystem(this, Content);
			//explosionSmokeParticles = new ExplosionSmokeParticleSystem(this, Content);
			//projectileTrailParticles = new ProjectileTrailParticleSystem(this, Content);
			smokePlumeParticles = new SmokePlumeParticleSystem(this, Content,(int)OptionsParticleDensity);
			fireParticles = new FireParticleSystem(this, Content, (int)OptionsParticleDensity);
			antifireParticles = new AntiFireParticleSystem(this, Content, (int)OptionsParticleDensity);

			// Set the draw order so the explosions and fire
			// will appear over the top of the smoke.
			//smokePlumeParticles.DrawOrder = 
			//explosionSmokeParticles.DrawOrder = 200;
			//projectileTrailParticles.DrawOrder = 300;
			//explosionParticles.DrawOrder = 400;
			//fireParticles.DrawOrder = 500;

			// Register the particle system components.
			//Components.Add(explosionParticles);
			//Components.Add(explosionSmokeParticles);
			//Components.Add(projectileTrailParticles);
			//Components.Add(smokePlumeParticles);
			//Components.Add(fireParticles);
			#endregion

			//Bloom
			bloom = new BloomComponent(this);
			bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
			bloom.FadeSettings = new PostFadeSettings();

			//Components.Add(bloom);	

			bloom.Settings.BloomIntensity = 3.0f * (OptionsBloomLevel / 100);
		}

		#endregion

		#region Initialize Method

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Score = 0;
			Level = 0;

			this.Window.Title = "3D Creature Cave";
			// create a default world and matrix
			meshRotation = Matrix.Identity;

			currentjump = 0;
			jumping = false;
			falling = false;

			MusicPlaying = false;

			smokePlumeParticles.Initialize();
			fireParticles.Initialize();
			antifireParticles.Initialize();
			bloom.Initialize();
			base.Initialize();
		}

		#endregion

		#region Load Content Method

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			//SFX
			Wind = Content.Load<SoundEffect>("windy2");
			Heart = Content.Load<SoundEffect>("heart_beating_at_normal_speed");
			Scream = Content.Load<SoundEffect>("bloodscream");
			NearMonster = Content.Load<SoundEffect>("this_way_comes_2");		

			//Models
			testmodel = Content.Load<Model>("box");
			monsterModel = Content.Load<Model>("SphereHighPoly");
			//Music
			BackgroundMusic = Content.Load<Song>("MonsterMusicTest");
			Music2 = Content.Load<Song>("breifing");
			//Mini Map
			MiniMap_Closed = Content.Load<Texture2D>("minimap_closed");
			MiniMap_Monster = Content.Load<Texture2D>("minimap_monster");
			MiniMap_Open = Content.Load<Texture2D>("minimap_open");
			MiniMap_Player = Content.Load<Texture2D>("minimap_player");
			//compass
			CompassTexture = Content.Load<Texture2D>("gamecompass2");
			CompassSkull = Content.Load<Texture2D>("skull");
			CompassArrow = Content.Load<Texture2D>("tele");
			CompassPointer = Content.Load<Texture2D>("compasspointer");
			//Banner
			Banner = Content.Load<Texture2D>("banner");
			//Point Spirtes
			pointSpritesEffect = Content.Load<Effect>("Effect\\pointsprites");
			pointSpritesEffect.Parameters["SpriteTexture"].SetValue(
				Content.Load<Texture2D>("fire"));
			spriteArray = new VertexPositionColor[200];
			vertexPosColDecl = new VertexDeclaration(graphics.GraphicsDevice,
				VertexPositionColor.VertexElements); 


			device = graphics.GraphicsDevice;
			basicEffect = new BasicEffect(device, null);			

			font = Content.Load<SpriteFont>("SpriteFont1");
			smallfont = Content.Load<SpriteFont>("SpriteFont2");						

			if (graphics.GraphicsDevice.GraphicsDeviceCapabilities.
				PixelShaderVersion.Major >= 3)
			{
				baseEffect = Content.Load<Effect>("Effect\\MaterialShader30");				
				numLights = 3;
				baseEffect.Parameters["numLights"].SetValue(numLights);
				shaderVersionString = "Using Shader Model 3.0";
			}
			else
			{
				baseEffect = Content.Load<Effect>("Effect\\MaterialShader20");				
				numLights = 3;
				baseEffect.Parameters["numLights"].SetValue(numLights);
				shaderVersionString = "Using Shader Model 2.0";
			}

			// cache the effect parameters
			viewParameter = baseEffect.Parameters["view"];
			projectionParameter = baseEffect.Parameters["projection"];
			cameraPositionParameter = baseEffect.Parameters["cameraPosition"];

			// create the materials
			BasicMaterial = new Material(Content, graphics.GraphicsDevice,baseEffect);
			WallMaterial = new Material(Content, graphics.GraphicsDevice, baseEffect);
			FloorMaterial = new Material(Content, graphics.GraphicsDevice, baseEffect);
			CeilingMaterial = new Material(Content, graphics.GraphicsDevice, baseEffect);
			
			BasicMaterial.SetBasicProperties(Color.Purple, 0.5f, 1.2f);			
			WallMaterial.SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity*2,
				"wall_diffuse",
				"wall_specular", 1f, 1f);
			FloorMaterial.SetTexturedMaterial(Color.White, 0, 0,
				"ground06",
				"ground06", 1f, 1f);
			CeilingMaterial.SetTexturedMaterial(Color.White, 0, 0,
				"ground06",
				"ground06", 1f, 1f);


			//Set ambient light
			baseEffect.Parameters["ambientLightColor"].SetValue(
				new Vector4(Stage.ConfigurationSettings.Ambient*1.4f,
					Stage.ConfigurationSettings.Ambient*1.3f,
					Stage.ConfigurationSettings.Ambient, 1.0f));

			// Recalculate the projection properties on every LoadGraphicsContent call.
			// That way, if the window gets resized, then the perspective matrix will
			// be updated accordingly
			float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
				(float)graphics.GraphicsDevice.Viewport.Height;
			float fieldOfView = aspectRatio * MathHelper.PiOver4 * 3f / 4f;
			projection = Matrix.CreatePerspectiveFieldOfView(
				fieldOfView, aspectRatio, .1f, 1000f);
			projectionParameter.SetValue(projection);

			// calculate the safe left and top edges of the screen
			safeBounds = new Vector2(
				(float)graphics.GraphicsDevice.Viewport.X +
				(float)graphics.GraphicsDevice.Viewport.Width * 0.1f,
				(float)graphics.GraphicsDevice.Viewport.Y +
				(float)graphics.GraphicsDevice.Viewport.Height * 0.1f
				);

			// Init light
			PlayerLight = new PointLight(new Vector4(0, 0, 0, 1.0f)
					, baseEffect.Parameters["lights"].Elements[0]);
			MonsterLight = new PointLight(new Vector4(0, 0, 0, 1.0f)
					, baseEffect.Parameters["lights"].Elements[1]);
			TeleporterLight = new PointLight(new Vector4(0, 0, 0, 1.0f)
					, baseEffect.Parameters["lights"].Elements[2]);


			//color, range, and power of the lights
			PlayerLight.Range = Stage.ConfigurationSettings.TorchRange;
			PlayerLight.Falloff = Stage.ConfigurationSettings.TorchAttenuation;

			MonsterLight.Range = Stage.ConfigurationSettings.TorchRange/8;
			MonsterLight.Falloff = Stage.ConfigurationSettings.TorchAttenuation;

			TeleporterLight.Range = Stage.ConfigurationSettings.TorchRange/2;
			TeleporterLight.Falloff = Stage.ConfigurationSettings.TorchAttenuation;

			// always set light 0 to pure white as a reference point
			PlayerLight.Color = new Color(0.0f,0.0f,0.0f);
			MonsterLight.Color = new Color(0.0f, 1.0f, 0.0f);
			TeleporterLight.Color = new Color(0.0f, 0.0f, 1.0f);

			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			//Set up the UI component
			UI = new UIComponent(this, graphics);
			UI.GameSpriteBatch = spriteBatch;
			this.Components.Add(UI);

			//Set up the UI component
			Texture2D mouseTexture = Content.Load<Texture2D>("mouse");
			Point mouseHotspot = new Point(8, 7);
			Point mouseLocation = new Point(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
			Dictionary<MousePointerState, Rectangle> mouserects = new Dictionary<MousePointerState, Rectangle>();
			mouserects.Add(MousePointerState.Default, new Rectangle(0, 0, 31, 31));
			mouserects.Add(MousePointerState.ActiveArea, new Rectangle(32, 0, 63, 31));
			Rectangle screenRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

			UI.CurrentMousePointer = MousePointer.CreateFromTexture2D(
				mouseTexture,
				mouseHotspot,
				mouseLocation,
				mouserects,
				screenRectangle);

			//Load UI Form
			Forms.TestRootForm root = new GOOS.MonsterMaze.Forms.TestRootForm("TheRootForm");
			root.Content = Content;
			root.RenderRectangle = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
			root.AddControls();
			UI.RootForm = root;						

			//Add event handlers
			UI.RootForm.GetControl("Button_NewGame").Click += new GameControlEventHandler(ClickNew);
			UI.RootForm.GetControl("Button_Quit").Click += new GameControlEventHandler(ClickQuit);
			UI.RootForm.GetControl("Button_Ok").Click += new GameControlEventHandler(ClickOk);
			UI.RootForm.GetControl("Button_HiScore").Click += new GameControlEventHandler(ClickHiScore);
			UI.RootForm.GetControl("Button_Options").Click += new GameControlEventHandler(ClickOptions);
			UI.RootForm.GetControl("Button_Help").Click += new GameControlEventHandler(ClickHelp);

			//Load the hi-scores, or make some up.
			string columnNames = "name|level|time|score";
			string sortOrder = "score|level|time;ASC|name;ASC";
			HiScores = HiScoreTable.LoadFromFile("CreatureCave.hiscores", columnNames, sortOrder);
			if (HiScores.Scores.Count < 1)
			{
				//Scores are empty or score file isn't available. Make up some scores.
				HiScores.AddScore("Robert Prim", 1, 50, 200);
				HiScores.AddScore("Rex", 2, 170, 400);
				HiScores.AddScore("Vojtech Jarnik", 4, 290, 800);
				HiScores.AddScore("William Dyer", 6, 410, 1600);
				HiScores.AddScore("Joseph Kruskal", 8, 530, 3200);
				HiScores.AddScore("Ellen Ripley", 10, 650, 6400);
				HiScores.AddScore("Edsger Dijkstra", 14, 770, 12800);
				HiScores.AddScore("Danny Torrance", 18, 890, 25600);
				HiScores.AddScore("Alan Turing", 22, 1010, 51200);
				HiScores.AddScore("Theseus", 26, 1130, 102400);
			}

            //Set up display mode slider
            //((DisplayMode)Modes[0]).Width

            Slider DisplayModeSlider = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderDisplayMode"));
            DisplayModeSlider.MinValue = 0;
            DisplayModeSlider.MaxValue = ModeList.Count();         

			//Set the options contgrols to the loaded values from config.

			//floats
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSampling")).Value = OptionsTextureSampler;
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderParticles")).Value = OptionsParticleDensity;
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSFX")).Value = OptionsSFXVolume;
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderMusic")).Value = OptionsMusicVolume;
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderViewDist")).Value = OptionsViewDistance;
			((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderBloom")).Value = OptionsBloomLevel;

			//bools
			((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckFullscreen")).Checked = Stage.ConfigurationSettings.Fullscreen;
			((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckMusicMute")).Checked = OptionsMuteMusic;
			((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckSFXMute")).Checked = OptionsMuteSFX;
			((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckBloom")).Checked = OptionsBloomOn;

			//Add scores to table control
			((HiScoreTableControl)root.GetControl("HiScoreTable")).Table = HiScores;

			//Set the initial Game State
			Stage.GameState.Console = false;
			Stage.GameState.DemoMode = true;
			Stage.GameState.Menu = true;
			Stage.GameState.Pause = false;

			//Generate the initial map
			GenerateNewMap();	
	
			//Keyboard input test
			UI.KeyboardActive = true;
			UI.ControlWithInputFocus = root.GetControl("KeyboardTestLabel");

						
		}			

		#endregion

		#region UI Event Handlers

		/// <summary>
		/// Handle the transition to the hi score screen.
		/// </summary>	
		void ClickHiScore(IGameControl sender, GameControlEventArgs args)
		{
			SwitchMenuScreen(UIScreenEnum.Hiscore);			
		}	

		/// <summary>
		/// Handle the transition to the Options screen.
		/// </summary>
		void ClickOptions(IGameControl sender, GameControlEventArgs args)
		{
			SwitchMenuScreen(UIScreenEnum.Options);			
		}	

		/// <summary>
		/// Handle the transition to the Help screen
		/// </summary>
		void ClickHelp(IGameControl sender, GameControlEventArgs args)
		{
			SwitchMenuScreen(UIScreenEnum.Help);		
		}	

		/// <summary>
		/// Handle the OK click button (return to main screen from sub screen.
		/// </summary>
		void ClickOk(IGameControl sender, GameControlEventArgs args)
		{
			//If this is the options screen, save the selected values;
			if (((TestRootForm)UI.RootForm).ScreenState == UIScreenEnum.Options)
			{
				//Populate local

				//floats
				OptionsTextureSampler = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSampling")).Value;
				OptionsParticleDensity = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderParticles")).Value;
				OptionsSFXVolume = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSFX")).Value;
				OptionsMusicVolume = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderMusic")).Value;
				OptionsViewDistance = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderViewDist")).Value;
				OptionsBloomLevel = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderBloom")).Value;

				//bools
				//((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckFullscreen")).Checked = Stage.ConfigurationSettings.Fullscreen; //HMMMMM
				OptionsMuteMusic = ((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckMusicMute")).Checked;
				OptionsMuteSFX = ((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckSFXMute")).Checked;
				OptionsBloomOn = ((GOOS.JFX.UI.Controls.Checkbox)UI.RootForm.GetControl("OptionsCheckBloom")).Checked;

				//Populate config object

				ExtraConfigFile.ExtraFloatSettings["BloomLevel"] = OptionsBloomLevel;
				ExtraConfigFile.ExtraFloatSettings["ParticleDensity"] = OptionsParticleDensity;
				ExtraConfigFile.ExtraFloatSettings["SoundVolume"] = OptionsSFXVolume;
				ExtraConfigFile.ExtraFloatSettings["TextureSampler"] = OptionsTextureSampler;
				ExtraConfigFile.ExtraFloatSettings["ViewDistance"] = OptionsViewDistance;
				ExtraConfigFile.ExtraFloatSettings["MusicVolume"] = OptionsMusicVolume;

				ExtraConfigFile.ExtraBoolSettings["BloomOn"] = OptionsBloomOn;
				ExtraConfigFile.ExtraBoolSettings["MusicMute"] = OptionsMuteMusic;
				ExtraConfigFile.ExtraBoolSettings["SFXMute"] = OptionsMuteSFX;			

				//Save config object

				ExtraConfigFile.SaveTofile("CCExtra.config");

				//Set sounds if sounds playing
				if (Ambient_Heart != null)
					Ambient_Heart.Volume = (OptionsSFXVolume / 100);
				if (Ambient_Monster != null)
					Ambient_Monster.Volume = (OptionsSFXVolume / 100);
				if(Ambient_Wind != null)
					Ambient_Wind.Volume = (OptionsSFXVolume / 100);
				if (MusicPlaying)
					MediaPlayer.Volume = (OptionsMusicVolume / 100);

				//Set particle density
				((SmokePlumeParticleSystem)smokePlumeParticles).Density = (int)OptionsParticleDensity;
				((AntiFireParticleSystem)antifireParticles).Density = (int)OptionsParticleDensity;
				((FireParticleSystem)fireParticles).Density = (int)OptionsParticleDensity;

				//Bloom
				bloom.Settings.BloomIntensity = 3.0f * (OptionsBloomLevel / 100);
				
			}

			if (((TestRootForm)UI.RootForm).ScreenState == UIScreenEnum.EnterName)
			{
				CurrentScoreInfo.ExtendedValues["name"] = ((TextBox)UI.RootForm.GetControl("KeyboardTestLabel")).Text;
				HiScores.AddScore(CurrentScoreInfo);
				HiScores.SaveToFile("CreatureCave.hiscores");
				SwitchMenuScreen(UIScreenEnum.Hiscore);
			}
			else 
				SwitchMenuScreen(UIScreenEnum.MainMenu);	
		}	

		/// <summary>
		/// Handle the New Game button click event
		/// </summary>
		void ClickNew(IGameControl sender, GameControlEventArgs args)
		{
			Stage.GameState.DemoMode = false;
			Stage.GameState.Pause = false;
			Stage.GameState.Menu = false;
			Stage.GameState.Intro = false;
			Stage.GameState.Console = false;
			Score = 0;
			Level = 0;
			GenerateNewMap();
		}

		//The quit button handler
		void ClickQuit(IGameControl sender, GameControlEventArgs args)
		{
			Stage.DumpDebugToFile();
			this.Exit();	
		}		

		/// <summary>
		/// Switch the screen between the main menu and the various sub menus.
		/// </summary>
		private void SwitchMenuScreen(UIScreenEnum e)
		{			
			if (((TestRootForm)UI.RootForm).ScreenState != UIScreenEnum.EnterName)
			{
				IGameControl c = UI.RootForm.GetControl("Button_NewGame");
				c.Visible = !c.Visible;
				UI.RootForm.GetControl("Button_Options").Visible = c.Visible;
				UI.RootForm.GetControl("Button_HiScore").Visible = c.Visible;
				UI.RootForm.GetControl("Button_Help").Visible = c.Visible;
				UI.RootForm.GetControl("Button_Quit").Visible = c.Visible;
				UI.RootForm.GetControl("Button_Ok").Visible = !c.Visible;
			}
			((TestRootForm)UI.RootForm).ScreenState = e;

			UI.RootForm.GetControl("Label_HelpText").Visible = (e == UIScreenEnum.Help);
			UI.RootForm.GetControl("HiScoreTable").Visible = (e == UIScreenEnum.Hiscore);
			UI.RootForm.GetControl("HiScoreLabel").Visible = (e == UIScreenEnum.EnterName);
			UI.RootForm.GetControl("KeyboardTestLabel").Visible = (e == UIScreenEnum.EnterName);

			//Options screen controls
			UI.RootForm.GetControl("OptionsMainLabel").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("OptionsLabels").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("OptionsCheckFullscreen").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("OptionsCheckBloom").Visible = false; //Never let them turn bloom off.
			UI.RootForm.GetControl("OptionsCheckMusicMute").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("OptionsCheckSFXMute").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderViewDist").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderBloom").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderParticles").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderMusic").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderSFX").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderDisplayMode").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("SliderSampling").Visible = (e == UIScreenEnum.Options);
			UI.RootForm.GetControl("sliderlabel").Visible = (e == UIScreenEnum.Options);            
		}

		#endregion

		#region Unload Content Method

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		#endregion

		#region Update Method

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			//What's wrong with my depth buffer???
			//graphics.GraphicsDevice.PresentationParameters.EnableAutoDepthStencil = true;


			#region In any mode			

			UI.Active = Stage.GameState.Menu;

			//Play the background music
			if (MediaPlayer.State != MediaState.Playing)
			{				
			    MediaPlayer.Play(BackgroundMusic);
			    MediaPlayer.IsRepeating = true;
				if (!OptionsMuteMusic)
					MediaPlayer.Volume = (OptionsMusicVolume / 100);
				else
					MediaPlayer.Volume = 0.0f;
			}

			//Set monster warnings.
			Vector3 losvec = Vector3.Zero;
			float nearvec = Vector3.Distance(fpsCam.Position, monster.CurrentLocation);
			float teledist = Vector3.Distance(fpsCam.Position, TeleporterLocation);

			SeeMonster = Stage.Graph.GetLOSByWallBBCollision(fpsCam.Position, monster.CurrentLocation, 240f, ref losvec);
			HearMonster = (nearvec < 240);

			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			if (gamePadState.Buttons.Back == ButtonState.Pressed)
			{
				Stage.DumpDebugToFile();
				this.Exit();
			}

			//Check for the exit key
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyState = Keyboard.GetState();
					
			((GOOS.JFX.UI.Controls.Label)UI.RootForm.GetControl("mouselabel")).Text = UI.CurrentMousePointer.PickedScreenCoordinate.X.ToString() + "," + UI.CurrentMousePointer.PickedScreenCoordinate.Y.ToString();			
			if (((GOOS.JFX.UI.Controls.Label)UI.RootForm.GetControl("sliderlabel")).Visible == true)
			{
				string slidertext = "";

                float dm = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderDisplayMode")).Value;
                slidertext += "(" + ModeList[(int)dm].Width.ToString() + "x" + ModeList[(int)dm].Height.ToString() + ")\r\n\r\n";
				slidertext += "(" + ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderViewDist")).Value.ToString() + ")\r\n";
				slidertext += "(" + ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderBloom")).Value.ToString() + ")\r\n";
				slidertext += "(" + ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderParticles")).Value.ToString() + ")\r\n";

				float v = ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSampling")).Value;
				switch ((int)v)
				{
					case (int)0: slidertext += "(none)"; break;
					case (int)33: slidertext += "(point)"; break;
					case (int)66: slidertext += "(linear)"; break;
					case (int)99: slidertext += "(anisotropic)"; break;
				}				
				slidertext += "\r\n\r\n";				
				slidertext += "(" + ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderMusic")).Value.ToString() + ")\r\n";
				slidertext += "(" + ((GOOS.JFX.UI.Controls.Slider)UI.RootForm.GetControl("SliderSFX")).Value.ToString() + ")";
				((GOOS.JFX.UI.Controls.Label)UI.RootForm.GetControl("sliderlabel")).Text = slidertext;
			}

			if (keyState.IsKeyDown(Keys.F11))
			{
				Stage.DumpDebugToFile();
				this.Exit();
			}
			if (keyState.IsKeyDown(Keys.Escape))
			{
				if ((float)gameTime.TotalGameTime.TotalSeconds - lastpause > 0.5)
				{
					if (!Stage.GameState.DemoMode)
					{
						if (!(Stage.GameState.Menu && fpsCam.InDeathThroes))
						{
							//If the player is first pressing Escape after dying.
							if (fpsCam.InDeathThroes && !Stage.GameState.Menu)
							{
								Event_Scream = null;
								//Compare his score.					
								CurrentScoreInfo = ScoreInfo.CreateNewDefault();
								CurrentScoreInfo.ExtendedValues["name"] = "";
								CurrentScoreInfo.ExtendedValues["score"] = Score;
								CurrentScoreInfo.ExtendedValues["time"] = TimeInGame;
								CurrentScoreInfo.ExtendedValues["level"] = Level;
								CurrentScoreInfo.SortOrder = HiScores.SortOrder;
								List<ScoreInfo> sl = HiScores.GetTop(10);
								if (sl[sl.Count - 1].CompareTo(CurrentScoreInfo) > 0)
								{
									SwitchMenuScreen(UIScreenEnum.EnterName);
									UI.KeyboardActive = true;
									UI.ControlWithInputFocus = UI.RootForm.GetControl("KeyboardTestLabel");
									((TextBox)UI.RootForm.GetControl("KeyboardTestLabel")).Text = "";
								}
								TimeInGame = 0;
							}

							Stage.GameState.Pause = !Stage.GameState.Pause;
							Stage.GameState.Menu = !Stage.GameState.Menu;							
							UI.RootForm.GetControl("BigBanner").Visible = false;							
						}
					}
					lastpause = (float)gameTime.TotalGameTime.TotalSeconds;
				}
			}
			else			
				lastpause = 0;			

			//Set the torch to follow the player.
			Vector3 torchvector = new Vector3(fpsCam.LastTarget.X, fpsCam.LastTarget.Y, fpsCam.LastTarget.Z);
			PlayerLight.Position = new Vector4(torchvector.X, torchvector.Y, torchvector.Z, 1.0f);
			MonsterLight.Position = new Vector4(monster.CurrentLocation.X, monster.CurrentLocation.Y, monster.CurrentLocation.Z, 1.0f);
			TeleporterLight.Position = new Vector4(TeleporterLocation.X, TeleporterLocation.Y, TeleporterLocation.Z, 1.0f);

			flickerdelay += (float)gameTime.ElapsedRealTime.TotalMilliseconds;

			//flame flicker 10.2Hz = 1 flicker every milliseconds
			if (flickerdelay > 98)
			{
				float flicker = 0.73f + (float)random.NextDouble() * 0.07f;
				PlayerLight.Color = new Color(flicker * 1.4f, flicker * 1.2f, flicker - 0.1f);
				flickerdelay = 0;
			}

			#endregion

			#region In play mode
			// Do this if not paused and not in demo mode
			if (!Stage.GameState.DemoMode && !Stage.GameState.Pause)
			{
				//Standard post processing settings.
				bloom.FadeSettings.On = false;
				bloom.ColourSettings.Sepia = false;

				//Update particle systems
				#region Particle System test
				switch (currentState)
				{
					case ParticleState.Explosions:
						UpdateExplosions(gameTime);
						break;

					case ParticleState.SmokePlume:
						UpdateSmokePlume();
						UpdateFire();
						break;

					case ParticleState.RingOfFire:
						UpdateFire();
						break;
				}

				UpdateProjectiles(gameTime);
				#endregion

				//SFX -- always windy
				if (Ambient_Wind == null && !OptionsMuteSFX)
					Ambient_Wind = Wind.Play((0.1f* OptionsSFXVolume/100), 0.0f, 0.0f, true);
				//monster noise louder as monster gets near
				if (Ambient_Monster == null && !OptionsMuteSFX)
					Ambient_Monster = NearMonster.Play((0.0f* OptionsSFXVolume/100), 0.0f, 0.0f, true);
				else
				{
					if (HearMonster)
						Ambient_Monster.Volume = ((1.0f / 240.0f) * (240 - nearvec) * OptionsSFXVolume / 100);
					else
						Ambient_Monster.Volume = 0.0f;
				}
				//heart noise louder as monster gets near
				if (Ambient_Heart == null)
					Ambient_Heart = Heart.Play(0.0f, 0.0f, 0.0f, true);
				else
				{
					if (SeeMonster)
						Ambient_Heart.Volume = ((1.0f / 240.0f) * (240 - nearvec) *OptionsSFXVolume / 100);
					else
						Ambient_Heart.Volume = 0.0f;
				}
				/////////////////////////////////////////////// SIN/COS Wibble effect

				//Wibbling gets more pronounced the closer the monster is.
				if (HearMonster && nearvec < 48)
				{
					bloom.SineSetting.LFO = (240 - nearvec) * -0.1f;
					bloom.SineSetting.HFO = 24;
				}
				else
				{
					// no wibble (monster too far)
					bloom.SineSetting.LFO = 0;
					bloom.SineSetting.HFO = 0;
				}
				//////////////////////////////////////////////////////////////////////////////////////////// Player Is Dead						
				if ((monster.Actions & FLAG_Actions.FIGHTING) == FLAG_Actions.FIGHTING || fpsCam.InDeathThroes)
				{
					bloom.FadeSettings.On = true;
					bloom.FadeSettings.Red = 1.0f;
					bloom.FadeSettings.Green = 0.0f;
					bloom.FadeSettings.Blue = 0.0f;

					Ambient_Heart.Volume = 0.0f;
					Ambient_Monster.Volume = 0.0f;
					if (Event_Scream == null && !OptionsMuteSFX)
						Event_Scream = Scream.Play(OptionsSFXVolume/100);
					if (!fpsCam.InDeathThroes)
					{
						//This is where we kill the player.
						fpsCam.Die(4.0f);
					}
					else
					{
						//post fade to/from red 
						if (bloom.FadeSettings.Level < 1.0f)
							bloom.FadeSettings.Level += 0.01f;
						else
						{
							//Reverse direction of fade
							bloom.FadeSettings.FadeTo = !bloom.FadeSettings.FadeTo;
							//Reset Fade
							bloom.FadeSettings.Level = 0;
						}
					}
				}
				else
				{
				}

				//jumping - don't update jump when in pause or demo mode
				if (falling)
					if (currentjump > 0)
						currentjump -= 0.25f;
					else
						falling = false;

				if (jumping)
					if (currentjump < 6.0)
						currentjump += 0.25f;
					else
					{
						jumping = false;
						falling = true;
					}

				//Set the picking ray - only need do this when not in demo or pause mode
				fpsCam.PickingRay = CalculateCursorRay(fpsCam.ProjectionMatrix,
														fpsCam.ViewMatrix, new Vector2(
														graphics.PreferredBackBufferWidth / 2,
														graphics.PreferredBackBufferHeight / 2));

				//Check for the jump key
				if (keyState.IsKeyDown(Keys.Space))
				{
					if (!jumping && !falling)
					{
						jumping = true;
					}
				}

				//Update camera.
				fpsCam.Update(mouseState, keyState, gamePadState);				
				fpsCam.currentjump = this.currentjump;

				//Victory?
				if (Vector3.Distance(fpsCam.Position, TeleporterLocation) < 8)
				{
					GenerateNewMap();
				}				

				//Don't update the breadcrumbs in demo or pause mode.
				//minimap breadcrumbs
				Point p = new Point(fpsCam.LastGridSquareX, fpsCam.LastGridSquareY);
				if (fpsCam.LastGridSquareX != LastVisitedSquare.X || fpsCam.LastGridSquareY != LastVisitedSquare.Y)
				{
					if (!VisitedGridSquares.Contains(p))
						VisitedGridSquares.Add(p);
				}
				LastVisitedSquare = p;

				// Don't update the timer in demo or pause mode
				if ((monster.Actions & FLAG_Actions.FIGHTING) != FLAG_Actions.FIGHTING)
				{
					//Timer
					int totalsecondsint = (int)gameTime.TotalRealTime.TotalSeconds;
					if (TimeOnLevel == -1)
					{
						TimeAtLevelStart = totalsecondsint;
						TimeOnLevel = 0;
					}
					if (totalsecondsint > TimeAtLevelStart)
					{
						TimeOnLevel = totalsecondsint - TimeAtLevelStart;
					}
				}
			}
			#endregion

			#region In Pause or Demo mode
			else // When paused or in demo mode
			{
				//If the wind is playing - stop it
				if (Ambient_Wind != null)
					Ambient_Wind.Stop(true);
				//No wind
				Ambient_Wind = null;

				//If the monster noise is playing stop it 
				if (Ambient_Monster != null)
					Ambient_Monster.Stop(true);
				//No monster
				Ambient_Monster = null;

				//If the heartbeat noise is playing stop id
				if (Ambient_Heart != null)
					Ambient_Heart.Stop(true);
				//No heartbeat
				Ambient_Heart = null;

				//No wibble in pause or demo mode
				bloom.SineSetting.LFO = 0;
				bloom.SineSetting.HFO = 0;

				//In demo mode there can be no jump
				if (Stage.GameState.DemoMode)
					fpsCam.currentjump = 0;
				
				bloom.ColourSettings.Sepia = true;
				bloom.FadeSettings.FadeTo = true;
				bloom.FadeSettings.On = true;
				bloom.FadeSettings.Red = 0.0f;
				bloom.FadeSettings.Green = 0.0f;
				bloom.FadeSettings.Blue = 0.0f;
				bloom.FadeSettings.Level = 0.5f;

				base.Update(gameTime);
				bloom.Update(gameTime);
			}
			#endregion		
			
			//Update the Stage - this updates AI driven monsters including the demo zombie.
			if (!Stage.GameState.Pause)
			{
				Stage.Update(true, true, true, fpsCam.Position);
				base.Update(gameTime);
				smokePlumeParticles.Update(gameTime);
				fireParticles.Update(gameTime);
				antifireParticles.Update(gameTime);
				bloom.Update(gameTime);
			}
			//If in demo mode, the player follows the demo zombie.
			if (Stage.GameState.DemoMode && demoZombie != null)
			{
				fpsCam.Position = demoZombie.CurrentLocation;
				fpsCam.SetFacingDegrees(demoZombie.GetFacingDegrees()-90);
			}

			//Keyboard input test
			if (UI.ControlWithInputFocus != null)
			{
				TextBox l = (TextBox)UI.ControlWithInputFocus;
				string labeltext = l.Text;
				while (UI.KeyQueue.Count > 0)
				{
					Keys k = UI.KeyQueue.Dequeue();
					string keytostring = KeyPressInterpreter.KeyToString(k, keyState);

					if (keytostring.Length > 0)
						labeltext += keytostring;
					else
					{
						if (k == Keys.Back && labeltext.Length > 0)
							labeltext = labeltext.Substring(0, labeltext.Length - 1);
					}
				}
				l.Text = labeltext;
			}			
		}

		#endregion

		#region Draw Method

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{								

			//Jumping
			if (!fpsCam.InDeathThroes)
				fpsCam.Setjump(currentjump + 10.0f);

			graphics.GraphicsDevice.Clear(new Color(new Vector3(0.002f, 0.002f, 0.002f)));			

			// enable the depth buffer since geometry will be drawn
			graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
			graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;						
			graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;			

			// always set the shared effects parameters
			viewParameter.SetValue(fpsCam.ViewMatrix);
			cameraPositionParameter.SetValue(fpsCam.Position);

			//Further optimisation needed - each quad should know which square it's in and the player's field of view should be taken into account

			// MAIN DRAW
			//Particles			
			smokePlumeParticles.SetCamera(fpsCam.ViewMatrix, fpsCam.ProjectionMatrix);
			fireParticles.SetCamera(fpsCam.ViewMatrix, fpsCam.ProjectionMatrix);
			antifireParticles.SetCamera(fpsCam.ViewMatrix, fpsCam.ProjectionMatrix);

			BoundingSphere sphere = new BoundingSphere();
			BoundingFrustum frustum = new BoundingFrustum(fpsCam.ViewMatrix * fpsCam.ProjectionMatrix);
			Matrix ww;

			//Only render the monster in play mode.
			if (!Stage.GameState.DemoMode)			
				//Render The Stage				
				Stage.Render(true, false, false, fpsCam, 160.0f);

			if (!Stage.GameState.DemoMode)
			{
				//Test render - a purple cube at the teleporter location.
				Matrix mw = Matrix.Identity * Matrix.CreateTranslation(TeleporterRenderLocation);
				BasicMaterial.DrawComplexModelWithMaterial(testmodel, ref mw, string.Empty);
			}
			else
			{
				Matrix mw = Matrix.Identity * Matrix.CreateTranslation(0,0,100);
				BasicMaterial.DrawComplexModelWithMaterial(testmodel, ref mw, string.Empty);
			}

			//Render walls
			foreach (Quad q in Stage.Map.Floors)
			{
				if (q.Origin.X * 2 < fpsCam.Position.X + (320 * OptionsViewDistance / 100) && q.Origin.X * 2 > fpsCam.Position.X - (320 * OptionsViewDistance / 100)
					&& q.Origin.Z * 2 < fpsCam.Position.Z + (320 * OptionsViewDistance / 100) && q.Origin.Z * 2 > fpsCam.Position.Z - (320 * OptionsViewDistance / 100))
				{
					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);

					if (frustum.Intersects(sphere))
					{
						ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
						FloorMaterial.DrawQuadWithMaterial(q, ref ww);						
					}
				}
			}
			foreach (Quad q in Stage.Map.Ceilings)
			{
				if (q.Origin.X * 2 < fpsCam.Position.X + (320 * OptionsViewDistance / 100) && q.Origin.X * 2 > fpsCam.Position.X - (320 * OptionsViewDistance / 100)
					&& q.Origin.Z * 2 < fpsCam.Position.Z + (320 * OptionsViewDistance / 100) && q.Origin.Z * 2 > fpsCam.Position.Z - (320 * OptionsViewDistance / 100))
				{
					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);

					if (frustum.Intersects(sphere))
					{
						ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
						CeilingMaterial.DrawQuadWithMaterial(q, ref ww);						
					}
				}
			}
			foreach (Quad q in Stage.Map.Walls)
			{
				if (q.Origin.X * 2 < fpsCam.Position.X + (320 * OptionsViewDistance / 100) && q.Origin.X * 2 > fpsCam.Position.X - (320 * OptionsViewDistance / 100)
					&& q.Origin.Z * 2 < fpsCam.Position.Z + (320 * OptionsViewDistance / 100) && q.Origin.Z * 2 > fpsCam.Position.Z - (320 * OptionsViewDistance / 100))
				{

					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);

					if (frustum.Intersects(sphere))
					{
						ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
						WallMaterial.DrawQuadWithMaterial(q, ref ww);
					}
				}
			}
			//draw smoke only when teleporter is near.				
			if (Vector3.Distance(fpsCam.Position, TeleporterLocation) < (320 * OptionsViewDistance / 100))			
				smokePlumeParticles.Draw(gameTime);
				//fireParticles.Draw(gameTime);
			
			//draw fire only when monster is near.
			if (Vector3.Distance(fpsCam.Position, monster.CurrentLocation) < (320 * OptionsViewDistance / 100))
			{
				antifireParticles.Draw(gameTime);
				fireParticles.Draw(gameTime);
			}
				//smokePlumeParticles.Draw(gameTime);


			//only the walls that are close to me are obscuring the particles.

			bloom.Draw(gameTime);			
			base.Draw(gameTime); // DO this before the sprites so the sprites aren't post processed.			
			//Now post process							
			

					

			spriteBatch.Begin(SpriteBlendMode.AlphaBlend);


			if (Stage.GameState.DemoMode)
			{
				//spriteBatch.DrawString(smallfont, "DEBUG Zombie:  " + demoZombie.CurrentLocation.ToString(), new Vector2(10, 40), Color.WhiteSmoke);				
			}

			//compass - don't draw in demo or pause (no pausing to plan strategy!)
			if (!Stage.GameState.DemoMode && !Stage.GameState.Pause)
			{
				spriteBatch.DrawString(smallfont, "LEVEL: " + Level.ToString(), new Vector2(10, 40), Color.WhiteSmoke);
				spriteBatch.DrawString(smallfont, "SCORE: " + Score.ToString(), new Vector2(10, 60), Color.WhiteSmoke);
				spriteBatch.DrawString(smallfont, "TIME : " + TimeOnLevel.ToString(), new Vector2(10, 80), Color.WhiteSmoke);				

				spriteBatch.Draw(CompassTexture,
					new Vector2(100, Stage.ConfigurationSettings.Height - 100),
					null,
					Color.White,
					fpsCam.leftrightRot,
					new Vector2(100, 100),
					Vector2.One,
					SpriteEffects.None,
					0.0f);

				spriteBatch.Draw(CompassPointer,
					new Vector2(100, Stage.ConfigurationSettings.Height - 100),
					null,
					Color.White,
					0,
					new Vector2(5, 112),
					Vector2.One,
					SpriteEffects.None,
					0.1f);

				//For some reason this comes out with the opposite sign from which it should.
				float angletowardsmonster = Collision.GetAngleBetween_XZ(fpsCam.Position, monster.CurrentLocation) - MathHelper.ToRadians(90) - fpsCam.leftrightRot;
				//Fix it.
				if (angletowardsmonster > 0)
					angletowardsmonster -= angletowardsmonster * 2;
				else if (angletowardsmonster < 0)
					angletowardsmonster = Math.Abs(angletowardsmonster);

				spriteBatch.Draw(CompassSkull,
					new Vector2(100, Stage.ConfigurationSettings.Height - 100),
					null,
					Color.White,
					angletowardsmonster,
					new Vector2(5, 100),
					Vector2.One,
					SpriteEffects.None,
					0.1f);

				//For some reason this comes out with the opposite sign from which it should.
				float angletowardsteleporter = Collision.GetAngleBetween_XZ(fpsCam.Position, TeleporterLocation) - MathHelper.ToRadians(90) - fpsCam.leftrightRot;
				//Fix it.
				if (angletowardsteleporter > 0)
					angletowardsteleporter -= angletowardsteleporter * 2;
				else if (angletowardsteleporter < 0)
					angletowardsteleporter = Math.Abs(angletowardsteleporter);

				spriteBatch.Draw(CompassArrow,
					new Vector2(100, Stage.ConfigurationSettings.Height - 100),
					null,
					Color.White,
					angletowardsteleporter,
					new Vector2(5, 100),
					Vector2.One,
					SpriteEffects.None,
					0.1f);

				//Don't draw the "clue" messages in pause or demo modes.
				//if (!((monster.Actions & FLAG_Actions.FIGHTING) == FLAG_Actions.FIGHTING) && !fpsCam.InDeathThroes)
				//{
				//    if (SeeMonster)
				//        spriteBatch.DrawString(smallfont, "It's here, RUN!", new Vector2(10, 140), Color.Crimson);
				//    else if (HearMonster)
				//        spriteBatch.DrawString(smallfont, "You can hear something moving...", new Vector2(10, 140), Color.Crimson);
				//}
				//else
				//{
				//    spriteBatch.DrawString(font, "AARRGHGHGHH!", new Vector2(10, 120), Color.Crimson);
				//}

				//Minimap		
				int dimention = 8;
				int mapwidth = dimention * Stage.Map.Width;
				int mapheight = dimention * Stage.Map.Height;
				float offset_x = Stage.ConfigurationSettings.width - mapwidth - dimention;
				float offset_y = dimention;

				for (int y = 0; y < Stage.Map.Height; y++)
					for (int x = 0; x < Stage.Map.Width; x++)
					{
						if (Stage.Map.GetSquareAt(x, y).type == MapSquareType.Open)
							if (VisitedGridSquares.Contains(new Point(x, y)))
							{
								spriteBatch.Draw(MiniMap_Open, new Vector2(offset_x + x * dimention, offset_y + y * dimention), new Rectangle(8, 8, 8, 8), Color.White);
								if (fpsCam.LastGridSquareX == x && fpsCam.LastGridSquareY == y)
									spriteBatch.Draw(MiniMap_Player, new Vector2(offset_x + x * dimention, offset_y + y * dimention), new Rectangle(0, 0, dimention, dimention), Color.White);
							}
					}
			}

			//The banner is not drawn in demo mode since the UI component displays the big banner. But it is drawn in pause mode.
			if(!Stage.GameState.DemoMode)
				spriteBatch.Draw(Banner,
					new Vector2(0, 0),
					null,
					Color.White,
					0.0f,
					new Vector2(0, 0),
					Vector2.One,
					SpriteEffects.None,
					0.1f);						

			spriteBatch.End();		

						
		}

#endregion

		#region Methods

		/// <summary>
		/// Get a new random map.
		/// </summary>
		private void GenerateNewMap()
		{			

			//If in demo mode, score is reset to 0 (this will either be when the game is loaded or when the player 
			//quits to the main menu

			if (Stage.GameState.DemoMode)			
				Score = 0;			
			else
			{
				//Update score
				Score += Level * ScorePerLevel;
				if (TimeOnLevel < 200)
					Score += Level * (200 - TimeOnLevel);				
			}

			int dimention = 12;
			int wallstobreak = 24;

			//clear breadcrumbs
			LastVisitedSquare = Point.Zero;
			VisitedGridSquares = new List<Point>();

			//Randomise Stage Manager's map.
			Stage.GenerateNewRandomSquareMap(dimention, wallstobreak);			

			// NEW COLLISION/LOS MODEL DATA
			Stage.Map.CalculateWallCollision();

			// Get collision grid data and load into camera class.
			bool[] bg = new bool[Stage.Map.Width * Stage.Map.Height];

			for (int y = 0; y < Stage.Map.Height; y++)
			{
				for (int x = 0; x < Stage.Map.Width; x++)
				{
					if (Stage.Map.GetSquareAt(x, y).type == MapSquareType.Closed)
						bg[x + (y * Stage.Map.Width)] = true;
					else
						bg[x + (y * Stage.Map.Width)] = false;
				}
			}			

			//Get starting location.
			List<Vector2> exclusion = new List<Vector2>();
			int playerquater = Helpers.RandomInt(1, 4);
			Vector2 StartingLocation = Stage.Map.GetRandomSquareCoordsByType(MapSquareType.Open, null, exclusion, playerquater);			

			//Get Monster Starting Location.
			int monsterquater = playerquater;
			while (monsterquater == playerquater)
				monsterquater = Helpers.RandomInt(1, 4);
			exclusion.Add(StartingLocation);
			Vector2 MonsterStartingLocation = Stage.Map.GetRandomSquareCoordsByType(MapSquareType.Open, null, exclusion, monsterquater);

			//Get teleporter starting location.
			int teleporterquater = playerquater;
			while (teleporterquater == playerquater || teleporterquater == monsterquater)
				teleporterquater = Helpers.RandomInt(1, 4);
			//exclusion.Add(MonsterStartingLocation); // Makes it too easy
			Vector2 TeleporterStartingLocation = Stage.Map.GetRandomSquareCoordsByType(MapSquareType.Open, null, exclusion, teleporterquater);			

			//Init camera/Player
			fpsCam = new QuakeCamera(GraphicsDevice.Viewport, new Vector3(StartingLocation.X*16 +4,10.0f,StartingLocation.Y*16+4), 0, 0);
			fpsCam.SetFacingDegrees(0);

			//Init Teleporter
			TeleporterGridLocation = TeleporterStartingLocation;
			TeleporterLocation = new Vector3(TeleporterStartingLocation.X * 16, 10.0f, TeleporterStartingLocation.Y * 16);
			TeleporterRenderLocation = new Vector3(TeleporterStartingLocation.X * 16, -10.0f, TeleporterStartingLocation.Y * 16);

			fpsCam.CollisionGrid = bg;
			fpsCam.GridHeight = Stage.Map.Height;
			fpsCam.GridWidth = Stage.Map.Width;
			fpsCam.SquareWidth = 8;
			fpsCam.SquareHeight = 8;	

			//Init Monster			
			Stage.ClearEntities(true, true);
			monster = new Actor();
		
			monster.CollisionGrid = bg;
			monster.GridWidth = Stage.Map.Width;
			monster.GridHeight = Stage.Map.Height;
			monster.SetFacingDegrees(-90);
			monster.SetLocation(MonsterStartingLocation.X*16+4, 10, MonsterStartingLocation.Y*16 +4);
			monster.FacingOffset = MathHelper.ToRadians(270);
			monster.FOV = 355.0f;
			monster.UpdateWorldMatrix();
			monster.Behaviours = FLAG_Behaviours.AGGRESSIVE | FLAG_Behaviours.MONSTER | FLAG_Behaviours.EVIL_GENIUS;			
			monster.ScalingFactor = 1.0f;
			monster.FightRange = 2.0f;
			monster.HearingRadius = 640.0f;
			monster.SightRadius = 640.0f;
			monster.InstanceName = "Rex";
			monster.movespeed = 0.28f + MonsterSpeedupFactor*Level;
			//Set model
			monster.DefaultModel = monsterModel;
			//Set material
			Material monsterMaterial = new Material(Content, GraphicsDevice, baseEffect);
			monsterMaterial.SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity, "eyestex", "eyestex", 20f, 20f);
			monsterMaterial.Scaled = true;
			monster.DefaultMaterials.Add("default", monsterMaterial);
			//Load the monster into the Stage manager and bring him onstage.
			Stage.LoadActor(monster);
			Stage.BringActorOnStage("Rex");

			//Init Demo Zombie -- In demo mode the demo zombie patrolls a list of random waypoints and the player follows it.
			//The demo zombie is invisible.
			if (Stage.GameState.DemoMode)
			{
				demoZombie = new Actor();

				demoZombie.CollisionGrid = bg;
				demoZombie.GridWidth = Stage.Map.Width;
				demoZombie.GridHeight = Stage.Map.Height;
				demoZombie.SetFacingDegrees(-90);
				demoZombie.SetLocation(StartingLocation.X * 16 + 4, 10, StartingLocation.Y * 16 + 4);
				demoZombie.FacingOffset = MathHelper.ToRadians(270);
				demoZombie.FOV = 355.0f;
				demoZombie.UpdateWorldMatrix();
				demoZombie.Behaviours = FLAG_Behaviours.PATROLLER | FLAG_Behaviours.MONSTER;
				demoZombie.ScalingFactor = 0.0f;
				demoZombie.FightRange = 0.0f;
				demoZombie.HearingRadius = 0.0f;
				demoZombie.SightRadius = 0.0f;
				demoZombie.InstanceName = "Zombie";
				demoZombie.movespeed = 0.56f;

				//Set up the zombie's waypoints.
				//Set up patrol list.
				Vector2 lastLocation = Stage.Map.GetRandomSquareCoordsByType(MapSquareType.Open, null, null, 0);
				Vector2 nextLocation = lastLocation;

				Stage.Graph.SearchAStarPath(StartingLocation, nextLocation);
				demoZombie.SimplePatrolList = Stage.Graph.GetPathList();
				//The last node of this list will always be the same as the first node of the next list. This affects the direction in which the actor[0] looks
				//so.
				for (int zombiepaths = 0; zombiepaths < 10; zombiepaths++)
				{
					demoZombie.SimplePatrolList.RemoveAt(demoZombie.SimplePatrolList.Count - 1);
					nextLocation = Stage.Map.GetRandomSquareCoordsByType(MapSquareType.Open, null, null, 0);
					Stage.Graph.SearchAStarPath(lastLocation, nextLocation);
					demoZombie.SimplePatrolList.AddRange(Stage.Graph.GetPathList());
					lastLocation = nextLocation;
				}

				demoZombie.SimplePatrolList.RemoveAt(demoZombie.SimplePatrolList.Count - 1);
				nextLocation = StartingLocation;
				Stage.Graph.SearchAStarPath(lastLocation, nextLocation);
				demoZombie.SimplePatrolList.AddRange(Stage.Graph.GetPathList());				

				//Set model
				demoZombie.DefaultModel = monsterModel;
				//Set material
				Material zombieMaterial = new Material(Content, GraphicsDevice, baseEffect);
				zombieMaterial.SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity, "banner", "banner", 1f, 1f);
				zombieMaterial.Scaled = true;
				demoZombie.DefaultMaterials.Add("default", monsterMaterial);
				//Load the monster into the Stage manager and bring him onstage.
				Stage.LoadActor(demoZombie);
				Stage.BringActorOnStage("Zombie");

			}

			//Update level
			Level++;
			TimeInGame += TimeOnLevel;
			TimeOnLevel = -1;

			if (Stage.GameState.DemoMode)
				Level = 0;
		}

		// CalculateCursorRay Calculates a world space ray starting at the camera's
		// "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
		// to accomplish this. see the accompanying documentation for more explanation
		// of the math behind this function.
		public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix, Vector2 Position)
		{
			// create 2 positions in screenspace using the cursor position. 0 is as
			// close as possible to the camera, 1 is as far away as possible.
			Vector3 nearSource = new Vector3(Position, 0f);
			Vector3 farSource = new Vector3(Position, 1f);

			// use Viewport.Unproject to tell what those two screen space positions
			// would be in world space. we'll need the projection matrix and view
			// matrix, which we have saved as member variables. We also need a world
			// matrix, which can just be identity.
			Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
				projectionMatrix, viewMatrix, Matrix.Identity);

			Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
				projectionMatrix, viewMatrix, Matrix.Identity);

			// find the direction vector that goes from the nearPoint to the farPoint
			// and normalize it....
			Vector3 direction = farPoint - nearPoint;
			direction.Normalize();

			// and then create a new ray using nearPoint as the source.
			return new Ray(nearPoint, direction);
		}

		#endregion

		#region Particle System Test Helper Methods

		/// <summary>
		/// Helper for updating the explosions effect.
		/// </summary>
		void UpdateExplosions(GameTime gameTime)
		{
			timeToNextProjectile -= gameTime.ElapsedGameTime;

			//if (timeToNextProjectile <= TimeSpan.Zero)
			//{
			//    // Create a new projectile once per second. The real work of moving
			//    // and creating particles is handled inside the Projectile class.
			//    projectiles.Add(new Projectile(explosionParticles,
			//                                   explosionSmokeParticles,
			//                                   projectileTrailParticles));

			//    timeToNextProjectile += TimeSpan.FromSeconds(1);
			//}
		}


		/// <summary>
		/// Helper for updating the list of active projectiles.
		/// </summary>
		void UpdateProjectiles(GameTime gameTime)
		{
			int i = 0;

			while (i < projectiles.Count)
			{
				if (!projectiles[i].Update(gameTime))
				{
					// Remove projectiles at the end of their life.
					projectiles.RemoveAt(i);
				}
				else
				{
					// Advance to the next projectile.
					i++;
				}
			}
		}


		/// <summary>
		/// Helper for updating the smoke plume effect.
		/// </summary>
		void UpdateSmokePlume()
		{
			// This is trivial: we just create one new smoke particle per frame.
			smokePlumeParticles.AddParticle(TeleporterRenderLocation, Vector3.Zero);
			//smokePlumeParticles.AddParticle(new Vector3(monster.CurrentLocation.X, monster.CurrentLocation.Y - 10, monster.CurrentLocation.Z), Vector3.Zero);
		}


		/// <summary>
		/// Helper for updating the fire effect.
		/// </summary>
		void UpdateFire()
		{
			const int fireParticlesPerFrame = 20;

			// Create a number of fire particles, randomly positioned around a circle.
			for (int i = 0; i < fireParticlesPerFrame; i++)
			{
				//fireParticles.AddParticle(TeleporterLocation, Vector3.Zero);
				fireParticles.AddParticle(new Vector3(monster.CurrentLocation.X, monster.CurrentLocation.Y - 7, monster.CurrentLocation.Z), Vector3.Zero);
				antifireParticles.AddParticle(new Vector3(monster.CurrentLocation.X, monster.CurrentLocation.Y + 7, monster.CurrentLocation.Z), Vector3.Zero);

			}

			// Create one smoke particle per frmae, too.
			//smokePlumeParticles.AddParticle(RandomPointOnCircle(), Vector3.Zero);
		}


		/// <summary>
		/// Helper used by the UpdateFire method. Chooses a random location
		/// around a circle, at which a fire particle will be created.
		/// </summary>
		Vector3 RandomPointOnCircle()
		{
			const float radius = 30;
			const float height = 40;

			double angle = random.NextDouble() * Math.PI * 2;

			float x = (float)Math.Cos(angle);
			float y = (float)Math.Sin(angle);

			return new Vector3(x * radius, y * radius + height, 0);
		}

		#endregion
	}
}
