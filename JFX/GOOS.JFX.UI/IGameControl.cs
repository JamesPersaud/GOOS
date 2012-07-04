using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.UI
{	
	/// <summary>
	/// All UI Controls must implement this interface
	/// </summary>
	public interface IGameControl
	{
		//General
		string Name { get; set; }				// Name of this instance of the ICGameControl
		Vector3 Location { get; set; }			// Location of the control relative to its parent.
		bool Visible { get; set; }				// True if control should be rendered.
		bool Focus { get; set; }				// True if this control is focused.
		bool Enabled { get; set; }				// True if control is Enabled.

		//UI Component
		Point RelativeMouseCoords { get;}
		//Forms
		IGameForm ParentForm { get; set; }		//A reference to the parent form of this Control

		//Rendering
		Texture2D DefaultSkin { get; set; }		//Default texture to render.
		Dictionary<string, Texture2D> AltSkins { get; set; } // alternative textures.
		Texture2D CurrentSkin { get; set; }		// Current texture of the control.
		Rectangle RenderRectangle { get; set; } // Area of the current texture to render.
		Dictionary<string, Rectangle> SourceRectangles { get; set; } // Source rectangle collection.
		Vector4 RenderColour { get; set; }		// colour to render.
		Vector3 GetAbsoluteLocation();

		//Focus Methods
		Rectangle FocusRectangle { get; set; }

		//Content
		ContentManager Content { get; set; }

		//Timing
		float ElapsedTime { get; set; }						// The elapsed game time between now and the last update.
		float TotalTime { get; set; }						// The total elapsed game time at the tome of the last update.

		//Shader
		Effect Shader { get; set; }							//The effect with which to render the Game Control

		//Events
		void RaiseEvent(BaseControlEvents e, IGameControl sender, GameControlEventArgs args);
		event GameControlTimedEventHandler Init;	//These are more or less self explanatory - based on Win Forms.
		event GameControlTimedEventHandler Update;	
		event GameControlRenderEventHandler Render;
		event GameControlEventHandler ChangeFocus;
		event GameControlEventHandler Click;
		event GameControlEventHandler MouseDown;
		event GameControlEventHandler MouseUp;
		event GameControlEventHandler MouseHeld;
	}
}
