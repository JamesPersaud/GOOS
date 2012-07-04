using System;

namespace GOOS.JFX.Level
{
	/// <summary>
	/// 
	/// This class is taken from Mike Gold's blog http://www.gamespp.com/c/generatingMazes.html
	/// 
	/// Summary description for Cell.
	/// </summary>
	public class Cell
	{
		public static int kCellSize = 10;
		public static int kPadding = 5;
		public int[] Walls = new int[4] { 1, 1, 1, 1 };
		public int Row;
		public int Column;
		private static long Seed = DateTime.Now.Ticks;
		static public Random TheRandom = new Random((int)Cell.Seed);


		public Cell()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public bool HasAllWalls()
		{
			for (int i = 0; i < 4; i++)
			{
				if (Walls[i] == 0)
					return false;
			}

			return true;
		}

		public void KnockDownWall(int theWall)
		{
			Walls[theWall] = 0;
		}

		public void KnockDownWall(Cell theCell)
		{
			// find adjacent wall
			int theWallToKnockDown = FindAdjacentWall(theCell);
			Walls[theWallToKnockDown] = 0;
			int oppositeWall = (theWallToKnockDown + 2) % 4;
			theCell.Walls[oppositeWall] = 0;
		}


		public int FindAdjacentWall(Cell theCell)
		{
			if (theCell.Row == Row)
			{
				if (theCell.Column < Column)
					return 0;
				else
					return 2;
			}
			else // columns are the same
			{
				if (theCell.Row < Row)
					return 1;
				else
					return 3;
			}
		}

		public int GetRandomWall(int dim)
		{
			int nextWall = TheRandom.Next(0, 3);
			while ((Walls[nextWall] == 0)
			|| ((Row == 0) && (nextWall == 0)) ||
					((Row == dim - 1) && (nextWall == 2)) ||
					((Column == 0) && (nextWall == 1)) ||
					((Column == dim - 1) && (nextWall == 3))
				   )
			{
				nextWall = TheRandom.Next(0, 3);
			}

			return nextWall;
		}		
	}
}
