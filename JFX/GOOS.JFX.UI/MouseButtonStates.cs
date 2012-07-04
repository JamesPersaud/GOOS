using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// Records an instance of mouse button status.
	/// </summary>
	public struct MouseButtonStates
	{
		public ButtonState Left;
		public ButtonState Right;
		public ButtonState Middle;
		public int Time;
		public string ControlName;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="s">The mouse state</param>
		/// <param name="t">The point in time</param>
		public MouseButtonStates(MouseState s, int t,string controlName)
		{
			Left = s.LeftButton;
			Right = s.RightButton;
			Middle = s.MiddleButton;
			Time = t;
			ControlName = controlName;
		}
	}
}
