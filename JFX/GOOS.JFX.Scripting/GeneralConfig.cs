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
	public class GeneralConfig : IScriptable
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
		private int m_height;
		[ContentSerializer]
		private int m_width;
		[ContentSerializer]
		private string m_defaultlevel;
		[ContentSerializer]
		private float m_wallspecular_power;
		[ContentSerializer]
		private float m_wallspecular_int;
		[ContentSerializer]
		private float m_ambient;
		[ContentSerializer]
		private float m_torchrange;
		[ContentSerializer]
		private float m_torchattenuation;
		[ContentSerializer]
		private bool m_fullscreen;
		//[ContentSerializer]
		//private Dictionary<string, bool> m_ExtraBoolSettings;
		//[ContentSerializer]
		//private Dictionary<string, float> m_ExtraFloatSettings;

		#endregion

		#region Properties
		//[ContentSerializerIgnore]
		//public Dictionary<string, float> ExtraFloatSettings
		//{
		//    get { return m_ExtraFloatSettings; }
		//    set { m_ExtraFloatSettings = value; }
		//}
		//[ContentSerializerIgnore]
		//public Dictionary<string, bool> ExtraBoolSettings
		//{
		//    get { return m_ExtraBoolSettings; }
		//    set { m_ExtraBoolSettings = value; }
		//}

		[ContentSerializerIgnore]
		public bool Fullscreen
		{
			get
			{
				return m_fullscreen;
			}
			set
			{
				m_fullscreen = value;
			}
		}

		[ContentSerializerIgnore]
		public float TorchAttenuation
		{
			get
			{
				return m_torchattenuation;
			}
			set
			{
				m_torchattenuation = value;
			}
		}

		[ContentSerializerIgnore]
		public float TorchRange
		{
			get
			{
				return m_torchrange;
			}
			set
			{
				m_torchrange = value;
			}
		}

		[ContentSerializerIgnore]
		public float Ambient
		{
			get
			{
				return m_ambient;
			}
			set
			{
				m_ambient = value;
			}
		}

		[ContentSerializerIgnore]
		public float WallSpecularIntensity
		{
			get
			{
				return m_wallspecular_int;
			}
			set
			{
				m_wallspecular_int = value;
			}
		}

		[ContentSerializerIgnore]
		public float WallSpecularPower
		{
			get
			{
				return m_wallspecular_power;
			}
			set
			{
				m_wallspecular_power = value;
			}
		}

		[ContentSerializerIgnore]
		public string DefaultLevel
		{
			get
			{
				return m_defaultlevel;
			}
			set
			{
				m_defaultlevel = value;
			}
		}

		[ContentSerializerIgnore]
		public int width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		[ContentSerializerIgnore]
		public int Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		#endregion

		#region Constructors

		public GeneralConfig()
		{
		}

		#endregion

		#region Factory

		public static GeneralConfig LoadFromFile(string filename)
		{
			try
			{
				GeneralConfig returndata;
				XmlReaderSettings settings = new XmlReaderSettings();

				using (XmlReader reader = XmlReader.Create(filename, settings))
				{
					returndata = IntermediateSerializer.Deserialize<GeneralConfig>(reader, null);
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
		public GeneralConfig GetConfigSettings()
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
				IntermediateSerializer.Serialize<GeneralConfig>(writer, this, null);
			}
		}

		#endregion
	}
}
