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
	public class Label : BaseGameControl
	{
		#region Members

		private string mText;
		private SpriteFont mFont;

		#endregion

		/// <summary>
		/// Get or Set the font to use.
		/// </summary>
		public SpriteFont Font
		{
			get { return mFont; }
			set { mFont = value; }
		}

		/// <summary>
		/// Get or Set the text to display.
		/// </summary>
		public string Text
		{
			get { return mText; }
			set { mText = value; }
		}

		#region Properties

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		protected Label() : base()
		{
		}

		#endregion

		#region Factory

		/// <summary>
		/// Create a new label control
		/// </summary>
		/// <param name="name">The name of the control</param>
		/// <param name="text">The text to display</param>
		/// <param name="colour">The text colour to display</param>
		/// <param name="font">The font to use</param>
		/// <returns>A new label control</returns>
		public static Label CreateNew(string name, string text, Vector4 colour, SpriteFont font)
		{			
			Label l = new Label();
			l.Text = text;
			l.Font = font;
			l.RenderColour = colour;
			l.Name = name;
			l.Visible = true;
			l.Enabled = false; //By default labels don't get focus.

			return l;
		}

		#endregion

		#region Event Handler Overrides

		/// <summary>
		/// Do this instead of the usual render method
		/// </summary>
		protected override void BaseGameControl_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			//The basic render for a control is just to draw the skin texture at its location (relative to the form location
			Vector3 absolute = GetAbsoluteLocation();
			Vector2 screenloc = new Vector2(absolute.X, absolute.Y);
			float depth = absolute.Z;
			Color c = new Color(RenderColour * 255);

			//Draw Text
			args.spriteBatch.DrawString(Font, Text, screenloc, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
		}

		#endregion
	}
}
