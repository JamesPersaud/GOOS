using System;
using System.Collections.Generic;
using GOOS.JFX.Core;
using GOOS.JFX.Entity;
using GOOS.JFX.Level;
using GOOS.JFX.Scripting;
using EMK.Cartography;
using Microsoft.Xna.Framework;
using System.IO;

namespace GOOS.JFX.Game
{
	/// <summary>
	/// 
	/// Stage manager is the highest level of abstraction besides the game itself and should contain all game logic that
	/// involves more than one single game entity or type of entity.
	/// 
	/// Stage Manager is responsible for ensuring that actors, props and effects all behave according to script
	/// and follow direction.
	/// 
	/// Stage manager owns the following:
	/// 
	/// Actors
	/// Props
	/// Maps
	/// Map Graphs
	/// Scripts
	/// The scripting engine
	/// 
	/// And is owned by the highest level object (the game -or director- itself)
	/// 
	/// TODO:
	/// 
	/// Load Entity "cast list" from level data
	/// Execute initial scripts for cast members
	/// 
	/// :::
	/// </summary>
	public class StageManager : IScriptable
	{
		#region IScriptable Members

		public string AssemblyName
		{
			get { return "Grimwood.Bane.Game"; }
		}

		public string ClassName
		{
			get { return "Grimwood.Bane.Game.StageManager"; }
		}

		public Stack<KeyValuePair<string, string>> ScriptingFunctions
		{
			get 
			{ 
				Stack<KeyValuePair<string, string>> s = new Stack<KeyValuePair<string,string>>();

				s.Push(new KeyValuePair<string, string>("GetStageManager","GetStageManager"));

				return s; 			
			}
		}

		public string InitialScript
		{
			get
			{
				return "Stage = GetStageManager()";
			}
			set
			{
				// do nothing
			}
		}

		#endregion

		#region Members

		private LUAScriptWriter mScripter;			//The scripting engine wrapper.
	
		private IList<IEntity> mEntitiesOutOfPlay;	//Loaded instances of this list begin offstage 											
		private IList<IEntity> mEntitiesInPlay;		//Loaded instances of entities currently in play

		private LevelData mLevel;				//Loaded level
		private LevelMapData mMap;				//Loaded map
		private LevelMapGraph mGraph;			//Loaded map graph

		private GeneralConfig mCurrentConfiguration;	//Loaded config file
		private string mDebugBuffer;					//Debug buffer for dumping to text file.
		private string mBuffer;							//An internal buffer for string building
		private double mGameTime;

		private GameStateSettings mGameState;

		#endregion

		#region Properties

		/// <summary>
		/// The current game state settings for this game.
		/// </summary>
		public GameStateSettings GameState
		{
			get { return mGameState; }
			set { mGameState = value; }
		}

		/// <summary>
		/// Gets the debug buffer (a string which holds any debug info reported to the manager
		/// </summary>
		public string DebugBuffer
		{
			get{return mDebugBuffer;}
		}

		/// <summary>
		/// Gets the pathfinding graph of the map of the current level being managed.
		/// </summary>
		public LevelMapGraph Graph
		{
			get{return mGraph;}
		}

		/// <summary>
		/// Gets the map data of the current level being managed.
		/// </summary>
		public LevelMapData Map
		{
			get{return mMap;}
		}

		/// <summary>
		/// Gets the current level being managed.
		/// </summary>
		public LevelData Level
		{
			get{return mLevel;}
		}

		/// <summary>
		/// Gets the current LUA scripting interface wrapper.
		/// </summary>
		public LUAScriptWriter ScriptWriter
		{
			get{return mScripter;}
		}

		/// <summary>
		/// Gets the current game config settings.
		/// </summary>
		public GeneralConfig ConfigurationSettings
		{
			get{return mCurrentConfiguration;}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// The default private constructor.
		/// </summary>
		private StageManager()
		{
			mGameState = GameStateSettings.EmptySettings();
			mDebugBuffer = string.Empty;
			mBuffer = string.Empty;
			mEntitiesOutOfPlay = new List<IEntity>();
			mEntitiesInPlay = new List<IEntity>();
			mScripter = LUAScriptWriter.CreateNew();
			mScripter.LoadAndImportFromInstance(this,out this.mBuffer);			
			mGameTime = 0.0;			
			WriteDebugLine("Stage Manager instantiated");
		}

		#endregion

		#region Factory		

		/// <summary>
		/// Create a new instance of stage manager with no maps or entities.
		/// </summary>
		/// <returns>An new stage manager object</returns>
		public static StageManager EmptyStage()
		{			
			return new StageManager();
		}

		/// <summary>
		/// Create a new instance of stage manager with config,
		/// default level and default map specified by a config file.
		/// </summary>
		/// <param name="filename">The name of the config file</param>
		/// <returns>A new stage manager object</returns>
		public static StageManager CreateFromConfigFile(string filename)
		{
			StageManager sm = new StageManager();
			sm.LoadConfigFromFile(filename);
			return sm;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Update the stage
		/// </summary>
		/// <param name="actors">update the actors</param>
		/// <param name="props">update the props</param>
		/// <param name="scenery">update the scenery</param>
		/// <param name="PlayerLocation">3D Location of the player</param>
		public void Update(bool actors, bool props, bool scenery, Vector3 PlayerLocation)
		{
			if (actors)
			{
				foreach (Actor a in mEntitiesInPlay)
				{
					//Update aggressive actors.
					if ((a.Behaviours & FLAG_Behaviours.AGGRESSIVE) > 0)
					{
						Vector3 NEWLOSCOLL = new Vector3();
						bool NEWLOS = Graph.GetLOSByWallBBCollision(a.CurrentLocation, PlayerLocation, 100, ref NEWLOSCOLL);

						if ((a.Actions & FLAG_Actions.ALERTED) != 0)
						{
							if (a.SimpleMoveQueue == null && a.CurrentTarget == Vector2.Zero)
							{
								Graph.SearchAStarPath(Graph.GetGridReferenceFromVector(a.CurrentLocation),
															Graph.GetGridReferenceFromVector(PlayerLocation));
								a.Update(PlayerLocation, Graph.GetPathQueue(), NEWLOS);
							}
							else
							{
								if (a.SimpleMoveQueue == null || (a.SimpleMoveQueue.Count < 1 && a.CurrentTarget == Vector2.Zero))
								{
									Graph.SearchAStarPath(Graph.GetGridReferenceFromVector(a.CurrentLocation),
															Graph.GetGridReferenceFromVector(PlayerLocation));
									a.Update(PlayerLocation, Graph.GetPathQueue(), NEWLOS);
								}
								else
								{
									a.Update(PlayerLocation, null, NEWLOS);
								}
							}
						}
						else
						{
							a.Update(PlayerLocation, null, NEWLOS);
						}
					}
					else
					{
						a.Update();
					}
				}
			}
		}

		/// <summary>
		/// Render the stage
		/// </summary>
		/// <param name="actors">Render actors</param>
		/// <param name="props">Render Props</param>
		/// <param name="scenery">Render Scenery</param>
		public void Render(bool actors, bool props, bool scenery)
		{
			if (actors)
			{
				foreach (Actor a in mEntitiesInPlay)				
					a.Render();				
			}
		}
		public void Render(bool actors, bool props, bool scenery,QuakeCamera cam,float dist)
		{
			if (actors)
			{
				foreach (Actor a in mEntitiesInPlay)
					a.Render(cam,dist);
			}
		}

		/// <summary>
		/// Remove All Actors Props and Scenery objects from play and/or offstage.
		/// </summary>
		/// <param name="clearonstage">Clear the onstage list.</param>
		/// <param name="clearoffstage">Clear the offstage list.</param>
		public void ClearEntities(bool clearonstage,bool clearoffstage)
		{
			if (clearoffstage)
				this.mEntitiesOutOfPlay.Clear();
			if (clearonstage)
				this.mEntitiesInPlay.Clear();			
		}

		/// <summary>
		/// Generate and initialise a new random square map and graph.
		/// </summary>
		/// <param name="Dimention">The side length of the square map.</param>
		public void GenerateNewRandomSquareMap(int Dimention, int wallstobreak)
		{
			mMap = RandomMapGenerator.RandomSquareLevelMap(Dimention, null, wallstobreak);			
			mGraph = LevelMapGraph.NewGraphFromMap(mMap);	
		}

		/// <summary>
		/// Scripting access to this object
		/// </summary>
		/// <returns>this instance</returns>
		public StageManager GetStageManager()
		{
			return this;
		}

		/// <summary>
		/// Writes a new line to the debug buffer.
		/// </summary>
		/// <param name="debugtext">The text of the new line</param>
		/// /// <param name="takenewline">True if a blank line should appear above this line</param>
		public void WriteDebugLine(string debugtext,bool takenewline)
		{
			if (takenewline) mDebugBuffer += "\r\n";
			mDebugBuffer += mGameTime.ToString().PadRight(10) + debugtext + "\r\n";
		}

		/// <summary>
		/// Writes a new line to the debug buffer.
		/// </summary>
		/// <param name="debugtext">The text of the new line</param>
		public void WriteDebugLine(string debugtext)
		{
			WriteDebugLine(debugtext, false);
		}

		/// <summary>
		/// Dumps debug text to a file
		/// </summary>
		/// <param name="filename">The name of the file</param>
		public void DumpDebugToFile(string filename)
		{			
			File.WriteAllText(filename, mDebugBuffer);
		}

		/// <summary>
		/// Dumps debug text to "debug_sm.txt"
		/// </summary>
		public void DumpDebugToFile()
		{
			DumpDebugToFile("debug_sm.txt");
		}

		/// <summary>
		/// Loads the game config from a config file
		/// This will also load the default level
		/// </summary>
		/// <param name="filename">The name of the config file to load</param>
		public void LoadConfigFromFile(string filename)
		{
			WriteDebugLine("BEGIN LOADING CONFIG FILE",true);
			//Load config and default level			
			mCurrentConfiguration = GeneralConfig.LoadFromFile(filename);
			WriteDebugLine("Loaded Config from " + filename);
			mLevel = LevelData.Load(mCurrentConfiguration.DefaultLevel);
			WriteDebugLine("Loaded Level from " + mCurrentConfiguration.DefaultLevel);
			mMap = LevelMapData.LoadFromFile(mLevel.LevelMapFileName);
			WriteDebugLine("Loaded Map from " + mLevel.LevelMapFileName);
			//Create pathfinding graph for level
			mGraph = LevelMapGraph.NewGraphFromMap(mMap);
			WriteDebugLine("Created pathfinding graph from map");
			//Add config to scripting
			mScripter.LoadAndImportFromInstance(mCurrentConfiguration,out mBuffer);
			WriteDebugLine("Imported Config into Scripting Engine: " + mBuffer);
			WriteDebugLine("END LOADING CONFIG FILE"); 
		}

		/// <summary>
		/// Test Harness Output Method
		/// </summary>		
		public string ReportActors(bool onstage)
		{
			string s = string.Empty;
			if (onstage)
			{
				s = "\r\nOnstage:";
				if (mEntitiesInPlay.Count > 0)
				{
					foreach (IEntity a in mEntitiesInPlay)
						s += "\r\n" + a.InstanceName + " " + a.InstanceID;
				}
				else				
					s += "\r\nNone";				
			}
			else
			{
				s = "\r\nOffStage:";
				if (mEntitiesOutOfPlay.Count > 0)
				{
					foreach (IEntity a in mEntitiesOutOfPlay)
						s += "\r\n" + a.InstanceName + " " + a.InstanceID;
				}
				else
					s += "\r\nNone";			
			}

			return s;
		}

		/// <summary>
		/// Load an actor to the offstage list
		/// </summary>
		/// <param name="a">a pre-existing instance of an actor</param>
		public void LoadActor(Actor a)
		{			
			mEntitiesOutOfPlay.Add(a);						
		}

		/// <summary>
		/// Move an instance of an actor from the off stage list to the on stage list.
		/// </summary>
		/// <param name="instancename">3rd search criteria - possibly not unique</param>
		/// <param name="instanceid">2nd search criteria - unique to an instance</param>
		/// <param name="appearanceid">1st search criterian - unique to an instance within a specific scene</param>
		/// <param name="a">an instance of an actor</param>
		/// <returns>true if successful, false if actor not found off stage</returns>
		public bool BringActorOnStage(string instancename, string instanceid, string appearanceid, out Actor a)
		{
			a = GetActor(instancename, instanceid, appearanceid, false);
			if (a == null) return false;

			mEntitiesOutOfPlay.Remove(a);
			mEntitiesInPlay.Add(a);
			return true;
		}
		public bool BringActorOnStage(string instancename)
		{
			Actor a = new Actor();
			return BringActorOnStage(instancename, string.Empty, string.Empty, out a);
		}

		/// <summary>
		/// Move an instance of an actor from the on stage list to the off stage list.
		/// </summary>
		/// <param name="instancename">3rd search criteria - possibly not unique</param>
		/// <param name="instanceid">2nd search criteria - unique to an instance</param>
		/// <param name="appearanceid">1st search criterian - unique to an instance within a specific scene</param>
		/// <param name="a">an instance of an actor</param>
		/// <returns>true if successful, false if actor not found on stage</returns>
		public bool TakeActorOffStage(string instancename, string instanceid, string appearanceid,out Actor a)
		{
			a = GetActor(instancename, instanceid, appearanceid, true);
			if (a == null) return false;

			mEntitiesInPlay.Remove(a);
			mEntitiesOutOfPlay.Add(a);
			return true;
		}

		/// <summary>
		/// Get an instance of an actor
		/// </summary>
		/// <param name="instancename">3rd search criteria - possibly not unique</param>
		/// <param name="instanceid">2nd search criteria - unique to an instance</param>
		/// <param name="appearanceid">1st search criterian - unique to an instance within a specific scene</param>
		/// <param name="onstage">set to true if the actor is currently on stage</param>
		/// <returns>an instance of an actor</returns>
		private Actor GetActor(string instancename, string instanceid, string appearanceid, bool onstage)
		{
			if (onstage)
			{
				foreach (IEntity a in mEntitiesInPlay)
					if (a.AppearanceID != null && a.AppearanceID == appearanceid)
						return (Actor)a;
					else if (a.InstanceID == instanceid)
						return (Actor)a;
					else if (a.InstanceName == instancename)
						return (Actor)a;
			}
			else
			{
				foreach (IEntity a in mEntitiesOutOfPlay)
					if (a.AppearanceID != null && a.AppearanceID == appearanceid)
						return (Actor)a;
					else if (a.InstanceID == instanceid)
						return (Actor)a;
					else if (a.InstanceName == instancename)
						return (Actor)a;
			}
			return null;
		}

		#endregion		
	}
}
