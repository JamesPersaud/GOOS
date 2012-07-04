using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GOOS.JFX.Level
{
	/// <summary>
	/// Used to generate various random maps
	/// </summary>
	public class RandomMapGenerator
	{		
		#region Constructors

		/// <summary>
		/// Empty constructor, not used.
		/// </summary>
		private RandomMapGenerator()
		{
		}

		#endregion

		#region Factory

		/// <summary>
		/// Get a new square random level map.
		/// </summary>
		/// <param name="dimention">The length of a side of the map.</param>
		/// <param name="rand">The random number provider.</param>
		/// <returns>A new Level Map.</returns>
		public static LevelMapData RandomSquareLevelMap(int dimention, Random rand, int wallstobreak)
		{
			//Make a new random square maze of the given dimention.
			Maze sourceMaze = new Maze(dimention);
			sourceMaze.Generate();
			
			//Generate level map based on maze.
			int yoffset = 1;
			LevelMapSquare topleft = new LevelMapSquare(MapSquareType.Open);
			LevelMapSquare topright = new LevelMapSquare(MapSquareType.Open);
			LevelMapSquare bottomleft = new LevelMapSquare(MapSquareType.Open);
			LevelMapSquare bottomright = new LevelMapSquare(MapSquareType.Open);
			LevelMapData MyLevel = new LevelMapData(sourceMaze.kDimension * 2 + 1, sourceMaze.kDimension * 2 + 1);

			for (int i = 0; i < sourceMaze.kDimension * sourceMaze.kDimension; i++)
			{
				topleft = new LevelMapSquare(MapSquareType.Open);
				topright = new LevelMapSquare(MapSquareType.Open);
				bottomleft = new LevelMapSquare(MapSquareType.Open);
				bottomright = new LevelMapSquare(MapSquareType.Open);

				//Do the west and south walls.
				//west
				if (sourceMaze.Cells[i % sourceMaze.kDimension, i / sourceMaze.kDimension].Walls[1] == 1)
				{
					topleft.type = MapSquareType.Closed;
					bottomleft.type = MapSquareType.Closed;
				}
				if (sourceMaze.Cells[i % sourceMaze.kDimension, i / sourceMaze.kDimension].Walls[2] == 1)
				{
					bottomleft.type = MapSquareType.Closed;
					bottomright.type = MapSquareType.Closed;
				}

				// maze x = map x*2
				MyLevel.SetSquareAt((i % sourceMaze.kDimension) * 2, (i / sourceMaze.kDimension) * 2 + yoffset, topleft);
				MyLevel.SetSquareAt(((i % sourceMaze.kDimension) * 2) + 1, (i / sourceMaze.kDimension) * 2 + yoffset, topright);
				MyLevel.SetSquareAt(((i % sourceMaze.kDimension) * 2) + 1, ((i / sourceMaze.kDimension) * 2) + 1 + yoffset, bottomright);
				MyLevel.SetSquareAt((i % sourceMaze.kDimension) * 2, ((i / sourceMaze.kDimension) * 2) + 1 + yoffset, bottomleft);

				//This method misses out corner bits - so if a square has both a right and up wall, fill the corner.
				if (i / sourceMaze.kDimension > 0 && i % sourceMaze.kDimension < sourceMaze.kDimension - 1)
				{
					if (sourceMaze.Cells[i % sourceMaze.kDimension, i / sourceMaze.kDimension].Walls[0] == 1
						&& sourceMaze.Cells[i % sourceMaze.kDimension, i / sourceMaze.kDimension].Walls[3] == 1)
					{
						MyLevel.SetSquareAt(((i % sourceMaze.kDimension) * 2) + 2, ((i / sourceMaze.kDimension) * 2) - 1 + yoffset, new LevelMapSquare(MapSquareType.Closed));
					}
				}
			}

			//Break down a few walls.
			Vector2 walltobreak;
			List<Vector2> exclusion = new List<Vector2>();
			for (int i = 0; i < MyLevel.Width; i++)
			{
				exclusion.Add(new Vector2(i, 0));//don't break down top north wall
				exclusion.Add(new Vector2(i, MyLevel.Height - 1));//or south wall
				exclusion.Add(new Vector2(0, i));//or east wall
				exclusion.Add(new Vector2(MyLevel.Width - 1, i));//or west wall
			}
			for (int i = 0; i < wallstobreak; i++)
			{
				walltobreak = MyLevel.GetRandomSquareCoordsByType(MapSquareType.Closed, null, exclusion, 0);
				MyLevel.SetSquareAt((int)walltobreak.X, (int)walltobreak.Y, new LevelMapSquare(MapSquareType.Open));
			}

			MyLevel.ComputeQuads();
			return MyLevel;		
		}

		#endregion
	}
}
