using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GOOS.JFX.Core
{
	/// <summary>
	/// A collection of static methods to help with collision detection.
	/// </summary>
	public class Collision
	{		
		/// <summary>
		/// Simplest collision test between a Vector3 and a grid.              
		/// 
		/// </summary>
		/// <param name="v">The test vector</param>
		/// <param name="grid">The test grid - true represents closed</param>
		/// <returns>true if the vector is within a closed grid square</returns>
		public static bool CollideVector3WithGridSquare(ref Microsoft.Xna.Framework.Vector3 v, ref bool[] grid, int gridwidth, int gridheight, int squarewidth, int squareheight, ref int x, ref int y)
		{
			//the vector comes in with x and z values.......... 
			//grid x=0 lies between vector x=-4 and vector x = 4 | if we add 4 to x these values become 0 and 8

			double vectorx = (double)v.X;
			double vectory = (double)v.Z;

			vectorx += 8;
			vectory += 8;

			double gridx = vectorx / 16.0;
			double gridy = vectory / 16.0;

			x = (int)Math.Floor(gridx);
			y = (int)Math.Floor(gridy);

			if (x < 0 || y < 0 || x > gridwidth - 1 || y > gridheight - 1)
				return true;
			if (grid[x + (y * gridwidth)])
				return true;

			//If not in a closed square - how close to the edge is the requested point?
			float gridx1, gridx2, gridy1, gridy2;

			// Get the bounds of the current square;
			gridx1 = x * 16;
			gridx2 = gridx1 + 16;
			gridy1 = y * 16;
			gridy2 = y * 16 + 16;

			// set edge buffer distance size
			float buffer = 1.5f;

			//which ways are blocked?
			bool n = false, e = false, s = false, w = false;

			n = (y == 0);                       // north wall blocked obvious case
			e = (x > gridwidth - 1);            // east wall blocked obvious case
			s = (y > gridheight - 1);           // south wall blocked obvious case
			w = (x == 0);                       // west wall blocked obvious case

			if (!n)                                     //check against square flags
				n = grid[x + ((y - 1) * gridwidth)];
			if (!e)
				e = grid[(x + 1) + (y * gridwidth)];
			if (!s)
				s = grid[x + ((y + 1) * gridwidth)];
			if (!w)
				w = grid[(x - 1) + (y * gridwidth)];

			if (n && vectory < gridy1 + buffer)         //check player proximity to wall
				return true;

			if (e && vectorx > gridx2 - buffer)
				return true;

			if (s && vectory > gridy2 - buffer)
				return true;

			if (w && vectorx < gridx1 + buffer)
				return true;

			return false;
		}

		/// <summary>
		/// Returns false when square is blocked - use in LOS algorithm
		/// </summary>     
		public static bool CollideVector3WithGridForLOS(ref Microsoft.Xna.Framework.Vector3 v, ref bool[] grid, ref int gridwidth, ref int gridheight, ref int x, ref int y)
		{
			double vectorx = (double)v.X;
			double vectory = (double)v.Z;

			vectorx += 8;
			vectory += 8;

			double gridx = vectorx / 16.0;
			double gridy = vectory / 16.0;

			x = (int)Math.Floor(gridx);
			y = (int)Math.Floor(gridy);

			if (x < 0 || y < 0 || x > gridwidth - 1 || y > gridheight - 1)
				return true;

			return (grid[x + (y * gridwidth)]);
		}

		// triangle intersect from http://www.graphics.cornell.edu/pubs/1997/MT97.pdf

		/// <summary>
		/// Intersect A ray with a triangle
		/// </summary>
		/// <param name="r">The input ray</param>
		/// <param name="vert0">1st triangle vertex</param>
		/// <param name="vert1">2nd triangle vertex</param>
		/// <param name="vert2">3rd triangle vertex</param>
		/// <param name="t">ouput float holding the distance along the ray to the triangle.</param>
		/// <returns>True if there is an intersection, false if not.</returns>
		public static bool RayTriangleIntersect(Ray r, Vector3 vert0, Vector3 vert1, Vector3 vert2, out float t)
		{
			t = 0;

			Vector3 edge1 = vert1 - vert0;
			Vector3 edge2 = vert2 - vert0;

			Vector3 tvec, pvec, qvec;
			float det, inv_det;

			pvec = Vector3.Cross(r.Direction, edge2);

			det = Vector3.Dot(edge1, pvec);

			if (det > -0.00001f)
				return false;

			inv_det = 1.0f / det;

			tvec = r.Position - vert0;

			float u = Vector3.Dot(tvec, pvec) * inv_det;
			if (u < -0.001f || u > 1.001f)
				return false;

			qvec = Vector3.Cross(tvec, edge1);

			float v = Vector3.Dot(r.Direction, qvec) * inv_det;
			if (v < -0.001f || u + v > 1.001f)
				return false;

			t = Vector3.Dot(edge2, qvec) * inv_det;

			if (t <= 0)
				return false;

			return true;
		}

		/// <summary>
		/// Get the angle between two vectors on the xz plane.
		/// </summary>
		/// <param name="position">The first vector</param>      
		/// <param name="target">The second vector</param>    
		/// <returns>The angle between the two on the xz plane in radians.</returns>
		public static float GetAngleBetween_XZ(Vector3 position, Vector3 target)
		{
			float y = position.Y;
			float locationz = target.Z;
			target.Y = 0.0f;
		         
			target -= position;

			//Get the angle between the translated vector and the x axis (x axis unit vector is 1.0,0.0,0.0
			float costheta;
			target.Normalize();

			if (locationz <= position.Z)
				costheta = Vector3.Dot(target, Vector3.Right);
			else
				costheta = Vector3.Dot(target, Vector3.Left);

			double theta = Math.Acos(costheta);

			if (locationz > position.Z)
				theta += MathHelper.ToRadians(180);

			return (float)theta;			
		}


		//bool SelectPolyFromModel(Model m,Matrix transform,Ray clickRay,out int meshIndex,out int polyIndex)
		//{
		//    meshIndex = -1;
		//    polyIndex = -1;
		//    float fClosestPoly = float.MaxValue;

		//    Matrix[] transforms = new Matrix[m.Bones.Count];
		//    m.CopyAbsoluteBoneTransformsTo(transforms);

		//    Matrix mat = Matrix.Invert(transform);
		//    clickRay.Position = Vector3.Transform(clickRay.Position, mat);
		//    clickRay.Direction = Vector3.TransformNormal(clickRay.Direction, mat);

		//    for (int i = 0; i < m.Meshes.Count; i++)
		//    {
		//        ModelMesh mesh = m.Meshes[i];

		//        if (mesh.BoundingSphere.Intersects(clickRay).HasValue == false)
		//        {
		//            continue;
		//        }

		//        VertexPositionNormal[] vertices = new VertexPositionNormal[
		//                            mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride];

		//        mesh.VertexBuffer.GetData<VertexPositionNormal>(vertices);

		//        if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
		//        {
		//            short[] indices = new short[mesh.IndexBuffer.SizeInBytes / sizeof(short)];

		//            mesh.IndexBuffer.GetData<short>(indices);

		//            for (int x = 0; x < indices.Length; x += 3)
		//            {
		//                float fDist;

		//                if (Collision.Intersection.RayTriangleIntersect(
		//                    clickRay,
		//                    vertices[indices[x + 0]].Position,
		//                    vertices[indices[x + 1]].Position,
		//                    vertices[indices[x + 2]].Position,
		//                    out fDist))
		//                {
		//                    if (fDist < fClosestPoly)
		//                    {
		//                        meshIndex = i;
		//                        polyIndex = x / 3;

		//                        fClosestPoly = fDist;
		//                    }
		//                }
		//            }
		//        }
		//        else if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
		//        {
		//            int[] indices = new int[mesh.IndexBuffer.SizeInBytes / sizeof(int)];

		//            mesh.IndexBuffer.GetData<int>(indices);


		//            for (int x = 0; x < indices.Length; x += 3)
		//            {
		//                float fDist;

		//                if (Collision.Intersection.RayTriangleIntersect(
		//                    clickRay,
		//                    vertices[indices[x + 0]].Position,
		//                    vertices[indices[x + 1]].Position,
		//                    vertices[indices[x + 2]].Position,
		//                    out fDist))
		//                {
		//                    if (fDist < fClosestPoly)
		//                    {
		//                        meshIndex = i;
		//                        polyIndex = x / 3;

		//                        fClosestPoly = fDist;
		//                    }
		//                }
		//            }
		//        }
		//    }
		//    return meshIndex != -1 && polyIndex != -1;
		//}
	}
}