using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// Static methods to handle keyboard input
	/// </summary>
	public class KeyPressInterpreter
	{
		/// <summary>
		/// Interprets a single key press as a string
		/// </summary>
		/// <param name="k">The key pressed</param>
		/// <param name="board">The keyboard state</param>
		/// <returns>A string</returns>
		public static string KeyToString(Keys k, KeyboardState board)
		{			
			bool alt = (board.IsKeyDown(Keys.LeftAlt) || board.IsKeyDown(Keys.RightAlt));
			bool ctrl = (board.IsKeyDown(Keys.LeftControl) || board.IsKeyDown(Keys.RightControl));						

			//alt ctrl
			if (alt || ctrl)
				return "";

			//spacebar
			if (k == Keys.Space)
				return " ";
			
			bool shift = (board.IsKeyDown(Keys.LeftShift) || board.IsKeyDown(Keys.RightShift));
			string keystring = k.ToString();

			//numbers
			string[] numbers = { "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D0" };
			if (numbers.Contains(keystring.ToUpper()))
				if (!shift)
					return keystring.Substring(1, 1);
				else
				{
					switch (k)
					{
						case Keys.D1: return "!";
						case Keys.D2: return "\"";
						case Keys.D3: return "£";
						case Keys.D4: return "$";
						case Keys.D5: return "%";
						case Keys.D6: return "^";
						case Keys.D7: return "&";
						case Keys.D8: return "*";
						case Keys.D9: return "(";
						case Keys.D0: return ")";
					}
				}

			//letters
			string[] letters = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
			if (letters.Contains(keystring.ToUpper()))
				if (shift)
					return keystring.ToUpper();
				else
					return keystring.ToLower();	
		
			//nonalphanumeric
			switch(k)
			{
				case Keys.OemComma: return ",";
				case Keys.OemPeriod: return ".";
				case Keys.OemSemicolon: if (shift) return ":"; else return ";";
			}

			return "";
		}
	}
}
