using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOOS.JFX.Entity;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace GOOS.JFX.Level
{
	/// <summary>
	/// This class is purely a container for entities belonging to a level to facilitate their loading/saving 
	/// to and from Stage Manager
	/// </summary>
	class LevelEntityData
	{
		#region Properties

		[ContentSerializer]
		public List<Actor> ActorList;

		#endregion

		#region Factory

		/// <summary>
		/// Load Level Entity Data from a file.
		/// </summary>
		/// <param name="filename">The name of the file to load</param>
		/// <returns>a new LevelEntityData object as specified by the file.</returns>
		public static LevelEntityData Load(string filename)
		{
			try
			{
				LevelEntityData returndata;
				XmlReaderSettings settings = new XmlReaderSettings();

				using (XmlReader reader = XmlReader.Create(filename, settings))
				{
					returndata = IntermediateSerializer.Deserialize<LevelEntityData>(reader, null);
				}			

				return returndata;
			}
			catch (Exception Ex)
			{
				throw new Exception("File " + filename.ToString() + " not found or corrupt");
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default Empty Constructor
		/// </summary>
		public LevelEntityData()
		{
			ActorList = new List<Actor>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Save the Collection
		/// </summary>
		/// <param name="filename">Name of the file to save</param>
		public void SaveToFile(string filename)
		{
			if (filename.Length < 1)
			{
				throw new Exception("No filename specified");
			}				

			//Save the collection data
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(filename, settings))
			{
				IntermediateSerializer.Serialize<LevelEntityData>(writer, this, null);
			}
		}

		#endregion
	}
}
