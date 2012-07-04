using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using GOOS.JFX.Core;

namespace GOOS.JFX.Level
{
    /// <summary>
    /// MapData holds data used to draw a level map including wall defining quads and areas of special geometry.
    /// </summary>
    [Serializable]
    public class LevelMapData
    {
        #region members

		[ContentSerializerIgnore]
		public List<BoundingBox> WallCollision;

        [ContentSerializer]
        private string m_name;
        [ContentSerializer]
        private string m_filename;
        [ContentSerializer]
        private int m_width;
        [ContentSerializer]
        private int m_height;
        private LevelMapSquare[,] m_squares; // Cannot serialize multidimentional arrays.
        [ContentSerializer]
        private LevelMapSquare[] m_squares_serialize; // Serialize this instead.
        [ContentSerializer]
        private List<Quad> m_walls;
        [ContentSerializer]
        private List<Quad> m_floors;
        [ContentSerializer]
        private List<Quad> m_ceilings;       	

        #endregion

        #region Properties       

        [ContentSerializerIgnore]
        public List<Quad> Walls
        {
            get
            {
                return m_walls;
            }
        }

        [ContentSerializerIgnore]
        public List<Quad> Ceilings
        {
            get
            {
                return m_ceilings;              
            }
        }

        [ContentSerializerIgnore]
        public List<Quad> Floors
        {
            get
            {
                return m_floors;
            }
        }

        [ContentSerializerIgnore]
        public int Width
        {
            get
            {
                return m_width;
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

        [ContentSerializerIgnore]
        public string FileName
        {
            get
            {
                return m_filename;
            }
            set
            {
                m_filename = value;
            }
        }   

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
        public LevelMapData()
        {                 
            m_walls = new List<Quad>();
            m_floors = new List<Quad>();
            m_ceilings = new List<Quad>();
        }

        /// <summary>
        /// Instantiate a new Mapdata
        /// </summary>
        public LevelMapData(int width, int height):this()
        {
            m_width = width;
            m_height = height;
            m_squares = new LevelMapSquare[width, height];
            m_squares_serialize = new LevelMapSquare[width * height];

            for(int y=0;y<height;y++)            
                for (int x = 0; x < width; x++)
                    m_squares[x, y] = new LevelMapSquare(MapSquareType.Closed);          
        }

        #endregion

        #region Factory

        /// <summary>
        /// Get an empty map
        /// </summary>
        /// <param name="width">width of the grid</param>
        /// <param name="height">height of the grid</param>
        /// <returns></returns>
        public static LevelMapData CreateEmptyMap(int width,int height)
        {
            return new LevelMapData(width,height);
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename">name of the file to load</param>
        /// <returns></returns>
        public static LevelMapData LoadFromFile(string filename)
        {
            try
            {
                LevelMapData returndata;
                XmlReaderSettings settings = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    returndata = IntermediateSerializer.Deserialize<LevelMapData>(reader, null);
                }

                returndata.DeserializeGrid();

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
		/// Get a random 2D map location of a specific type.
		/// </summary>
		/// <param name="t">The type of location (square) to get.</param>
		/// <param name="r">The random number provider (passing null takes a new one.)</param>
		/// <param name="exceptions">A list of exceptions (pass null if there are no exceptions)</param>
		/// <param name="quater">1-4 restricts search to a quater of the map clockwise from top left. Set to any other value for a search of the entire map</param>
		/// <returns>A Vector2 struct holding the 2D map coords of the location.</returns>	
		public Vector2 GetRandomSquareCoordsByType(MapSquareType t, Random r, List<Vector2> exceptions, int quater)
		{
			if (exceptions == null) exceptions = new List<Vector2>();
			if (r == null) r = new Random();

			int min_x,min_y,max_x,max_y;

			switch (quater)
			{
				case 1: min_x = 0; min_y = 0; max_x = this.Width / 2; max_y = this.Height / 2; break; //North West
				case 2: min_x = this.Width / 2; min_y = 0; max_x = this.Width - 1; max_y = this.Height / 2; break; //North East
				case 3: min_x = 0; min_y = this.Height / 2; max_x = this.Width / 2; max_y = this.Height - 1; break; //South West
				case 4: min_x = this.Width / 2; min_y = this.Height / 2; max_x = this.Width - 1; max_y = this.Height - 1; break; //South East
				default: min_x = 0; min_y = 0; max_x = this.Width - 1; max_y = this.Height - 1; break; //Entire map
			}
			
			//Throw away a few values to try to make things more random.
			//for(int i=0;i<10;i++) r.Next(0,1000);

			//build a list of all square coords corresponding to the desired type.
			//and not appearing in the exceptions list.
			//and within the specified boundaries (specified quater or entire map.)
			List<Vector2> filteredList = new List<Vector2>();
			for (int y = 0; y < this.Height; y++)
				for (int x = 0; x < this.Width; x++)
					if (m_squares[x, y].type == t 
						&& !exceptions.Contains(new Vector2(x, y)) 
						&& x>=min_x && x<=max_x && y >= min_y && y <= max_y)
						filteredList.Add(new Vector2(x, y));		

			if (filteredList.Count > 0)
				//Return a random choice from the list.
				return filteredList[Helpers.RandomInt(0, filteredList.Count - 1)];			
			else			
				//Error code (type not found)
				return new Vector2(-1, -1);				
		}

		/// <summary>
		/// Save the map data to a file
		/// </summary>
		/// <param name="filename">Name of the file to save</param>
		public void SaveTofile(string filename)
        {			
            if (filename.Length < 1)
            {
                throw new Exception("No filename specified");
            }

            SerializeGrid();
            ComputeQuads();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename,settings))
            {
                IntermediateSerializer.Serialize<LevelMapData>(writer,this,null);
            }
        }

        /// <summary>
        /// Populates the map grid with data from the serializeable squares array.
        /// </summary>
        public void DeserializeGrid()
        {
            m_squares = new LevelMapSquare[m_width,m_height];

            for (int i = 0; i < m_squares_serialize.Length; i++)            
                m_squares[i % m_width, i / m_width] = m_squares_serialize[i];            
        }

        /// <summary>
        /// Populate the serializeable squares array with data from the map grid.
        /// </summary>
        public void SerializeGrid()
        {
            m_squares_serialize = new LevelMapSquare[m_width * m_height];

            for (int y = 0; y < m_height; y++)
                for (int x = 0; x < m_width; x++)
                    m_squares_serialize[(y * m_width) + x] = m_squares[x, y];
        }

        /// <summary>
        /// Get the square at x,y
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        /// <returns></returns>
        public LevelMapSquare GetSquareAt(int x, int y)
        {
            return m_squares[x, y];
        }

        /// <summary>
        /// Set the square at x,y
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        /// <param name="square">new square</param>
        public void SetSquareAt(int x, int y, LevelMapSquare square)
        {
            m_squares[x,y] = square;
        }

		/// <summary>
		/// Build a list of bounding boxes for wall collision detection. (Bresenham has failed us)
		/// 
		/// Optimisation: Build and store bounding boxes on init.
		/// 
		/// </summary>
		public void CalculateWallCollision()
		{
			if (Walls == null || Walls.Count < 1)
				return;

			WallCollision = new List<BoundingBox>();

			foreach(Quad q in Walls)
			{
				BoundingBox b = new BoundingBox();
				//4 normal possibilities equates to n,s,e,w facing walls. BB should be 2 units thick.

				//north or south
				if (q.Normal == Vector3.Forward)
				{
					b = new BoundingBox(new Vector3(q.LowerLeft.X * 2-8, q.LowerLeft.Y+5, q.LowerLeft.Z * 2 - 0.05f),
						new Vector3(q.UpperRight.X * 2+8, q.UpperRight.Y, q.UpperRight.Z * 2 + 0.05f));
				}
				if (q.Normal == Vector3.Left)
				{
					b = new BoundingBox(new Vector3(q.LowerLeft.X * 2 - 0.05f, q.LowerLeft.Y + 5, q.LowerLeft.Z * 2 + 8),
						new Vector3(q.UpperRight.X * 2 + 0.05f, q.UpperRight.Y, q.UpperRight.Z * 2 - 8));
				}

				this.WallCollision.Add(b);
			}
		}

        /// <summary>
        /// Will calculate the required quads based on the level map's grid.
        /// </summary>
        public void ComputeQuads() // TODO: Improve readability!!!
        {
            Quad q;

            m_floors.Clear();
            m_ceilings.Clear();
            m_walls.Clear();

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.m_squares[x, y].type == MapSquareType.Open)
                    {
                        //Make floor and ceiling quads for all open squares
                        q = new Quad(new Vector3((float)x * 8, 1.0f, (float)y * 8), Vector3.Up, Vector3.Left, 16, 16);
                        this.m_floors.Add(q);
                        q = new Quad(new Vector3((float)x * 8, 9.0f, (float)y * 8), Vector3.Down, Vector3.Right, 16, 16);
                        this.m_ceilings.Add(q);


                        //If y is 0 add a north wall
                        if (y == 0)
                        {
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 - 4), Vector3.Forward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 - 4), Vector3.Backward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }

                        //If the square below is closed or y is height-1 add a south wall
                        if (y == (Height - 1))
                        {
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Forward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Backward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }
                        else if (m_squares[x, y + 1].type == MapSquareType.Closed)
                        {
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Forward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Backward, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }

                        //If x is 0 add a west wall
                        if (x == 0)
                        {
                            q = new Quad(new Vector3((float)x * 8 - 4, 5.0f, (float)y * 8), Vector3.Left, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8 - 4, 5.0f, (float)y * 8), Vector3.Right, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }

                        //If x is width-1 or the square to the right is closed add an east wall
                        if (x == (Width - 1))
                        {
                            q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Left, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Right, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }
                        else if (m_squares[x + 1, y].type == MapSquareType.Closed)
                        {
                            q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Left, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                            q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Right, Vector3.Up, 16, 16);
                            this.m_walls.Add(q);
                        }
                    }
                    //just the gaps to fill.
                    if (this.m_squares[x, y].type == MapSquareType.Closed)
                    {
                        //If open to the south make a wall
                        if (y + 1 < Height)
                        {
                            if (m_squares[x, y + 1].type == MapSquareType.Open)
                            {
                                q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Forward, Vector3.Up, 16, 16);
                                this.m_walls.Add(q);
                                q = new Quad(new Vector3((float)x * 8, 5.0f, (float)y * 8 + 4), Vector3.Backward, Vector3.Up, 16, 16);
                                this.m_walls.Add(q);
                            }
                        }

                        //If open to the east make a wall
                        if (x + 1 < Width)
                        {
                            if (m_squares[x + 1, y].type == MapSquareType.Open)
                            {
                                q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Left, Vector3.Up, 16, 16);
                                this.m_walls.Add(q);
                                q = new Quad(new Vector3((float)x * 8 + 4, 5.0f, (float)y * 8), Vector3.Right, Vector3.Up, 16, 16);
                                this.m_walls.Add(q);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}