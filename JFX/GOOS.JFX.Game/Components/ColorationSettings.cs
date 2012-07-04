using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Game.Components
{
	/// <summary>
	/// Coloration settings for the post processing component.
	/// </summary>
	public class ColorationSettings
	{
		#region Members

		private bool mFloodFill;
		private bool mMono;
		private bool mSepia;

		private float mFloodRed;
		private float mFloodGreen;
		private float mFloodBlue;

		#endregion

		#region Properties

		/// <summary>
		/// Red Component of the flood fill
		/// </summary>
		public float FloodBlue
		{
			get { return mFloodBlue; }
			set { mFloodBlue = value; }
		}

		/// <summary>
		/// Green Component of the flood fill
		/// </summary>
		public float FloodGreen
		{
			get { return mFloodGreen; }
			set { mFloodGreen = value; }
		}

		/// <summary>
		/// Red Component of the flood fill
		/// </summary>
		public float FloodRed
		{
			get { return mFloodRed; }
			set { mFloodRed = value; }
		}

		/// <summary>
		/// True if the sepia effect is enabled.
		/// </summary>
		public bool Sepia
		{
			get { return mSepia; }
			set { mSepia = value; }
		}

		/// <summary>
		/// True if the mono effect is enabled (applied before sepia)
		/// </summary>
		public bool Mono
		{
			get { return mMono; }
			set { mMono = value; }
		}

		/// <summary>
		/// True if the flood fill effect is enabled (applied before mono and sepia)
		/// </summary>
		public bool FloodFill
		{
			get { return mFloodFill; }
			set { mFloodFill = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ColorationSettings()
		{
			mFloodFill = false;
			mSepia = false;
			mMono = false;
			mFloodRed = 0.0f;
			mFloodGreen = 0.0f;
			mFloodBlue = 0.0f;
		}

		#endregion
	}
}
