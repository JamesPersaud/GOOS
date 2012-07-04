using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Game.Components
{
	/// <summary>
	/// Input variables to a sine wave effect post process effect.
	/// 
	/// Can also represent cosine waves with a phase value of +/- pi/2 radians or 90 degrees.
	/// 
	/// cos(x) = sin(x + pi/2)
	/// 
	/// </summary>
	public class SineSettings
	{
		#region Members

		private bool mSineWave;
		private float mSineTime;
		private float mAmplitude;
		private float mFrequency;
		private float mPhase_X;
		private float mPhase_Y;
		private float mSpeed;
		private float mLFO;
		private float mHFO;

		#endregion

		#region Properties

		/// <summary>
		/// HighFrequency oscillator
		/// </summary>
		public float HFO
		{
			get { return mHFO; }
			set { mHFO = value; }
		}

		/// <summary>
		/// Low frequency oscillator
		/// </summary>
		public float LFO
		{
			get { return mLFO; }
			set { mLFO = value; }
		}

		/// <summary>
		/// The amount by which the time variable is incremented each update(frame)
		/// </summary>
		public float Speed
		{
			get { return mSpeed; }
			set { mSpeed = value; }
		}

		/// <summary>
		/// Represents theta in the function y(t) = A * sin(omega * t + theta)
		/// </summary>
		public float PhaseY
		{
			get { return mPhase_Y; }
			set { mPhase_Y = value; }
		}

		/// <summary>
		/// Represents theta in the function x(t) = A * sin(omega * t + theta)
		/// </summary>
		public float PhaseX
		{
			get { return mPhase_X; }
			set { mPhase_X = value; }
		}

		/// <summary>
		/// Represents omega in the function f(t) = A * sin(omega * t + theta)
		/// </summary>
		public float Frequency
		{
			get { return mFrequency; }
			set { mFrequency = value; }
		}

		/// <summary>
		/// Represents A in the function f(t) = A * sin(omega * t + theta)
		/// </summary>
		public float Amplitude
		{
			get { return mAmplitude; }
			set { mAmplitude = value; }
		}

		/// <summary>
		/// Represents the parameter t in the function f(t) = A * sin(omega * t + theta)
		/// </summary>
		public float Time
		{
			get { return mSineTime; }
			set { mSineTime = value; }
		}

		/// <summary>
		/// True if the sine wave post processing effect is on
		/// </summary>
		public bool On
		{
			get { return mSineWave;}
			set { mSineWave = value;}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public SineSettings()
		{
			mAmplitude = 0.01f;
			mFrequency = 1.00f;
			mPhase_X = 0.0f;
			mPhase_Y = 1.57f;
			mSineTime = 1.0f;
			mSpeed = 1.0f;
			mSineWave = true;

			mLFO = 0;
			mHFO = 0;
		}

		#endregion
	}
}
