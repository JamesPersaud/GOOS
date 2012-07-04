using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// A GameForm object might be a root level object (with no rendered component) or a window.
	/// Either way it collects GameControls
	/// </summary>
	public interface IGameForm : IGameControl
	{
		//General		
		bool Root { get; set; }								//Is this the root level form?		

		//Controls
		Dictionary<string,IGameControl> Controls {get;set;} //A collection of controls belonging to the form. The key corresponds to the name of the control.
		void AddControl(IGameControl control);				//Add a control
		void RemoveControl(string name);					//remove a control
		IGameControl GetControl(string name);				//Get a control by name

		//Child Forms
		Dictionary<string, IGameForm> ChildForms { get; set; } // A collection of child forms.			
		void AddForm(IGameForm form);					//Add a form
		void RemoveForm(string name);						//remove a form
		IGameForm GetForm(string name);						//Get a form by name	
	
		//Root forms have a handle to the UI component.
		UIComponent UI { get; set; }
	}
}
