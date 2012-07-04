using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace GOOS.JFX.Scripting
{
	/// <summary>
	/// This class holds general engine configuration settings info
	/// 
	/// TODO: enable scripting
	/// 
	/// </summary>
	public class ExtraConfig : IScriptable
	{
		#region IScriptable Members

		[ContentSerializerIgnore]
		public string AssemblyName
		{
			get { return this.GetType().Namespace; }
		}

		[ContentSerializerIgnore]
		public string ClassName
		{
			get { return this.GetType().Namespace + "." + this.GetType().Name; }
		}

		[ContentSerializerIgnore]
		public Stack<KeyValuePair<string, string>> ScriptingFunctions
		{
			get
			{
				Stack<KeyValuePair<string, string>> s = new Stack<KeyValuePair<string, string>>();

				s.Push(new KeyValuePair<string, string>("GetConfigSettings", "GetConfigSettings"));

				return s;
			}
		}

		[ContentSerializerIgnore]
		public string InitialScript
		{
			get
			{
				string LUAscript = string.Empty;
				LUAscript += "Config = GetConfigSettings() ";
				return LUAscript;
			}
			set
			{
				throw new NotImplementedException("Set not implemenmted on this scriptable's initial script.");
			}
		}

		#endregion

		#region Members

		[ContentSerializer]
		private Dictionary<string, bool> m_ExtraBoolSettings;
		[ContentSerializer]
		private Dictionary<string, float> m_ExtraFloatSettings;

		#endregion

		#region Properties
		[ContentSerializerIgnore]
		public Dictionary<string, float> ExtraFloatSettings
		{
			get { return m_ExtraFloatSettings; }
			set { m_ExtraFloatSettings = value; }
		}
		[ContentSerializerIgnore]
		public Dictionary<string, bool> ExtraBoolSettings
		{
			get { return m_ExtraBoolSettings; }
			set { m_ExtraBoolSettings = value; }
		}	

		#endregion

		#region Constructors

		public ExtraConfig()
		{
		}

		#endregion

		#region Factory

		public static ExtraConfig LoadFromFile(string filename)
		{
			try
			{
				ExtraConfig returndata;
				XmlReaderSettings settings = new XmlReaderSettings();

				using (XmlReader reader = XmlReader.Create(filename, settings))
				{
					returndata = IntermediateSerializer.Deserialize<ExtraConfig>(reader, null);
				}

				return returndata;
			}
			catch (Exception ex)
			{
				string e = ex.Message;
				throw new Exception("File not found or corrupt");
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Scripting access to this object
		/// </summary>
		/// <returns>this instance</returns>
		public ExtraConfig GetConfigSettings()
		{
			return this;
		}

		public void SaveTofile(string filename)
		{
			if (filename.Length < 1)
			{
				throw new Exception("No filename specified");
			}

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(filename, settings))
			{
				IntermediateSerializer.Serialize<ExtraConfig>(writer, this, null);
			}
		}

		#endregion
	}
}
