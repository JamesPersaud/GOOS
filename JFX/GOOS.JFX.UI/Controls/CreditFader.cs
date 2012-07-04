using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// A simple label for text display
	/// </summary>
	public class CreditFader : BaseGameControl
	{
		#region Members

		private List<GameCredit> mCredits;
		private SpriteFont mFont;
		private int mCurrentCreditIndex;
		private int mLastSwitchTime;
		private float mCurrentFadeLevel;
		private GameCredit mCurrentCredit;
		private GameCredit mNextCredit;

		#endregion

		#region properties

		/// <summary>
		/// The next credit
		/// </summary>
		public GameCredit NextCredit
		{
			get { return mNextCredit; }
			set { mNextCredit = value; }
		}
		/// <summary>
		/// The current credit.
		/// </summary>
		public GameCredit CurrentCredit
		{
			get { return mCurrentCredit; }
			set { mCurrentCredit = value; }
		}
		/// <summary>
		/// The current level of fade 0 = next credit 1 = current credit.
		/// </summary>
		public float CurrentFadeLevel
		{
			get { return mCurrentFadeLevel; }
			set { mCurrentFadeLevel = value; }
		}

		/// <summary>
		/// The last time in total game time that the credits were switched.
		/// </summary>
		public int LastSwitchTime
		{
			get { return mLastSwitchTime; }
			set { mLastSwitchTime = value; }
		}
		/// <summary>
		/// The index of the currently displayed credit.
		/// </summary>
		public int CurrentcreditIndex
		{
			get { return mCurrentCreditIndex; }
			set { mCurrentCreditIndex = value; }
		}
		/// <summary>
		/// Get or Set the font to use.
		/// </summary>
		public SpriteFont Font
		{
			get { return mFont; }
			set { mFont = value; }
		}
		/// <summary>
		/// The credits to be displayed by this credit fader
		/// </summary>
		public List<GameCredit>Credits
		{
			get { return mCredits; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		private CreditFader()
			: base()
		{
			CurrentcreditIndex = 0;
			mCredits = new List<GameCredit>();
		}

		#endregion

		#region Factory

		public static CreditFader CreateNew(string name, SpriteFont font)
		{
			CreditFader cf = new CreditFader();
			cf.Name = name;
			cf.Font = font;
			cf.Enabled = false; //This makes sure the credit fader never gets focus.
			cf.Visible = true;
			return cf;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a new credit to the fader.
		/// </summary>
		/// <param name="gc">The credit to add</param>
		public void AddCredit(GameCredit gc)
		{
			mCredits.Add(gc);
		}

		#endregion

		#region Event Handler Overrides

		/// <summary>
		/// Do this in addiition to the main update.
		/// </summary>
		protected override void BaseGameControl_Update(IGameControl sender, GameControlTimedEventArgs args)
		{
			//First update
			if (LastSwitchTime == 0)
			{
				LastSwitchTime = (int)args.TotalTime;
				CurrentcreditIndex = 0;

				CurrentCredit = mCredits[CurrentcreditIndex];
				NextCredit = mCredits[(CurrentcreditIndex + 1) % mCredits.Count];
			}			

			//All updates
			if (mCredits != null && mCredits.Count > 0)
			{				
				//time to next switch is delay time - time since last switch
				int timeToSwitch = CurrentCredit.Delay - ((int)args.TotalTime - LastSwitchTime);

				if (timeToSwitch > 0)
				{
					//If in last 2 seconds, apply fade
					if (timeToSwitch < 4001)
						CurrentFadeLevel = (float)timeToSwitch / 4000.0f;
					else
						CurrentFadeLevel = 1.0f;
				}
				else //Switch
				{
					//Set credit and next credit.
					CurrentFadeLevel = 1.0f;
					CurrentcreditIndex = (CurrentcreditIndex + 1) % mCredits.Count;					
					CurrentCredit = mCredits[CurrentcreditIndex];
					NextCredit = mCredits[(CurrentcreditIndex + 1) % mCredits.Count];
					LastSwitchTime = (int)args.TotalTime;
				}
			}
			
			//Do the base update
			base.BaseGameControl_Update(sender, args);
		}

		/// <summary>
		/// Do this instead of the usual render method
		/// </summary>
		protected override void BaseGameControl_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			//The basic render for a control is just to draw the skin texture at its location (relative to the form location
			Vector3 absolute = GetAbsoluteLocation();
			Vector2 screenloc = new Vector2(absolute.X, absolute.Y);
			float depth = absolute.Z;			

			//Draw locations for current
			Vector2 roleloc = screenloc + new Vector2((float)CurrentCredit.RoleOffset.X, (float)CurrentCredit.RoleOffset.Y);
			Vector2 nameloc = screenloc + new Vector2((float)CurrentCredit.NamesOffset.X, (float)CurrentCredit.NamesOffset.Y);
			Vector4 cv = CurrentCredit.Colour;
			cv.W = CurrentFadeLevel;		
			Color colour = new Color(cv);						

			//Draw locations for next
			Vector2 nextroleloc = screenloc + new Vector2((float)CurrentCredit.RoleOffset.X, (float)CurrentCredit.RoleOffset.Y);
			Vector2 nextnameloc = screenloc + new Vector2((float)CurrentCredit.NamesOffset.X, (float)CurrentCredit.NamesOffset.Y);
			Vector4 ncv = NextCredit.Colour;
			ncv.W = (1.0f - CurrentFadeLevel);		
			Color nextcolour = new Color(ncv);

			//Draw Current
			args.spriteBatch.DrawString(Font, CurrentCredit.Role, roleloc, colour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			//Draw Next
			args.spriteBatch.DrawString(Font, NextCredit.Role, nextroleloc, nextcolour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth+0.1f);

			//Current Names
			if (CurrentCredit.Names.Length > 0)
			{
				string currentnames = CurrentCredit.Names;
				args.spriteBatch.DrawString(Font, currentnames, nameloc, colour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			}
			//Next Names
			if (NextCredit.Names.Length > 0)
			{
				string nextnames = NextCredit.Names;
				args.spriteBatch.DrawString(Font, nextnames, nextnameloc, nextcolour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			}
		}

		#endregion
	}

	#region Game Credit Struct

	//A struct used by this class to hold individual credits (all with the same font)
	public struct GameCredit
	{
		public string Role;
		public string Names;
		public Point RoleOffset;
		public Point NamesOffset;
		public int Delay;
		public Vector4 Colour;
		public float Scale;

		/// <summary>
		/// Construct a new Credit
		/// </summary>
		/// <param name="r">Role</param>
		/// <param name="n">Name</param>
		/// <param name="ro">Role Offset</param>
		/// <param name="no">Name offset</param>
		/// <param name="d">Delay</param>
		/// <param name="c">Colour</param>
		/// <param name="s">Scale</param>
		public GameCredit(string r, string n, Point ro, Point no, int d, Vector4 c, float s)
		{
			Role = r;
			Names = n;
			RoleOffset = ro;
			NamesOffset = no;
			Delay = d;
			Colour = c;
			Scale = s;
		}
	}

	#endregion
}
