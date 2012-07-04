using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Game
{
	/// <summary>
	/// This class holds high level state settings for the game and is used to keep track of whether the game is paused, 
	/// on demo mode, displaying the menu, displaying the colsole window, etc.
	/// 
	/// Most games will key off of common states such as those mentioned above, however, this class can also be used for management
	/// of extended states or as a collection of "global variables" and as such a GameStateSettings object should always be
	/// visible to the scripting language.
	/// 
	/// </summary>
	public class GameStateSettings
	{
		#region Members

		private bool mIntroSequence;
		private bool mPause;
		private bool mShowConsole;
		private bool mShowMenu;
		private bool mDemoMode;

		private Dictionary<string, bool> mExtendedBooleanSettings;
		private Dictionary<string, int> mExtendedIntSettings;
		private Dictionary<string, string> mExtendedStringSettings;
		private Dictionary<string, float> mExtendedFloatSettings;	 

		#endregion

		#region Properties

		/// <summary>
		/// True if the game is displaying the intro sequence
		/// </summary>
		public bool Intro
		{
			get { return mIntroSequence; }
			set { mIntroSequence = value; }
		}

		/// <summary>
		/// True if the game is running in demo mode
		/// </summary>
		public bool DemoMode
		{
			get { return mDemoMode; }
			set { mDemoMode = value; }
		}

		/// <summary>
		/// True if the game is displaying the menu overlay
		/// </summary>
		public bool Menu
		{
			get { return mShowMenu; }
			set { mShowMenu = value; }
		}

		/// <summary>
		/// True if the game is displaying the console window (usually scripting window)
		/// </summary>
		public bool Console
		{
			get { return mShowConsole; }
			set { mShowConsole = value; }
		}

		/// <summary>
		/// True if the game is paused
		/// </summary>
		public bool Pause
		{
			get { return mPause; }
			set { mPause = value; }
		}
		
		#endregion

		#region Constructors

		/// <summary>
		/// Default private constructor
		/// </summary>
		private GameStateSettings()
		{
			mExtendedBooleanSettings = new Dictionary<string, bool>();
			mExtendedFloatSettings = new Dictionary<string, float>();
			mExtendedIntSettings = new Dictionary<string, int>();
			mExtendedStringSettings = new Dictionary<string, string>();
		}

		#endregion

		#region Factory

		//TODO: Load/Save/Load Template

		/// <summary>
		/// Get a new game state object with all settings false and no extended settings.
		/// </summary>
		/// <returns>A new GameStateSettings object</returns>
		public static GameStateSettings EmptySettings()
		{
			return new GameStateSettings();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add or Modify a setting
		/// </summary>
		/// <param name="key">The name of the setting</param>
		/// <param name="value">The value of the setting</param>
		public void Set(string key,bool value)
		{
			mExtendedBooleanSettings.Add(key, value);
		}
		public void Set(string key, int value)
		{
			mExtendedIntSettings.Add(key, value);
		}
		public void Set(string key, string value)
		{
			mExtendedStringSettings.Add(key, value);
		}
		public void Set(string key, float value)
		{
			mExtendedFloatSettings.Add(key, value);
		}

		/// <summary>
		/// Get the value of a setting
		/// </summary>
		/// <param name="key">The name of the setting</param>
		/// <param name="b">An object to hold the value of the setting</param>
		/// <returns>The value of the setting</returns>
		public bool Get(string key, out bool b)
		{
			b = mExtendedBooleanSettings[key];
			return b;
		}
		public int Get(string key, out int i)
		{
			i = mExtendedIntSettings[key];
			return i;
		}
		public string Get(string key, out string s)
		{
			s = mExtendedStringSettings[key];
			return s;
		}
		public float Get(string key, out float f)
		{
			f = mExtendedFloatSettings[key];
			return f;
		}



		#endregion
	}
}
