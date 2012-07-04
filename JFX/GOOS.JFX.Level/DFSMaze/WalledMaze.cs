using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GOOS.JFX.Level
{
	/// <summary>
	/// 
	/// TODO - either complete or delete this implementation
	/// 
	/// A Walled Maze for use in depth first map generation of maze maps
	/// </summary>
	public class WalledMaze
	{
		#region Members	

		private Random Rand;
		private int Height;
		private int Width;
		private int TotalSquares;		

		#endregion

		#region properties

		public WalledSquare[] Squares;

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="w">Height to initialize the squares array</param>
		/// <param name="h">Width to initialize the squares array</param>
		/// <param name="open">Initial State of all walls</param>		
		public WalledMaze(int w, int h,bool open)
		{
			Squares = new WalledSquare[w * h];
			ClearAllWalls(open);

			Width = w;
			Height = h;
			TotalSquares = w * h;

			Rand = new Random();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Set all walls to open or closed
		/// </summary>
		/// <param name="open">open or closed flag</param>		
		private void ClearAllWalls(bool open)
		{
			for (int ws =0;ws<Squares.Length;ws++)
			{
				Squares[ws].N = open;
				Squares[ws].E = open;
				Squares[ws].S = open;
				Squares[ws].W = open;			
			}
		}

		/// <summary>
		/// Makes a random maze by removing walls from a fully walled maze.
		/// </summary>
		/// <param name="StartLocation"></param>
		public void MakeMaze(Vector2 StartLocation)
		{
			Enum4PointCompass NextDirection;
			int NextDirection_x;
			int NextDirection_y;
			Vector2 CurrentSquare = StartLocation;
			List<Vector2> Visited = new List<Vector2>();
			Stack<Vector2> ReTrace = new Stack<Vector2>();

			while (Visited.Count < TotalSquares)
			{
				//Find a random neighbouring cell that hasn't been visited.
				
			}			
		}
		
		#endregion
	}
}
