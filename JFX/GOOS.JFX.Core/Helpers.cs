using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.Core
{
	public class Helpers
	{	
		public static Random _r = new Random();

		/// <summary>
		/// Generate a new random int
		/// </summary>	
		/// <returns></returns>
		public static int RandomInt(int min, int max)
		{
			return _r.Next(min, max);
		}
	}
}
