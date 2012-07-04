#region About
/*
 *      This is a demo of the basic concepts in Grimwood Group - Bane.
 * 
 */
#endregion

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
using GOOS.JFX.Level;
using GOOS.JFX.Scripting;
using GOOS.JFX.Entity;
using GOOS.JFX.Game;

namespace GOOS.Dungeon
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
		//STAGE MANAGER - TODO: move all stage management tasks from the game instance to a stage manager.
		// 
		// move config
		// move map/graph
		// move actors

		public float flickerdelay;

		public float NearestWall = 200.0f;

		public Model testmodel;
		public Model tigermodel;
		public Model tinymodel;

		public StageManager Stage;

		//SCRIPTING - TODO: move all scripting and scriptable objects to the stage manager.
		
        #region Sample Fields
        private GraphicsDeviceManager graphics;
        //private SampleArcBallCamera camera;
        private Vector2 safeBounds;       
        private SpriteBatch spriteBatch;
        private Effect pointLightMeshEffect;
        private string shaderVersionString = string.Empty;
        private Matrix projection;
        private Random random = new Random();
        #endregion

        #region Scene Fields
        private Matrix meshRotation;
        private Matrix[] meshWorlds;
        private Material[] materials;
        private Material floorMaterial;
        private Matrix floorWorld;   
        private PointLight[] lights;
        private Model[] sampleMeshes;  
        private int numLights;   
        private Matrix lightMeshWorld;
        #endregion

        #region Shared Effect Fields
        private Effect baseEffect;
        private EffectParameter viewParameter;
        private EffectParameter projectionParameter;
        private EffectParameter cameraPositionParameter;
        #endregion

		Rectangle textBox;

		public List<string> PickedActors;

        private int numquads;             

        public bool LOS;
		public bool NEWLOS;
		public Vector3 NEWLOSCOLL = Vector3.Zero;
       
        private List<Quad> CurrentMapQuads;

        private Model barrel;

        //private Actor actor[0],actor[1],actor[2];
        private Actor[] actor;

        //private 

        private float fps;

        private Song BackgroundMusic;
        private bool MusicPlaying;

        private Quad testquad;
        private Quad testquad2;
        private Quad testquad3;
        private Quad testquad4;
        private Quad testquad5;

        private Quad testquadb;
        private Quad testquad2b;
        private Quad testquad3b;      

        private Texture2D logo;
        private Texture2D crosshair;

        private float currentjump;
        private bool jumping;
        private bool falling;

        private SpriteFont font;
        private Vector2 SpriteVector;

        GraphicsDevice device;
        BasicEffect basicEffect;
        QuakeCamera fpsCam;
        CoordCross cCross;

        Model myModel;      
        Matrix[] modelTransforms;

		/// <summary>
		/// Default Constructor
		/// </summary>
        public Game1()
        {
			//General XNA setup
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

			//Load configuration
			Stage = StageManager.CreateFromConfigFile("bane.config");			       			            	
			graphics.PreferredBackBufferWidth = Stage.ConfigurationSettings.width;
			graphics.PreferredBackBufferHeight = Stage.ConfigurationSettings.Height;
			graphics.IsFullScreen = Stage.ConfigurationSettings.Fullscreen;        
        }

        protected override void Initialize()
        {
			PickedActors = new List<string>();

			textBox = new Rectangle(10, 10, 300, 300);  

            this.Window.Title = "Bane XNA demo";
            // create a default world and matrix
            meshRotation = Matrix.Identity;

            currentjump = 0;
            jumping = false;
            falling = false;

            MusicPlaying = false;

            // create the mesh array
            sampleMeshes = new Model[5]; 

            //set up a ring of meshes
            meshWorlds = new Matrix[8];
            for (int i = 0; i < 8; i++)
            {
                float theta = MathHelper.TwoPi * ((float)i / 8f);
                meshWorlds[i] = Matrix.CreateTranslation(
                    5f * (float)Math.Sin(theta), 0, 5f * (float)Math.Cos(theta));
            }

            // set the initial material assignments to the geometry
    

            // stretch the cube out to represent a "floor"
            // that will help visualize the light radii
            floorWorld = Matrix.CreateScale(30f, 1f, 30f) *
                Matrix.CreateTranslation(0, -2.2f, 0);
            lightMeshWorld = Matrix.CreateScale(.2f);

            meshWorlds[1] = Matrix.Identity;
            meshWorlds[1] = meshWorlds[1] * Matrix.CreateScale(0.1f,0.2f,0.1f);
            meshWorlds[1] = meshWorlds[1] * Matrix.CreateTranslation(0, 5f, 0);

            float startx, startz;

            startx = Stage.Level.StartLocation.X * 16;
            startz = Stage.Level.StartLocation.Y * 16;           

            fpsCam = new QuakeCamera(GraphicsDevice.Viewport, new Vector3(startx+4, 10.0f, startz+4), 0, 0);
            fpsCam.SetFacingDegrees(Stage.Level.StartOrientation);

            fpsCam.LastGridSquareX = (int)Stage.Level.StartLocation.X;
            fpsCam.LastGridSquareY = (int)Stage.Level.StartLocation.Y;

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

            fpsCam.CollisionGrid = bg;
            fpsCam.GridHeight = Stage.Map.Height;
            fpsCam.GridWidth = Stage.Map.Width;
            fpsCam.SquareWidth = 8;
            fpsCam.SquareHeight = 8;

            //Set up the test actor[0](s)

            actor = new Actor[10];

            //actor[0] 1 - obeys a move to order then wanders aimlessly
            actor[0] = new Actor();           
            actor[0].CollisionGrid = bg;
            actor[0].GridWidth = Stage.Map.Width;
            actor[0].GridHeight = Stage.Map.Height;
            actor[0].SetFacingDegrees(-90);
            actor[0].SetLocation(160, 10, 72);
            actor[0].FacingOffset = MathHelper.ToRadians(270);
            actor[0].FOV = 60.0f;
            actor[0].UpdateWorldMatrix();
            actor[0].Behaviours = FLAG_Behaviours.WANDERER | FLAG_Behaviours.MONSTER;
			actor[0].ScalingFactor = 0.02f;

            //Set up the order            
            Order o = new Order();
            o.Type = ENUM_OrderType.MOVE_TO;
            Stage.Graph.SearchAStarPath(Stage.Graph.GetGridReferenceFromVector(actor[0].CurrentLocation), new Vector2(11, 37));
            o.Vector2Queue = Stage.Graph.GetPathQueue();

            actor[0].OrderQueue.Enqueue(o);

            //actor[0] 2 - stands around and attacks the player when alerted.
            actor[1] = new Actor();
            actor[1].CollisionGrid = bg;
            actor[1].GridWidth = Stage.Map.Width;
            actor[1].GridHeight = Stage.Map.Height;
            actor[1].SetFacingDegrees(-90);
            actor[1].SetLocation(228, 4, 132);
            actor[1].FacingOffset = MathHelper.ToRadians(270);
            actor[1].FOV = 60.0f;
            actor[1].UpdateWorldMatrix();
            actor[1].Behaviours = FLAG_Behaviours.AGGRESSIVE | FLAG_Behaviours.MONSTER;
			actor[1].ScalingFactor = 4.0f;
			actor[1].FightRange = 6.0f;
			actor[1].HearingRadius = 320.0f;

            //actor[0] 3 - patrols waypoints
            actor[2] = new Actor();
            actor[2].CollisionGrid = bg;
            actor[2].GridWidth = Stage.Map.Width;
            actor[2].GridHeight = Stage.Map.Height;
            actor[2].SetFacingDegrees(-90);
            actor[2].SetLocation(20, 10, 70);
            actor[2].FacingOffset = MathHelper.ToRadians(90);
            actor[2].FOV = 60.0f;
            actor[2].UpdateWorldMatrix();
            actor[2].Behaviours = FLAG_Behaviours.MONSTER | FLAG_Behaviours.PATROLLER;
			actor[2].ScalingFactor = 1.0f;

            //Set up patrol list.
            Stage.Graph.SearchAStarPath(new Vector2(1, 4), new Vector2(1, 13));            
            actor[2].SimplePatrolList = Stage.Graph.GetPathList();            
            //The last node of this list will always be the same as the first node of the next list. This affects the direction in which the actor[0] looks
            //so.
            actor[2].SimplePatrolList.RemoveAt(actor[2].SimplePatrolList.Count - 1);
            Stage.Graph.SearchAStarPath(new Vector2(1,13), new Vector2(5, 13));
            actor[2].SimplePatrolList.AddRange(Stage.Graph.GetPathList());
            actor[2].SimplePatrolList.RemoveAt(actor[2].SimplePatrolList.Count - 1);
            Stage.Graph.SearchAStarPath(new Vector2(5, 13), new Vector2(5, 4));
            actor[2].SimplePatrolList.AddRange(Stage.Graph.GetPathList());
            actor[2].SimplePatrolList.RemoveAt(actor[2].SimplePatrolList.Count - 1);
            Stage.Graph.SearchAStarPath(new Vector2(5, 4), new Vector2(1, 4));
            actor[2].SimplePatrolList.AddRange(Stage.Graph.GetPathList());
            actor[2].SimplePatrolList.RemoveAt(actor[2].SimplePatrolList.Count - 1);

			actor[0].mInstanceName = "Wanderer";
			actor[1].mInstanceName = "Dwarf";
			actor[2].mInstanceName = "Patroller";

            //TODO: Add waypoints and build this list from waypoint data assuming the last step is always a move back from the last waypoint to the first.
            //TODO: Then add a second type of patrol behaviour that goes backwards through the list when it reaches the end.

			// NEW COLLISION/LOS MODEL DATA
			Stage.Map.CalculateWallCollision();

            base.Initialize();
        }

        protected override void LoadContent()
        {

			testmodel = Content.Load<Model>("Dwarf");
			tinymodel = Content.Load<Model>("Tiny");
			tigermodel = Content.Load<Model>("tigermodel");

           // LevelMapData map = new LevelMapData(40, 40);
            //map.SaveTofile("test.xml");

          //  map = LevelMapData.LoadFromFile("test.xml");
           // map.SaveTofile("test2.xml");                                      

            BackgroundMusic = Content.Load<Song>("Lv 1 draft 2");            

            device = graphics.GraphicsDevice;
            basicEffect = new BasicEffect(device, null);
            cCross = new CoordCross(device);

            font = Content.Load<SpriteFont>("SpriteFont1");

            myModel = Content.Load<Model>("box");
         
            barrel = Content.Load<Model>("Barrel");
            modelTransforms = new Matrix[myModel.Bones.Count];

            //Matrix.

            logo = Content.Load<Texture2D>("BaneLogoRed_final");
            crosshair = Content.Load<Texture2D>("BlueCircle");            

            //pointLightMesh = Content.Load<Model>("Meshes\\SphereLowPoly");

            //load the effects
            pointLightMeshEffect = Content.Load<Effect>("Effects\\PointLightMesh");

            if (graphics.GraphicsDevice.GraphicsDeviceCapabilities.
                PixelShaderVersion.Major >= 3)
            {
                baseEffect = Content.Load<Effect>("Effects\\MaterialShader30");
                lights = new PointLight[8];

             
                numLights = 1;
                baseEffect.Parameters["numLights"].SetValue(numLights);
                shaderVersionString = "Using Shader Model 3.0";
            }
            else
            {
                baseEffect = Content.Load<Effect>("Effects\\MaterialShader20");
                lights = new PointLight[2];
        
                numLights = 1;
                baseEffect.Parameters["numLights"].SetValue(numLights);
                shaderVersionString = "Using Shader Model 2.0";
            }

            // cache the effect parameters
            viewParameter = baseEffect.Parameters["view"];
            projectionParameter = baseEffect.Parameters["projection"];
            cameraPositionParameter = baseEffect.Parameters["cameraPosition"];

            // create the materials
            materials = new Material[8];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(Content, graphics.GraphicsDevice,
                    baseEffect);
            }
            floorMaterial = new Material(Content, graphics.GraphicsDevice,
                baseEffect);

            //materials[0].SetBasicProperties(Color.Purple, .5f, 1.2f);
            //materials[1].SetBasicProperties(Color.Orange, 8f, 2f);
            materials[2].SetBasicProperties(Color.White, 1f, 1f);
            materials[3].SetBasicProperties(Color.Red, 256f, 16f);
            //materials[4].SetTexturedMaterial(Color.CornflowerBlue, 64f, 4f, null,
            //    "Scratches", .5f, .5f);
            //materials[5].SetTexturedMaterial(Color.LemonChiffon, 32f, 4f, "Marble",
            //    "Scratches", 1f, 1f);

			materials[4].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity / 10, "box2", "box2",
                1f, 1f);
			materials[5].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity, "Body", "Body",
                1f, 1f);
			materials[6].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity / 10, "box4", "box4",
                1f, 1f);
			materials[7].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity *2, "wall_diffuse",
                "wall_specular", 1f, 1f);

			materials[0].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity, "Tiny_skin", "Tiny_skin",
				1f, 1f);
			materials[1].SetTexturedMaterial(Color.White, Stage.ConfigurationSettings.WallSpecularPower, Stage.ConfigurationSettings.WallSpecularIntensity, "tiger", "tiger",
				1f, 1f);

			//Ghost floor
			materials[1].SetBasicProperties(new Color(200,200,200,10), 256f, 16f);

			//materials[1].DiffuseTexture.

            //floorMaterial.SetTexturedMaterial(Color.White, 16f, .8f, "Grid",
            //    "Grid", 15f, 15f);


            baseEffect.Parameters["ambientLightColor"].SetValue(
                new Vector4(Stage.ConfigurationSettings.Ambient,Stage.ConfigurationSettings.Ambient,Stage.ConfigurationSettings.Ambient, 1.0f));

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

            // Init light 0

            lights[0] = new PointLight(new Vector4(0,0,0,1.0f)
                    , baseEffect.Parameters["lights"].Elements[0]);

            //randomize the color, range, and power of the lights
            lights[0].Range = Stage.ConfigurationSettings.TorchRange;
            lights[0].Falloff = Stage.ConfigurationSettings.TorchAttenuation;

            // always set light 0 to pure white as a reference point
            lights[0].Color = Color.AntiqueWhite;

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            Matrix modrix;                     

            modrix = Matrix.Identity;
            modrix *= Matrix.CreateTranslation(new Vector3(10.0f, 10.0f, 5.0f));

            //frontwall front
            testquad = new Quad(new Vector3(15.0f, 5.0f, -32.0f), Vector3.Backward, Vector3.Up, 16, 16);
            //frontwall back
            testquadb = new Quad(new Vector3(15.0f, 5.0f, -32.0f), Vector3.Forward, Vector3.Up, 16, 16);

            //leftwallfront
            testquad2 = new Quad(new Vector3(11.0f, 5.0f, -28.0f), Vector3.Right, Vector3.Up, 16, 16);
            //leftwallback
            testquad2b = new Quad(new Vector3(11.0f, 5.0f, -28.0f), Vector3.Left, Vector3.Up, 16, 16);

            //rightwallfront
            testquad3 = new Quad(new Vector3(19.0f, 5.0f, -28.0f), Vector3.Right, Vector3.Up, 16, 16);
            //rightwallback
            testquad3b = new Quad(new Vector3(19.0f, 5.0f, -28.0f), Vector3.Left, Vector3.Up, 16, 16);

            //floorfront
            testquad4 = new Quad(new Vector3(15.0f, 1.0f, -28.0f), Vector3.Up, Vector3.Right, 16, 16);

            //ceilfront
            testquad5 = new Quad(new Vector3(15.0f, 9.0f, -28.0f), Vector3.Down, Vector3.Left, 16, 16);

			flickerdelay = 0;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
			//Set the picking ray
			fpsCam.PickingRay = CalculateCursorRay(fpsCam.ProjectionMatrix, 
													fpsCam.ViewMatrix,new Vector2(
													graphics.PreferredBackBufferWidth/2,
													graphics.PreferredBackBufferHeight/2) );

			NearestWall = Stage.Graph.IntersectRayWithWalls(fpsCam.PickingRay, 128.0f);

			//See if any actors have been picked
			PickedActors.Clear();
			for(int a=0;a<3;a++)
			{
				float? dist = fpsCam.PickingRay.Intersects(new BoundingSphere(actor[a].CurrentLocation, 8.0f));
				if (dist.HasValue && dist<NearestWall)
				{
					PickedActors.Add(actor[a].InstanceName);
				}
			}

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(BackgroundMusic);
                MediaPlayer.IsRepeating = true;
            }
            //Microsoft.Xna.Framework.Content.Pipeline.Audio.AudioFileType.

            if (falling)
            {
                if (currentjump > 0)
                {
                    currentjump -= 0.25f;
                }
                else
                {
                    falling = false;
                }
            }

            if (jumping)
            {
                if (currentjump < 6.0)
                {
                    currentjump += 0.25f;
                }
                else
                {
                    jumping = false;
                    falling = true;
                }
            }


            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			if (gamePadState.Buttons.Back == ButtonState.Pressed)
			{
				Stage.DumpDebugToFile();    
				this.Exit();
			}

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            if(keyState.IsKeyDown(Keys.Space))
            {
                if(!jumping && !falling)
                {
                    jumping = true;
                }
            }

            if (keyState.IsKeyDown(Keys.Escape))
            {
				Stage.DumpDebugToFile();
                this.Exit();
            }

            fpsCam.Update(mouseState, keyState, gamePadState);

            //lights[0].Position = new Vector4(fpsCam.Position.X, fpsCam.Position.Y, fpsCam.Position.Z, 1.0f);

            Vector3 torchvector = new Vector3(fpsCam.LastTarget.X, fpsCam.LastTarget.Y, fpsCam.LastTarget.Z);                       
         
            lights[0].Position = new Vector4(torchvector.X, torchvector.Y, torchvector.Z, 1.0f);

			flickerdelay += (float)gameTime.ElapsedRealTime.TotalMilliseconds;

			//flame flicker 10.2Hz = 1 flicker every milliseconds
			if (flickerdelay > 98)
			{
				float flicker = 0.93f + (float)random.NextDouble() * 0.07f;
				lights[0].Color = new Color(flicker,flicker,flicker-0.1f);
				flickerdelay = 0;
			}            

            fpsCam.currentjump = this.currentjump;

            float milliseconds = (float)gameTime.ElapsedRealTime.TotalMilliseconds;
            fps = 1.0f / (milliseconds / 1000.0f);           

            //Make the actor[0] turn;
            //actor[0].SetFacingVetor3(fpsCam.Position);

            actor[0].Update();            
            actor[2].Update();

			// Test new LOS method

			NEWLOS = Stage.Graph.GetLOSByWallBBCollision(actor[1].CurrentLocation, fpsCam.Position, 100, ref NEWLOSCOLL);

            //for now must always send LOS
            IEnumerable<Vector2> path = new Queue<Vector2>();
			//LOS = Stage.Graph.GetLOS(Stage.Graph.GetGridReferenceFromVector(actor[1].CurrentLocation),
			//                                Stage.Graph.GetGridReferenceFromVector(fpsCam.Position), ref path);

            Queue<Vector2> q;

            if ((actor[1].Actions & FLAG_Actions.ALERTED) != 0)
            {
                if (actor[1].SimpleMoveQueue == null && actor[1].CurrentTarget == Vector2.Zero)
                {
                    Stage.Graph.SearchAStarPath(Stage.Graph.GetGridReferenceFromVector(actor[1].CurrentLocation),
                                                Stage.Graph.GetGridReferenceFromVector(fpsCam.Position));
                    //q = Stage.Graph.GetPathQueue();
					actor[1].Update(fpsCam.Position, Stage.Graph.GetPathQueue(), NEWLOS);
                }
                else
                {
                    if (actor[1].SimpleMoveQueue.Count < 1 && actor[1].CurrentTarget == Vector2.Zero)
                    {
                        Stage.Graph.SearchAStarPath(Stage.Graph.GetGridReferenceFromVector(actor[1].CurrentLocation),
                                                Stage.Graph.GetGridReferenceFromVector(fpsCam.Position));
						actor[1].Update(fpsCam.Position, Stage.Graph.GetPathQueue(), NEWLOS);
                    }
                    else
                    {
						actor[1].Update(fpsCam.Position, null, NEWLOS);
                    }
                }
            }
            else
            {
				actor[1].Update(fpsCam.Position, null, NEWLOS);
            }						

            base.Update(gameTime);       
        }

        protected override void Draw(GameTime gameTime)
        {
            numquads = 0;
            fpsCam.Setjump(currentjump + 10.0f);

			//GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.			

            graphics.GraphicsDevice.Clear(new Color(new Vector3(0.002f,0.002f,0.002f)));

            // enable the depth buffer since geometry will be drawn
            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
			// Depth bias, useful for making small near things look like large far away things!
			graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            // always set the shared effects parameters
            viewParameter.SetValue(fpsCam.ViewMatrix);
            cameraPositionParameter.SetValue(fpsCam.Position);                                  

			

            //Further optimisation needed - each quad should know which square it's in and the player's field of view should be taken into account

            BoundingSphere sphere = new BoundingSphere();
            BoundingFrustum frustum = new BoundingFrustum(fpsCam.ViewMatrix * fpsCam.ProjectionMatrix);
            Matrix ww;
           
			//Render actors
			materials[0].SpinMyBones = true;
			materials[0].Scaled = true;
			materials[0].SpecularIntensity /= 8;
			materials[0].SpecularPower /= 8;
			if(Vector3.Distance(actor[0].CurrentLocation,fpsCam.Position) < 160)			
				materials[0].DrawComplexModelWithMaterial(tinymodel, ref actor[0].WorldMatrix,string.Empty);		
			graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
			materials[1].Scaled = true;
			if (Vector3.Distance(actor[1].CurrentLocation, fpsCam.Position) < 160)
				materials[5].DrawComplexModelWithMaterial(testmodel, ref actor[1].WorldMatrix,string.Empty);
			graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			//if (Vector3.Distance(actor[2].CurrentLocation, fpsCam.Position) < 160)
				materials[5].DrawComplexModelWithMaterial(testmodel, ref actor[2].WorldMatrix, string.Empty);

            foreach (Quad q in Stage.Map.Floors)
            {              				
                if (q.Origin.X*2 < fpsCam.Position.X + 160 && q.Origin.X*2 > fpsCam.Position.X - 160
                    && q.Origin.Z*2 < fpsCam.Position.Z + 160 && q.Origin.Z*2 > fpsCam.Position.Z -160)
                {
					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);

                    if (frustum.Intersects(sphere))
                    {
                        ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
                        materials[7].DrawQuadWithMaterial(q, ref ww);
                        numquads++;
                    }
                }
            }
			foreach (Quad q in Stage.Map.Ceilings)
			{
				if (q.Origin.X * 2 < fpsCam.Position.X + 160 && q.Origin.X * 2 > fpsCam.Position.X - 160
					&& q.Origin.Z * 2 < fpsCam.Position.Z + 160 && q.Origin.Z * 2 > fpsCam.Position.Z - 160)
				{
					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);

					if (frustum.Intersects(sphere))
					{
						ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
						materials[7].DrawQuadWithMaterial(q, ref ww);
						numquads++;
					}
				}
			}
			foreach (Quad q in Stage.Map.Walls)
			{
				if (q.Origin.X * 2 < fpsCam.Position.X + 160 && q.Origin.X * 2 > fpsCam.Position.X - 160
					&& q.Origin.Z * 2 < fpsCam.Position.Z + 160 && q.Origin.Z * 2 > fpsCam.Position.Z - 160)
				{

					sphere = new BoundingSphere(new Vector3(q.Origin.X * 2, q.Origin.Y, q.Origin.Z * 2), 16.0f);


					if (frustum.Intersects(sphere))
					{
						ww = Matrix.Identity * Matrix.CreateTranslation(q.Origin);
						materials[7].DrawQuadWithMaterial(q, ref ww);
						numquads++;
					}
				}
			}

            //baseEffect.Parameters["ambientLightColor"].SetValue(
            //   new Vector4(.185f, .185f, .185f, 1.0f));
            //lights[0].Range = 500f;
            //lights[0].Falloff = 0f;
          //  materials[6].DrawModelWithMaterial(barrel, ref meshWorlds[1]);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(logo, new Vector2(10, 10), Color.White);              

            spriteBatch.Draw(crosshair,
                new Vector2((float)graphics.PreferredBackBufferWidth/2 - (float)crosshair.Width/2,(float)graphics.PreferredBackBufferHeight/2 - (float)crosshair.Height/2),
                new Color(250,250,250,75));

            spriteBatch.DrawString(font, "GRID X: " + fpsCam.LastGridSquareX.ToString() + " y: " + fpsCam.LastGridSquareY.ToString(), new Vector2(10, 200), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "FACING: " + fpsCam.GetFacingdegrees().ToString(), new Vector2(10, 220), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "CAM  X: " + fpsCam.Position.X.ToString() + " Z: " + fpsCam.Position.Z.ToString(), new Vector2(10, 240), Color.WhiteSmoke);

            if (fps > 60)
            {
                spriteBatch.DrawString(font, "FPS   : >60", new Vector2(10, 280), Color.Green);
            }
            else if (fps <= 60 && fps > 45)
            {
                spriteBatch.DrawString(font, "FPS   : " + fps.ToString(), new Vector2(10, 280), Color.Yellow);
            }
            else if (fps <= 45)
            {
                spriteBatch.DrawString(font, "FPS   : " + fps.ToString(), new Vector2(10, 280), Color.Red);
            }

            spriteBatch.DrawString(font, "Wall Verts : " + (numquads*4).ToString(), new Vector2(10, 300), Color.WhiteSmoke);

            //Actor debugs

            // 0 = order move; 1 = aggressive; 2 = patrol move

            //  0 Location
            //  0 Target            
            //  0 Behav
            //  0 State
            //  0 Qcount 
            //
            //  1 Location
            //  1 Target
            //  1 Behav
            //  1 State
            //
            //  2 Location
            //  2 Target
            //  2 Behav
            //  2 State
            //  2 Patrol pos
            //            

            //Start at 340

            if (float.IsNaN(actor[1].CurrentLocation.X))
            {
                throw new Exception("NAN!");
            }

            //0
            spriteBatch.DrawString(font, "actor[0] Loc.   : " + actor[0].CurrentLocation.X.ToString() + ", " + actor[0].CurrentLocation.Y.ToString() + ", " + actor[0].CurrentLocation.Z.ToString(), new Vector2(10, 340), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[0] Target : " + actor[0].CurrentTarget.X.ToString() + ", " + actor[1].CurrentTarget.Y.ToString(), new Vector2(10, 360), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[0] Behav. : " + actor[0].Behaviours.ToString(), new Vector2(10, 380), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[0] State  : " + actor[0].Actions.ToString(), new Vector2(10, 400), Color.WhiteSmoke);
            if (actor[0].SimpleMoveQueue != null)            
                spriteBatch.DrawString(font, "actor[0] Queue  : " + actor[0].SimpleMoveQueue.Count.ToString(), new Vector2(10, 420), Color.WhiteSmoke);            
            else
                spriteBatch.DrawString(font, "actor[0] Queue  : NULL", new Vector2(10, 420), Color.WhiteSmoke);
            //1
            spriteBatch.DrawString(font, "actor[1] Loc.   : " + actor[1].CurrentLocation.X.ToString() + ", " + actor[1].CurrentLocation.Y.ToString() + ", " + actor[1].CurrentLocation.Z.ToString(), new Vector2(10, 460), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[1] Target : " + actor[1].CurrentTarget.X.ToString() + ", " + actor[1].CurrentTarget.Y.ToString() + "GRID: " + actor[1].target_gridX.ToString() + "," + actor[1].target_gridY.ToString(), new Vector2(10, 480), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[1] Behav. : " + actor[1].Behaviours.ToString(), new Vector2(10, 500), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[1] State  : " + actor[1].Actions.ToString(), new Vector2(10, 520), Color.WhiteSmoke);
            //2
            spriteBatch.DrawString(font, "actor[2] Loc.   : " + actor[2].CurrentLocation.X.ToString() + ", " + actor[2].CurrentLocation.Y.ToString() + ", " + actor[2].CurrentLocation.Z.ToString(), new Vector2(10, 560), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[2] Target : " + actor[2].CurrentTarget.X.ToString() + ", " + actor[2].CurrentTarget.Y.ToString(), new Vector2(10, 580), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[2] Behav. : " + actor[2].Behaviours.ToString(), new Vector2(10, 600), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[2] State  : " + actor[2].Actions.ToString(), new Vector2(10, 620), Color.WhiteSmoke);
            spriteBatch.DrawString(font, "actor[2] Patrol: " + actor[2].CurrentPatrolIndex.ToString(), new Vector2(10, 640), Color.WhiteSmoke);

			string pa = string.Empty;
			foreach(string s in PickedActors)
			{
				pa += s + " ";
			}

			spriteBatch.DrawString(font, "Picked Actors   : " + pa.ToString() , new Vector2(10, 680), Color.WhiteSmoke);
			spriteBatch.DrawString(font, "Walldist        : " + NearestWall.ToString(), new Vector2(10, 720), Color.WhiteSmoke);

            spriteBatch.End();

            base.Draw(gameTime);
           
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
    }
}
