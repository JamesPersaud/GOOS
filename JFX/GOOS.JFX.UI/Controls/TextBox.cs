using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// A very basic textbox
	/// </summary>
	public class TextBox :BaseGameControl
	{
		#region Members

		private string mText;
		private SpriteFont mFont;
		private bool blink;
		private float lastblinktime;

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
		protected TextBox() : base()
		{
			blink = false;
			lastblinktime = 0;			
		}		

		#endregion

		#region Factory

		/// <summary>
		/// Create a new textbox control
		/// </summary>
		/// <param name="name">The name of the control</param>
		/// <param name="text">The text to display</param>
		/// <param name="colour">The text colour to display</param>
		/// <param name="font">The font to use</param>
		/// <returns>A new textbox control</returns>
		public static TextBox CreateNew(string name, string text, Vector4 colour, SpriteFont font)
		{
			TextBox tb = new TextBox();
			tb.Text = text;
			tb.Font = font;
			tb.RenderColour = colour;
			tb.Name = name;
			tb.Visible = true;
			tb.Enabled = true;

			return tb;
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
			
			//Draw Cursor]
			if(ParentForm.UI.ControlWithInputFocus != null && ParentForm.UI.ControlWithInputFocus == this)
				if(blink)
					args.spriteBatch.DrawString(Font, "|",new Vector2(screenloc.X + Font.MeasureString(Text).X,screenloc.Y), c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
		}

		protected override void BaseGameControl_Update(IGameControl sender, GameControlTimedEventArgs args)
		{
			float interval = args.TotalTime - lastblinktime;
			if (interval > 500)
			{
				lastblinktime = args.TotalTime;
				blink = !blink;
			}

			base.BaseGameControl_Update(sender, args);
		}

		#endregion				
	}
}
