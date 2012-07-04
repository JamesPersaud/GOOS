using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace GOOS.JFX.Level
{
    /// <summary>
    /// LevelData holds data used to specify a level's map, attributes, monsters, objects, scripts and anything else belonging to this level.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        #region members

        [ContentSerializer]
        private string m_name;
        [ContentSerializer]
        private string m_filename;
        [ContentSerializer]
        private string m_levelmapfilename;
        [ContentSerializer]
        private Vector2 m_startlocation;
        [ContentSerializer]
        private float m_startorientation;
        private LevelMapData m_levelmap;

        #endregion

        #region Properties

        [ContentSerializerIgnore]
        public LevelMapData LevelMap
        {
            get
            {
                return m_levelmap;
            }
            set
            {
                m_levelmap = value;
               
            }
        }
        
        [ContentSerializerIgnore]
        public string LevelMapFileName
        {
            get
            {
                return this.m_levelmapfilename;
            }
            set
            {
                m_levelmapfilename = value;
            }
        }
        [ContentSerializerIgnore]
        public string FileName
        {
            get
            {
                return this.m_filename;
            }
        }
        [ContentSerializerIgnore]
        public Vector2 StartLocation
        {
            get
            {
                return m_startlocation;
            }
            set
            {
                m_startlocation = value;
            }
        }

        /// <summary>
        /// Get or Set Player Starting Orientation
        /// </summary>
        [ContentSerializerIgnore]
        public float StartOrientation
        {
            get
            {
                return m_startorientation;
            }
            set
            {
                m_startorientation = value;
            }
        }

        /// <summary>
        /// Get Or Set Level Name
        /// </summary>
        [ContentSerializerIgnore]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LevelData()
        {
            m_name = "<None>";
            m_filename = "<None>";
            m_levelmapfilename = "";
            m_startlocation = Vector2.Zero;
            m_startorientation = 0.0f;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Load a level from a file.
        /// </summary>
        /// <param name="filename">The name of the level file to load</param>
        /// <returns></returns>
        public static LevelData Load(string filename)
        {
            try
            {
                LevelData returndata;
                XmlReaderSettings settings = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    returndata = IntermediateSerializer.Deserialize<LevelData>(reader, null);
                }

                returndata.LoadMap();

                return returndata;
            }
            catch(Exception Ex)
            {
                throw new Exception("File " + filename.ToString() + " not found or corrupt");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save the level
        /// </summary>
        /// <param name="filename">Name of the file to save</param>
        public void SaveToFile(string filename, bool savemap)
        {
            if (filename.Length < 1)
            {
                throw new Exception("No filename specified");
            }

            m_filename = filename;

            //Save the level map if specified
            if (m_levelmap != null && m_levelmapfilename.Length >0 && savemap)
                m_levelmap.SaveTofile(m_levelmapfilename);

            //Save the level data
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize<LevelData>(writer, this, null);
            }
        }

        /// <summary>
        /// Load this level's map from a specified file.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadMap(string filename)
        {
            m_levelmapfilename = filename;
            LoadMap();
        }

        /// <summary>
        /// Load this level's current map.
        /// </summary>
        public void LoadMap()
        {
            if (m_levelmapfilename.Length > 0 && m_levelmapfilename != "<none>")
            {
                m_levelmap = LevelMapData.LoadFromFile(m_levelmapfilename);
            }
        }

        #endregion
    }
}
