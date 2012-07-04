using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Game.Components
{
	/// <summary>
	/// Post Fade settings for the post processing component
	/// 
	/// </summary>
	public class PostFadeSettings
	{
		#region Members

		private bool mFadeTo;
		private float mFadeLevel;
		private float mTargetRed;
		private float mTargetGreen;
		private float mTargetBlue;
		private bool mOn;

		#endregion

		#region Properties

		public bool On
		{
			get { return mOn; }
			set { mOn = value; }
		}

		/// <summary>
		/// The red component of the target colour 0.0 - 1.0
		/// </summary>
		public float Red
		{
			get { return mTargetRed; }
			set { mTargetRed = value; }
		}

		/// <summary>
		/// The green component of the target colour 0.0 - 1.0
		/// </summary>
		public float Green
		{
			get { return mTargetGreen; }
			set { mTargetGreen = value; }
		}

		/// <summary>
		/// The blue component of the target colour 0.0 - 1.0
		/// </summary>
		public float Blue
		{
			get { return mTargetBlue; }
			set { mTargetBlue = value; }
		}

		/// <summary>
		/// Set to true to fade to the target color, otherwise fade direction is from the target color.
		/// </summary>
		public bool FadeTo
		{
			get { return mFadeTo; }
			set { mFadeTo = value; }
		}

		/// <summary>
		/// The position through the fade to be rendered 0.0 - 1.0.
		/// </summary>
		public float Level
		{
			get { return mFadeLevel; }
			set { mFadeLevel = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Easy use public default constructor
		/// </summary>
		public PostFadeSettings()
		{
			//Default is fade to black
			FadeTo = true;
			Red = 0.0f;
			Green = 0.0f;
			Blue = 0.0f;
			Level = 0.0f;
			On = false;
		}

		#endregion
	}
}
