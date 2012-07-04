using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// Represents a slider control.
	/// 
	/// Usage:
	/// 
	/// The groove rectangle should have the same height as the slider
	/// 
	/// </summary>
	public class Slider : BaseGameControl
	{
		#region Members
	
		private float mMaxValue;
		private float mMinValue;
		private float mStep;
		private float mValue;
		private SliderAlignment mAlignment;
		private SliderStepMode mStepMode;

		#endregion

		#region Properties

		/// <summary>
		/// Get or Set the step mode of the slider.
		/// </summary>
		public SliderStepMode StepMode
		{
			get { return mStepMode; }
			set { mStepMode = value; }
		}

		/// <summary>
		/// Get or Set the alignment of the slider.
		/// </summary>
		public SliderAlignment Alignment
		{
			get { return mAlignment; }
			set { mAlignment = value; }
		}

		/// <summary>
		/// The current value held by the slider.
		/// </summary>
		public float Value
		{
			get { return mValue; }
			set { mValue = value; }
		}

		/// <summary>
		/// The step resolution of the slider bar.
		/// </summary>
		public float Step
		{
			get { return mStep; }
			set { mStep = value; }
		}

		/// <summary>
		/// The Minimum value
		/// </summary>
		public float MinValue
		{
			get { return mMinValue; }
			set { mMinValue = value; }
		}

		/// <summary>
		/// The Maximum value 
		/// </summary>
		public float MaxValue
		{
			get { return mMaxValue; }
			set { mMaxValue = value; }
		}

		/// <summary>
		/// The source rectangle to render the bar.
		/// </summary>
		public Rectangle BarSource
		{
			get { return SourceRectangles["bar"]; }
			set { SourceRectangles["bar"] = value; }
		}

		/// <summary>
		/// The source rectangle to render the groove.
		/// </summary>
		public Rectangle GrooveSource
		{
			get { return SourceRectangles["groove"]; }
			set { SourceRectangles["groove"] = value; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		private Slider() : base()
		{
			MouseHeld += new GameControlEventHandler(this.Slider_MouseHeld);
			MouseDown += new GameControlEventHandler(this.Slider_MouseHeld);
		}			

		#endregion

		#region Factory

		/// <summary>
		/// Create a new slider control
		/// </summary>
		/// <param name="name">The name of the control</param>
		/// <param name="texture">The texture for rendering</param>
		/// <param name="grooverect">The rectangle to specify the render area of the groove</param>
		/// <param name="barrect">The rectangle to specify the render area of the bar</param>
		/// <param name="min">The minimum value of the slider</param>
		/// <param name="max">The maxumum value of the slider</param>
		/// <param name="step">The step or resolution of the slider values.</param>
		/// <param name="align">The alignment of the slider values.</param>
		/// <param name="stepmode">The step mode of the slider values.</param>
		/// <returns>A new slider control object</returns>
		public static Slider CreateNew(string name, Texture2D texture, Rectangle grooverect, Rectangle barrect, float min, float max, float step, SliderAlignment align, SliderStepMode stepmode)
		{
			Slider s = new Slider();
			s.Alignment = align;
			s.BarSource = barrect;
			s.CurrentSkin = texture;
			s.DefaultSkin = texture;
			s.GrooveSource = grooverect;
			s.MaxValue = max;
			s.MinValue = min;
			s.Name = name;
			s.RenderColour = Vector4.One;
			s.Step = step;
			s.StepMode = stepmode;
			s.Enabled = true;

			return s;
		}

		#endregion

		#region Event Handler Overrides

		/// <summary>
		/// This control completely overrides the base render functionality (it has two textures to draw)
		/// </summary>
		protected override void BaseGameControl_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			//The basic render for a control is just to draw the skin texture at its location (relative to the form location
			Vector3 absolute = GetAbsoluteLocation();
			Vector2 screenloc = new Vector2(absolute.X, absolute.Y);
			float depth = absolute.Z;
			Color c = new Color(RenderColour);

			//Draw Groove
			args.spriteBatch.Draw(CurrentSkin, screenloc, GrooveSource, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			FocusRectangle = new Rectangle((int)screenloc.X, (int)screenloc.Y, GrooveSource.Width, GrooveSource.Height);

			//Draw Bar
			//express the current value as a fraction
			float fraction = (Value - MinValue) / (MaxValue - MinValue);
			int barOffsetX=0, baroffsetY=0;

			//If the value is the highest it can be without stepping over the max it should be defaulted to 100
			if (Value + Step > MaxValue) fraction = 1.0f;

			if (this.Alignment == SliderAlignment.Horizontal)
			{
				barOffsetX = (int)((float)GrooveSource.Width * fraction) - (int)(BarSource.Width/2);
				baroffsetY = 0;
			}
			else if (this.Alignment == SliderAlignment.Vertical)
			{
				int barOffsetY = (int)((float)GrooveSource.Height * fraction) - (int)(BarSource.Height/2);
				barOffsetX = 0;
			}
			
			//Draw bar
			args.spriteBatch.Draw(CurrentSkin, new Vector2(screenloc.X+(float)barOffsetX,screenloc.Y + (float)baroffsetY), BarSource, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth-0.1f);
		}

		#endregion

		#region Add Event Handlers

		/// <summary>
		/// The slidebar should move (and the selected value change) on mousedown
		/// </summary>
		protected void Slider_MouseHeld(IGameControl sender, GameControlEventArgs args)
		{			
			//Get mouse position
			Point mousepos = ParentForm.UI.CurrentMousePointer.PickedScreenCoordinate;
			int relativeX = mousepos.X - (int)GetAbsoluteLocation().X;
			int relativeY = mousepos.Y - (int)GetAbsoluteLocation().Y;
			float fraction = 0.0f;

			if (Alignment == SliderAlignment.Horizontal)			
				fraction = (float)relativeX / (float)GrooveSource.Width;
			else if (Alignment == SliderAlignment.Vertical)
				fraction = (float)relativeY / (float)GrooveSource.Height;

			float rawvalue = (fraction * (MaxValue - MinValue)) + MinValue;
			if (StepMode == SliderStepMode.Smooth)
				Value = rawvalue;
			else if (StepMode == SliderStepMode.Snap) // If in snap mode, snap to nearest value.
			{
				float mod = rawvalue % Step;
				if (mod >= Step / 2)
					Value = rawvalue + Step - mod;
				else
					Value = rawvalue - mod;
			}

			if (Value < MinValue)
				Value = MinValue;
			if (Value > MaxValue)
				Value = MaxValue;
		}	

		#endregion
	}
}
