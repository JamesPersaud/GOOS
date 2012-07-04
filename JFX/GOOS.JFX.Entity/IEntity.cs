using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Entity
{
	#region Event Handlers and args defs

	/// <summary>
	/// Entity Event Arguments struct
	/// </summary>
	public struct EntityEventArgs 
	{
		float ImplementMe;
	}

	//Delegate for Entities
	public delegate void EntityEventHandler(IEntity sender, EntityEventArgs args);

	#endregion

	/// <summary>
	/// Provides an interface through which the stage manager can interact with actors/props/scenery/etc
	/// </summary>
	public interface IEntity
	{
		//Grab the entity by one of these handles
		string TypeName { get; set; }
		string InstanceName { get; set; }
		string InstanceID { get; set; } // GUID     A unique id created when the actor is instantiated.
		string AppearanceID { get; set; } //GUID    A unique id re-created when the actor enters the stage.

		//All entities should have scripts of some kind - event scripts with names (dictionary keys) identical
		//to names of events will execute when that event is raised
		Dictionary<string, string> EventScripts {get;set;}

		//Events - hook up a method in whatever manages these entities (i.e. StageManager) to handle this event.
		event EntityEventHandler Init;		
	}
}
