using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GOOS.JFX.Game;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// A control to display a basic high score table.
	/// </summary>
	public class HiScoreTableControl :BaseGameControl
	{
		#region Members

		private SpriteFont mFont;
		private Vector4 mLowColour;
		private Vector4 mHighColour;
		private HiScoreTable mTable;
		private int mNumberOfScores;	

		#endregion

		/// <summary>
		/// The number of scores to display.
		/// </summary>
		public int NumberOfScores
		{
			get { return mNumberOfScores; }
			set { mNumberOfScores = value; }
		}

		/// <summary>
		/// The high score table object holding the score info.
		/// </summary>
		public HiScoreTable Table
		{
			get { return mTable; }
			set { mTable = value; }
		}

		/// <summary>
		/// The colour for the highest score on the table.
		/// </summary>
		public Vector4 HighColour
		{
			get { return mHighColour; }
			set { mHighColour = value; }
		}

		/// <summary>
		/// The colour for the lowest score on the table.
		/// </summary>
		public Vector4 LowColour
		{
			get { return mLowColour; }
			set { mLowColour = value; }
		}

		/// <summary>
		/// Get or Set the font to use.
		/// </summary>
		public SpriteFont Font
		{
			get { return mFont; }
			set { mFont = value; }
		}		

		#region Properties

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public HiScoreTableControl() : base()
		{

		}

		#endregion

		#region Factory

		/// <summary>
		/// Create a new high schore table control.
		/// </summary>
		/// <param name="name">The name of the control</param>
		/// <param name="font">The font to use to render the scores.</param>
		/// <param name="low">The colour in which to render the low score.</param>
		/// <param name="high">The colour in which to render the high score.</param>
		/// <param name="table">The table to reference.</param>
		/// <param name="numberofscores">The number of scores to render.</param>
		/// <returns>A new hiscore control object</returns>
		public static HiScoreTableControl CreateNew(string name, SpriteFont font, Vector4 low, Vector4 high, HiScoreTable table, int numberofscores)
		{
			HiScoreTableControl c = new HiScoreTableControl();
			c.Name = name;
			c.Font = font;
			c.LowColour = low;
			c.HighColour = high;
			c.Table = table;
			c.NumberOfScores = numberofscores;
			return c;
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
			Color lowCol = new Color(LowColour);
			Color highCol = new Color(HighColour);
			//Get Lineheight
			Vector2 fontdims = Font.MeasureString("A");

			float indent_name, indent_level, indent_time;

			indent_name = Font.MeasureString("ThisIsALongNameIndeed").X;
			indent_level = indent_name + Font.MeasureString("1000---").X;
			indent_time = indent_level + Font.MeasureString("1000000").X;

			List<ScoreInfo> scores = Table.GetTop(NumberOfScores);
			Stack<Vector4> colours = new Stack<Vector4>();
			float red, green, blue;
			float increment = 1.0f / (float)NumberOfScores;

			for (int i = 0; i < NumberOfScores; i++)
			{
				//Linear interpolation between low and high colour vectors.
				//Build stack from low to high then render from high to low.
				red = LowColour.X + ((HighColour.X - LowColour.X) * (increment * i));
				green = LowColour.Y + ((HighColour.Y - LowColour.Y) * (increment * i));
				blue = LowColour.Z + ((HighColour.Z - LowColour.Z) * (increment * i));
				//Add the colour to the stack
				colours.Push(new Vector4(red, green, blue, 1.0f));
			}

			//Need to render column headers.
			args.spriteBatch.DrawString(Font, "Name", new Vector2(screenloc.X, screenloc.Y ), highCol, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			args.spriteBatch.DrawString(Font, "Level", new Vector2(screenloc.X + indent_name, screenloc.Y ), highCol, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			args.spriteBatch.DrawString(Font, "Time", new Vector2(screenloc.X + indent_level, screenloc.Y ), highCol, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			args.spriteBatch.DrawString(Font, "Score", new Vector2(screenloc.X + indent_time, screenloc.Y ), highCol, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);

			//column data
			for (int loop = 0; loop < scores.Count; loop++)
			{
				Vector4 cv = colours.Pop();
				args.spriteBatch.DrawString(Font, scores[loop].ExtendedValues["name"].ToString(), new Vector2(screenloc.X, screenloc.Y + fontdims.Y * (loop+2)), new Color(cv), 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				args.spriteBatch.DrawString(Font, scores[loop].ExtendedValues["level"].ToString(), new Vector2(screenloc.X + indent_name, screenloc.Y + fontdims.Y * (loop+2)), new Color(cv), 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				args.spriteBatch.DrawString(Font, scores[loop].ExtendedValues["time"].ToString(), new Vector2(screenloc.X + indent_level, screenloc.Y + fontdims.Y * (loop+2)), new Color(cv), 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
				args.spriteBatch.DrawString(Font, scores[loop].ExtendedValues["score"].ToString(), new Vector2(screenloc.X + indent_time, screenloc.Y + fontdims.Y * (loop+2)), new Color(cv), 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			}
		}

		#endregion
	}
}
