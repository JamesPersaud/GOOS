using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI
{	
	//Controls

	//Delegate for basic events
	public delegate void GameControlEventHandler(IGameControl sender, GameControlEventArgs args);
	public class GameControlEventArgs { public object args;}
	//Delegate for time dependant events
	public delegate void GameControlTimedEventHandler(IGameControl sender, GameControlTimedEventArgs args);
	public class GameControlTimedEventArgs : GameControlEventArgs 
	{ 
		public float ElapsedTime; public float TotalTime;
		public GameControlTimedEventArgs(float elapsed, float total){ElapsedTime = elapsed; TotalTime = total;}
	}	
	//Delegate for render events
	public delegate void GameControlRenderEventHandler(IGameControl sender, GameControlRenderEventArgs args);
	public class GameControlRenderEventArgs : GameControlTimedEventArgs
	{
		public SpriteBatch spriteBatch;
		public GameControlRenderEventArgs(float elapsed, float total,SpriteBatch sprite): base(elapsed,total)
		{ spriteBatch = sprite; }
	}
}