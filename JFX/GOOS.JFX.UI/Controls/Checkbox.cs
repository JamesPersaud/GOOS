using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// Represents a checkbox control.
	/// </summary>
	public class Checkbox : BaseGameControl
	{
		#region Members		

		private SpriteFont mFont;
		private CheckboxAlignment mAlignment;
		private string mText;
		private int mWidth;
		private bool mChecked;
		private Vector4 mTextRenderColour;
		private int mPadding;

		#endregion

		#region Properties

		/// <summary>
		/// Padding to add between box and label.
		/// </summary>
		public int Padding
		{
			get { return mPadding; }
			set { mPadding = value; }
		}

		/// <summary>
		/// The colour to render the text.
		/// </summary>
		public Vector4 TextRenderColour
		{
			get { return mTextRenderColour; }
			set { mTextRenderColour = value; }
		}

		/// <summary>
		/// The font to use when drawing the label.
		/// </summary>
		public SpriteFont Font
		{
			get { return mFont; }
			set { mFont = value; }
		}

		/// <summary>
		/// The alignment of the checkbox
		/// </summary>
		public CheckboxAlignment Alignment
		{
			get { return mAlignment; }
			set { mAlignment = value; }
		}

		/// <summary>
		/// The text to display.
		/// </summary>
		public string Text
		{
			get { return mText; }
			set { mText = value; }
		}

		/// <summary>
		/// The total width of the control (including space for label text)
		/// </summary>
		public int Width
		{
			get { return mWidth; }
			set { mWidth = value; }
		}

		/// <summary>
		/// Is this checkbox checked?
		/// </summary>
		public bool Checked
		{
			get { return mChecked; }
			set
			{
				mChecked = value;
				if (value)
					RenderRectangle = SourceRectangles["checked"];
				else
					RenderRectangle = SourceRectangles["unchecked"];
			}
		}

		/// <summary>
		/// Source rectangle to render unckecked checkbox.
		/// </summary>
		public Rectangle UnckeckedSource
		{
			get { return SourceRectangles["unckecked"]; }
			set { SourceRectangles["unckecked"] = value; }
		}

		/// <summary>
		/// Source rectangle to render checked checkbox.
		/// </summary>
		public Rectangle CheckedSource
		{
			get { return SourceRectangles["checked"]; }
			set { SourceRectangles["checked"] = value; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		private Checkbox() : base()
		{
			Click += new GameControlEventHandler(Checkbox_Click);			
		}				

		#endregion

		#region Factory

		/// <summary>
		/// Create a new Check Box
		/// </summary>
		/// <param name="name">The name of the new Checkbox</param>
		/// <param name="location">The location of the new checkbox</param>
		/// <param name="text">The text to display</param>
		/// <param name="alignment">The alignment of the checkbox relative to the label</param>
		/// <param name="skin">The texture to use to render the checkbox</param>
		/// <param name="checkedrectangle">The source rectangle of the checkbox's checked frame</param>
		/// <param name="uncheckedrectangle">The source rectangle of the checkbox's unchecked frame</param>
		/// <param name="width">The overall width of the control (checkbox and label)</param>
		/// <returns>A new checkbox object</returns>
		public static Checkbox CreateNew(string name,
			Vector3 location,
			string text,
			CheckboxAlignment alignment,
			Texture2D skin,
			SpriteFont font,
			Rectangle checkedrectangle,
			Rectangle uncheckedrectangle,
			int width,
			Vector4 rendercolour,
			Vector4 textrendercolour,
			int padding)
		{
			Checkbox cb = new Checkbox();
			cb.Name = name;
			cb.Text = text;
			cb.Alignment = alignment;
			cb.CurrentSkin = skin;
			cb.Font = font;
			cb.SourceRectangles = new Dictionary<string, Rectangle>();
			cb.SourceRectangles.Add("checked", checkedrectangle);
			cb.SourceRectangles.Add("unchecked", uncheckedrectangle);
			cb.Width = width;
			cb.Location = location;
			cb.RenderColour = rendercolour;
			cb.TextRenderColour = textrendercolour;
			cb.Checked = false;
			cb.Padding = padding;
			cb.Visible = true;
			cb.Enabled = true;			
			return cb;
		}

		#endregion

		#region Base Event Overrides

		/// <summary>
		/// Override the base render behaviour.
		/// </summary>
		protected override void BaseGameControl_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			//The basic render for a control is just to draw the skin texture at its location (relative to the form location
			Vector3 absolute = GetAbsoluteLocation();			
			float depth = absolute.Z;
			Color c = new Color(RenderColour);
			Color tc = new Color(TextRenderColour);
			//Get size of text			
			Vector2 stringsize = Font.MeasureString(Text);
			//Get screen loc
			Vector2 screenloc = new Vector2(absolute.X, absolute.Y);

			//Align centre of image with centre of text
			int imagecentre = RenderRectangle.Height / 2;
			int textcentre = (int)stringsize.Y / 2;
			int diff = textcentre - imagecentre;			

			//Draw Checkbox
			if(Alignment == CheckboxAlignment.Left)
			{
				if (Width > 0)
				{
					args.spriteBatch.Draw(CurrentSkin, new Vector2(screenloc.X, screenloc.Y + diff), RenderRectangle, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
					FocusRectangle = new Rectangle((int)screenloc.X, (int)screenloc.Y + diff, RenderRectangle.Width, RenderRectangle.Height);
					args.spriteBatch.DrawString(Font, Text, new Vector2(screenloc.X + Width - (int)stringsize.X, screenloc.Y), tc, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				}
				else
				{
					args.spriteBatch.Draw(CurrentSkin, new Vector2(screenloc.X, screenloc.Y + diff), RenderRectangle, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
					FocusRectangle = new Rectangle((int)screenloc.X, (int)screenloc.Y + diff, RenderRectangle.Width, RenderRectangle.Height);
					args.spriteBatch.DrawString(Font, Text, new Vector2(screenloc.X + RenderRectangle.Width + Padding, screenloc.Y), tc, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				}
			}
			else if(Alignment == CheckboxAlignment.Right)
			{
				if (Width > 0)
				{
					args.spriteBatch.Draw(CurrentSkin, new Vector2(screenloc.X + Width - RenderRectangle.Width, screenloc.Y + diff), RenderRectangle, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
					FocusRectangle = new Rectangle((int)screenloc.X + Width - RenderRectangle.Width, (int)screenloc.Y + diff, RenderRectangle.Width, RenderRectangle.Height);
					args.spriteBatch.DrawString(Font, Text, screenloc, tc, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				}
				else
				{
					args.spriteBatch.Draw(CurrentSkin, new Vector2(screenloc.X + stringsize.X + Padding, screenloc.Y + diff), RenderRectangle, c, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
					FocusRectangle = new Rectangle((int)screenloc.X + (int)stringsize.X + Padding, (int)screenloc.Y + diff, RenderRectangle.Width, RenderRectangle.Height);
					args.spriteBatch.DrawString(Font, Text, screenloc, tc, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				}
			}											
		}

		#endregion

		#region Method overrides		

		#endregion

		#region Event Handlers

		/// <summary>
		/// Extra click functionality.
		/// </summary>
		void Checkbox_Click(IGameControl sender, GameControlEventArgs args)
		{
			Checked = !Checked;
		}

		#endregion
	}
}
