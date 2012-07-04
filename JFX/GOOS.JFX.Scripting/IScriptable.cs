using System;
using System.Collections.Generic;

namespace GOOS.JFX.Scripting
{
	/// <summary>
	/// Scriptable classes implement this interface.
	/// </summary>
	public interface IScriptable
	{
		/// <summary>
		/// Must be the full name of the assembly the class sits in.
		/// </summary>
		string AssemblyName { get; }
		/// <summary>
		/// Must be the full name of the class itself including namespace.
		/// </summary>
		string ClassName { get; }
		/// <summary>
		/// Collect the names and method names of the functions to register
		/// this could be done by attributes but this method should
		/// be faster than using reflection (twice).
		/// 
		/// Order MUST be 
		/// 
		/// key=Scripting FunctionName 
		/// value=CLR MethodName 
		/// 
		/// </summary>
		Stack<KeyValuePair<string, string>> ScriptingFunctions { get; }
		/// <summary>
		/// The script that should be run upon this instance being imported
		/// by the script writer.
		/// </summary>
		string InitialScript { get; set; }
	}
}
