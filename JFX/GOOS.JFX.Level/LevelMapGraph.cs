using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMK.Cartography;
using EMK.LightGeometry;
using Microsoft.Xna.Framework;
using GOOS.JFX.Core;

/*
 *   HUGE OPTIMISATION 
 * 
 *   Use Vector2 structs instead of the LightGeometry objects
 *  
 */
namespace GOOS.JFX.Level
{
    /// <summary>
    /// Represents the searchable pathfinding graph for this level which can also perform LOS checks
    /// </summary>
    public class LevelMapGraph
    {
        public LevelMapData level;
        public Graph MapGraph;
        public AStar AS;
        public Node[] MapNodes;

        public Vector2 LastOrigin;
        public Vector2 LastDestination;

        /// <summary>
        /// Construct a new graph for a level
        /// </summary>
        /// <param name="level"></param>
        public LevelMapGraph(LevelMapData level)
        {
            LoadData(level);
        }

        /// <summary>
        /// Create a new Graph from here
        /// </summary>
        /// <param name="l">The level map</param>
        /// <returns>A new level map graph</returns>
        public static LevelMapGraph NewGraphFromMap(LevelMapData l)
        {
            return new LevelMapGraph(l);
        }

        /// <summary>
        /// Build level graph
        /// </summary>
        /// <param name="l"></param>
        public void LoadData(LevelMapData l)
        {
            level = l;
            //make graph based on map

            try
            {
                MapGraph = new Graph();

                MapNodes = new Node[level.Height * level.Width];

                // put nodes in graph and array
                for (int i = 0; i < level.Height * level.Width; i++)
                {
                    if (level.GetSquareAt(i % level.Width, i / level.Width).type == MapSquareType.Open)
                    {
                        MapNodes[i] = MapGraph.AddNode((i % level.Width) * 16 -8, (i / level.Width) * 16 -8, 0);
                    }
                }

                for (int i = 0; i < level.Height * level.Width; i++)
                {
                    //updown
                    if (i / level.Width > 0)
                    {
                        if (MapNodes[i - level.Width] != null && MapNodes[i] != null)
                            MapGraph.Add2Arcs(MapNodes[i], MapNodes[i - level.Width], 1);
                    }
                    //leftright
                    if (i % level.Width > 0)
                    {
                        if (MapNodes[i - 1] != null && MapNodes[i] != null)
                            MapGraph.Add2Arcs(MapNodes[i], MapNodes[i - 1], 1);
                    }
                    //top left diag
                    if (i % level.Width > 0 && i / level.Width > 0)
                    {
                        if (MapNodes[i - level.Width - 1] != null && MapNodes[i] != null && MapNodes[i - level.Width] != null && MapNodes[i - 1] != null)
                            MapGraph.Add2Arcs(MapNodes[i], MapNodes[i - level.Width - 1], 1);
                    }
                    //top right diag
                    if (i % level.Width < level.Width - 1 && i / level.Width > 0)
                    {
                        if (MapNodes[i - level.Width + 1] != null && MapNodes[i] != null && MapNodes[i + 1] != null && MapNodes[i - level.Width] != null)
                            MapGraph.Add2Arcs(MapNodes[i], MapNodes[i - level.Width + 1], 1);
                    }
                }                            
            }
            catch (Exception e) { throw e; }     
        }

        /// <summary>
        /// Set the A* search path
        /// </summary>
        /// <param name="StartGridRef">Origin</param>
        /// <param name="EndGridRef">Destination</param>
        /// <returns>true if a path exists</returns>
        public bool SearchAStarPath(Vector2 StartGridRef, Vector2 EndGridRef)
        {
            if (StartGridRef == LastOrigin && EndGridRef == LastDestination)
                return true;

            AS = new AStar(MapGraph);
            return AS.SearchPath(MapNodes[(int)StartGridRef.X + (int)StartGridRef.Y * level.Width], MapNodes[(int)EndGridRef.X + (int)EndGridRef.Y * level.Width]);            
        }

        /// <summary>
        /// Returns the projected path as a Queue of Vector2 structs
        /// </summary>       
        public Queue<Vector2> GetPathQueue()
        {
            Queue<Vector2> q = new Queue<Vector2>();
           
            for(int i =0;i<AS.PathByCoordinates.Length;i++)
            {
                q.Enqueue(new Vector2((float)AS.PathByCoordinates[i].X,(float)AS.PathByCoordinates[i].Y));
            }

            return q;
        }

        /// <summary>
        /// Returns the projected path as a List of Vector2 structs
        /// </summary>       
        public List<Vector2> GetPathList()
        {
            List<Vector2> l = new List<Vector2>();

            for (int i = 0; i < AS.PathByCoordinates.Length; i++)
            {
                l.Add(new Vector2((float)AS.PathByCoordinates[i].X, (float)AS.PathByCoordinates[i].Y));
                
            }            

            return l;
        }

        /// <summary>
        /// Returns the projected path as an array of Vector2 structs
        /// </summary>       
        public Vector2[] GetPathArray()
        {
            Vector2[] a = new Vector2[AS.PathByCoordinates.Length];

            for (int i = 0; i < AS.PathByCoordinates.Length; i++)
            {
                a[i]=new Vector2((float)AS.PathByCoordinates[i].X, (float)AS.PathByCoordinates[i].Y);
            }

            return a;
        }

        /// <summary>
        /// Get a map grid reference from this map for a given 3D vector.
        /// </summary>
        /// <param name="v">The vector to check</param>
        /// <returns>A the grid reference's corresponding 2D vector</returns>
        public Vector2 GetGridReferenceFromVector(Vector3 v)
        {
            Vector2 v2 = new Vector2();

            double vectorx = (double)v.X;
            double vectory = (double)v.Z;

            vectorx += 8;
            vectory += 8;

            double gridx = vectorx / 16.0;
            double gridy = vectory / 16.0;

            int x = (int)Math.Floor(gridx);
            int y = (int)Math.Floor(gridy);

            v2.X = x;
            v2.Y = y;

            return v2;
        }

        /// <summary>
        /// Get a map grid reference from this map for a given 3D vector.
        /// </summary>
        /// <param name="v">The ref to check</param>
        /// <returns>A the grid reference's corresponding 3D vector</returns>
        public Vector3 GetVectorFromGridReference(Vector2 v)
        {
            Vector3 v3 = new Vector3((v.X * 16) -8, 0,(v.Y* 16) -8);        

            return v3;
        }

		public float IntersectRayWithWalls(Ray r, float maxrange)
		{
			float? collision;
			float nearest = maxrange;

			foreach (BoundingBox b in level.WallCollision)
			{
				if (Vector3.Distance(r.Position, b.Max) <= maxrange)
				{
					collision = r.Intersects(b);
					if (collision.HasValue && collision<nearest)
					{						
						nearest = collision.Value;						
					}
				}
			}

			return nearest;
		}

		/// <summary>
		/// Determine LOS from origin to destination by colliding a ray between the two points
		/// with the bounding boxes of nearby walls
		/// </summary>
		/// <param name="Origin">The Origin vector</param>
		/// <param name="Destination">The destination (target) vector</param>
		/// <param name="maxrange">The max range to check (returns false if exceeded)</param>
		/// <returns>True if in LOS and the two points are within maxrange of eachother</returns>
		public bool GetLOSByWallBBCollision(Vector3 Origin, Vector3 Destination, float maxrange, ref Vector3 PointOfCollision)
		{
			//Origin /= 2;
			//Destination /= 2;

			float distance = Vector3.Distance(Origin, Destination);
			if (Vector3.Distance(Origin, Destination) > maxrange)
				return false;

			Destination -= Origin;
			Destination.Normalize();

			Ray r = new Ray(Origin, Destination);
			float? collision;

			foreach (BoundingBox b in level.WallCollision)
			{
				if (Vector3.Distance(Origin, b.Max) <= distance + 32)
				{
					b.Intersects(ref r, out collision);
					if (collision.HasValue && collision < distance)
					{
						Destination *= (float)collision;
						PointOfCollision = Origin + Destination;
						return false; // There was something in the way.
					}
				}
			}

			return true; // There was nothing in the way;
		}		

        /// <summary>
        /// Determine whether a destination square is visible in a straight line from an origin square.
        /// And return the path as a list of vector 2 objects;
        /// </summary>
        /// <param name="Origin">The origin grid reference.</param>
        /// <param name="Destination">The destination grid reference.</param>
        /// <param name="path">A reference to a list of vector 2 objects to contain path info.</param>
        /// <returns>True if the destination point is visible.</returns>
        public bool GetLOS(Vector2 Origin, Vector2 Destination,ref IEnumerable<Vector2> path)
        {       
            // FIX NASTY BUG
            if (Destination.X > Origin.X && Destination.Y < Origin.Y)
            {
                Vector2 t = Vector2.Zero;
                t = Origin;
                Origin = Destination;
                Destination = t;                
            }

            if (Math.Abs(Destination.Y - Origin.Y) < Math.Abs(Destination.X - Origin.X))
            {
                // dX > dY... not steep
                if (Destination.X >= Origin.X)
                {
                    path= BresLineOrig(Origin, Destination);
                }
                else
                {
                    path = BresLineReverseOrig(Origin, Destination);
                }
            }
            else // steep (dY > dX)
            {
                if (Destination.Y >= Origin.Y)
                {
                    path = BresLineSteep(Origin, Destination);
                }
                else
                {
                    path = BresLineReverseSteep(Origin, Destination);                    
                }
            }                       

            //If there are any null nodes in the path, return false;
            foreach(Vector2 v in path)
            {               
                if (MapNodes[(int)v.X + (int)v.Y * level.Width] == null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
        /// * where x0 less than x1 and y0 less than y1
        ///   AND line is less steep than it is wide (dx less than dy)
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> BresLineOrig(Vector2 begin, Vector2 end)
        {
            Vector2 nextPoint = begin;
            int deltax = (int)end.X - (int)begin.X;
            int deltay = (int)end.Y - (int)begin.Y;
            int error = deltax / 2;
            int ystep = 1;
            if (end.Y < begin.Y)
            {
                ystep = -1;
            }
            else if (end.Y == begin.Y)
            {
                ystep = 0;
            }

            while (nextPoint.X < end.X)
            {
                if (nextPoint != begin) yield return nextPoint;
                nextPoint.X++;

                error -= deltay;
                if (error < 0)
                {
                    nextPoint.Y += ystep;
                    error += deltax;
                }
            }
        }

        /// <summary>
        /// Whenever dy > dx the line is considered steep and we have to change
        /// which variables we increment/decrement
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> BresLineSteep(Vector2 begin, Vector2 end)
        {
            Vector2 nextPoint = begin;
            int deltax = Math.Abs((int)end.X - (int)begin.X);
            int deltay = (int)end.Y - (int)begin.Y;
            int error = Math.Abs(deltax / 2);
            int xstep = 1;

            if (end.X < begin.X)
            {
                xstep = -1;
            }
            else if (end.X == begin.X)
            {
                xstep = 0;
            }

            while (nextPoint.Y < end.Y)
            {
                if (nextPoint != begin) yield return nextPoint;
                nextPoint.Y++;

                error -= deltax;
                if (error < 0)
                {
                    nextPoint.X += xstep;
                    error += deltay;
                }
            }
        }

        /// <summary>
        /// If x0 > x1 then we are going from right to left instead of left to right
        /// so we have to modify our routine slightly
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> BresLineReverseOrig(Vector2 begin, Vector2 end)
        {
            Vector2 nextPoint = begin;
            int deltax = (int)end.X - (int)begin.X;
            int deltay = (int)end.Y - (int)begin.Y;
            int error = deltax / 2;
            int ystep = 1;

            if (end.Y < begin.Y)
            {
                ystep = -1;
            }
            else if (end.Y == begin.Y)
            {
                ystep = 0;
            }

            while (nextPoint.X > end.X)
            {
                if (nextPoint != begin) yield return nextPoint;
                nextPoint.X--;

                error += deltay;
                if (error < 0)
                {
                    nextPoint.Y += ystep;
                    error -= deltax;
                }
            }
        }		

        /// <summary>
        /// If x0 > x1 and dy > dx we have to go from right to left and alter the routine
        /// for a steep line
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> BresLineReverseSteep(Vector2 begin, Vector2 end)
        {
            Vector2 nextPoint = begin;
            int deltax = (int)end.X - (int)begin.X;
            int deltay = (int)end.Y - (int)begin.Y;
            int error = deltax / 2;
            int xstep = 1;

            if (end.X < begin.X)
            {
                xstep = -1;
            }
            else if (end.X == begin.X)
            {
                xstep = 0;
            }

            while (nextPoint.Y > end.Y)
            {
                if (nextPoint != begin) yield return nextPoint;
                nextPoint.Y--;

                error += deltax;
                if (error < 0)
                {
                    nextPoint.X += xstep;
                    error -= deltay;
                }
            }
        }
    }
}
