using System;
using System.Collections.Generic;
using LuaInterface;

namespace GOOS.JFX.Scripting
{
	/// <summary>
	/// Privides the LUA scripting engine to the Stage Maanger and Director.
	/// Essentially, it's a wrapper for LuaInterface.
	/// </summary>
	public class LUAScriptWriter
	{
		#region Members

		private Lua mLUAInterface;
		private List<string> mAssemblies;
		private List<string> mTypes;
		private List<string> mFunctions;

		#endregion

		#region Properties

		/// <summary>
		/// Provide direct access to the interface
		/// </summary>
		public LuaInterface.Lua Interface
		{
			get
			{
				return this.mLUAInterface;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Empty private constructor.
		/// </summary>
		private LUAScriptWriter()
		{
			Init();
		}

		#endregion

		#region Factory

		/// <summary>
		/// Create a new LUA scripting interface
		/// </summary>
		/// <returns>A new lua object</returns>
		public static LUAScriptWriter CreateNew()
		{
			return new LUAScriptWriter();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Do a script from a string
		/// </summary>
		/// <param name="script">The script to run</param>
		public void RunScriptFromString(string script)
		{
			Interface.DoString(script);
		}

		/// <summary>
		/// Given an instance of a scriptable class, ensure that class is understood by LUA
		/// </summary>
		/// <param name="obj">An IScriptable object</param>
		public void LoadAndImportFromInstance(IScriptable obj, out string errors)
		{
			errors = string.Empty;

			//Load the assembly
			try { LoadAssembly(obj.AssemblyName); }
			catch (Exception ex) { errors += ex.Message + " "; }
			//Import the Type
			try { ImportType(obj.ClassName); }
			catch (Exception ex) { errors += ex.Message + " "; }

			//Register Functions
			Stack<KeyValuePair<string, string>> functions = obj.ScriptingFunctions;
			KeyValuePair<string, string> kvp;
			while (functions.Count > 0)
			{
				kvp = functions.Pop();
				RegisterFunction(kvp.Key, obj, kvp.Value, true);
			}

			//Run initial script
			RunScriptFromString(obj.InitialScript);
		}

		/// <summary>
		/// Import a CLR type into LUA
		/// </summary>
		/// <param name="FullTypeName">The full name of the type including namespace</param>		
		public void ImportType(string FullTypeName)
		{
			object[] obj;
			ImportType(FullTypeName, out obj);
		}

		/// <summary>
		/// Import a CLR type into LUA
		/// </summary>
		/// <param name="FullTypeName">The full name of the type including namespace</param>
		/// <param name="output">LUA output buffer</param>
		public void ImportType(string FullTypeName, out object[] output)
		{
			if (mTypes.Contains(FullTypeName))
			{
				throw new Exception("LUA already has the type " + FullTypeName);
			}
			else
			{
				try
				{
					output = Interface.DoString("luanet.import_type(\"" + FullTypeName + "\")");
				}
				catch
				{
					output = null;
					throw new Exception("LUA could not import type " + FullTypeName);
				}
			}
		}

		/// <summary>
		/// Load a CLR Assembly into LUA
		/// </summary>
		/// <param name="FullAssemblyName">The full name of the assembly</param>		
		public void LoadAssembly(string FullAssemblyName)
		{
			object[] obj;
			LoadAssembly(FullAssemblyName, out obj);
		}

		/// <summary>
		/// Load a CLR Assembly into LUA
		/// </summary>
		/// <param name="FullAssemblyName">The full name of the assembly</param>
		/// <param name="output">LUA output buffer</param>
		public void LoadAssembly(string FullAssemblyName, out object[] output)
		{
			if (mAssemblies.Contains(FullAssemblyName))
			{
				throw new Exception("LUA already has the assembly " + FullAssemblyName);
			}
			else
			{
				try
				{
					output = Interface.DoString("luanet.load_assembly(\"" + FullAssemblyName + "\")");
					mAssemblies.Add(FullAssemblyName);
				}
				catch
				{
					output = null;
					throw new Exception("LUA could not load assembly " + FullAssemblyName);
				}
			}
		}

		/// <summary>
		/// Register a CLR method as a LUA function
		/// </summary>
		/// <param name="FunctionName">The name of the LUA function</param>
		/// <param name="Target">The target object of the CLR method</param>
		/// <param name="MethodName">The name of the CLR method</param>
		/// <param name="overwrite">Ok to overwrite if this function already exists.</param>	
		public void RegisterFunction(string FunctionName, object Target, string MethodName, bool overwrite)
		{
			if (!overwrite && mFunctions.Contains(FunctionName))
			{
				throw new Exception("LUA already has the function " + FunctionName);
			}
			else
			{
				try
				{
					Interface.RegisterFunction(FunctionName, Target, Target.GetType().GetMethod(MethodName));
					mFunctions.Add(FunctionName);
				}
				catch
				{
					throw new Exception("LUA could not register method " + MethodName + " as function " + FunctionName);
				}
			}
		}

		/// <summary>
		/// Initialise the Interface and wrapper
		/// </summary>
		private void Init()
		{
			// Instantiate the Interface
			mLUAInterface = new Lua();
			mFunctions = new List<string>();
			mTypes = new List<string>();
			mAssemblies = new List<string>();
		}

		#endregion
	}
}
