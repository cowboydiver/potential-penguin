using UnityEngine;
using System.Collections;
using System;

public class MeshBuilder : MonoBehaviour
{
		public MeshBuilder ()
		{
		}

		static public Mesh fillLineMesh (double[] topXValues, double[] topYValues, double[] bottomXValues, double[] bottomYValues, double width)
		{
				Mesh mesh = new Mesh ();
				Vector3[] newVertices;
				Vector2[] newUVs;
				int[] newTriangles;

				int resolutionOfPath = topXValues.Length;

				Vector3[] topVerts = makeStripVertices (topXValues, topYValues, width);
				Vector2[] topUVs = makeTopUVs (topVerts);
				int[] topTris = makeTopTris (topVerts);

				Vector3[] bottomVerts = makeStripVertices (bottomXValues, bottomYValues, width);
				Vector2[] bottomUVs = makeBottomUVs (bottomVerts);
				int[] bottomTris = makeBottomTris (bottomVerts);

				for (int i = 0; i< bottomTris.Length; i++) {
						bottomTris [i] += topVerts.Length;
				}

				Vector3[] endVerts = MakeEndVertices (topXValues,topYValues,bottomYValues,width);
			
				Vector2[] endUVs = MakeEndUVs (topXValues, width);
			
				int endsOffset = topVerts.Length + bottomVerts.Length;
				int[] ends = MakeEndTris (endsOffset);
			
			
				Vector3[] side1Verts = MakeSide1Vertices(topXValues,topYValues,bottomXValues,bottomYValues);
			
				Vector2[] side1UVs = MakeSide1UVs (side1Verts,topYValues,resolutionOfPath);

				int side1Offset = endsOffset + endVerts.Length;
				int[] side1 = MakeSide1Tris (side1Offset,topVerts);
			
				Vector3[] side2Verts = MakeSide2Vertices (topXValues,topYValues,bottomXValues,bottomYValues,width);
			
				Vector2[] side2UVs = MakeSide2UVs (side2Verts,topYValues,resolutionOfPath);
			
				int side2Offset = side1Offset + side1Verts.Length;
				int[] side2 = MakeSide2Tris (side2Offset,topVerts);
			
				newVertices = new Vector3[topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length + side2Verts.Length];
				topVerts.CopyTo (newVertices, 0);
				bottomVerts.CopyTo (newVertices, topVerts.Length);
				endVerts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length);
				side1Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length);
				side2Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length);
			
				newUVs = new Vector2[topUVs.Length + bottomUVs.Length + endUVs.Length + side1UVs.Length + side2UVs.Length];
				topUVs.CopyTo (newUVs, 0);
				bottomUVs.CopyTo (newUVs, topUVs.Length);
				endUVs.CopyTo (newUVs, topUVs.Length + bottomUVs.Length);
				side1UVs.CopyTo (newUVs, topUVs.Length + bottomUVs.Length + endUVs.Length);
				side2UVs.CopyTo (newUVs, topUVs.Length + bottomUVs.Length + endUVs.Length + side1UVs.Length);
			
				newTriangles = new int[topTris.Length + bottomTris.Length + ends.Length + side1.Length + side2.Length];
				topTris.CopyTo (newTriangles, 0);
				bottomTris.CopyTo (newTriangles, topTris.Length);
				ends.CopyTo (newTriangles, topTris.Length + bottomTris.Length);
				side1.CopyTo (newTriangles, topTris.Length + bottomTris.Length + ends.Length);
				side2.CopyTo (newTriangles, topTris.Length + bottomTris.Length + ends.Length + side1.Length);
			
				mesh.vertices = newVertices;
				mesh.uv = newUVs;
				mesh.triangles = newTriangles;
				mesh.RecalculateNormals ();
				calculateMeshTangents(mesh);
			
				//Debug.Log ("Built new line mesh!");
				return mesh;
		}

		/*****************************************************************************
		 * 	Recomputation of vertices assume that the mesh has completely standard rotation
		 * 
		 *****************************************************************************/
         /*
		public static Vector3[] RecomputeVertices (Potential potential, Mesh mesh){
			Vector3[] newVertices;
			Vector3[] oldVertices = mesh.vertices;

			int resolutionOfPath = (oldVertices.Length-8)/8;

			double minXValue = oldVertices [0].x;
			double maxXValue = oldVertices [2*resolutionOfPath - 1].x;

			//Debug.Log ("Min xvalue was " + minXValue + " and max XValue was " + maxXValue);
			double[] xDoubles = Grid.linspace (minXValue, maxXValue, resolutionOfPath);
			//double[] topXValues = ArrayUtils.toFloatArray (xDoubles);
			double[] topXValues = xDoubles;
		
			//double[] topYValues = ArrayUtils.toFloatArray(potential.getValues(xDoubles,0));
			double[] topYValues = potential.getValues(xDoubles,0);

			double[] bottomXValues = topXValues;

			// We assume a flat bottom
			double minYValue = oldVertices [2*resolutionOfPath + 2].y;
			//double[] bottomYValues = ArrayUtils.todoubleArray(Grid.linspace (minYValue, minYValue, resolutionOfPath));
			double[] bottomYValues = Grid.linspace (minYValue, minYValue, resolutionOfPath);

			double width = Mathf.Abs (oldVertices [0].z - oldVertices [1].z);

			Vector3[] topVerts = makeStripVertices (topXValues, topYValues, width);
			Vector3[] bottomVerts = makeStripVertices (bottomXValues, bottomYValues, width);
			Vector3[] endVerts = MakeEndVertices (topXValues,topYValues,bottomYValues,width);
			Vector3[] side1Verts = MakeSide1Vertices(topXValues,topYValues,bottomXValues,bottomYValues);
			Vector3[] side2Verts = MakeSide2Vertices (topXValues,topYValues,bottomXValues,bottomYValues,width);

			newVertices = new Vector3[topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length + side2Verts.Length];
			topVerts.CopyTo (newVertices, 0);
			bottomVerts.CopyTo (newVertices, topVerts.Length);
			endVerts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length);
			side1Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length);
			side2Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length);

			return newVertices;
		}
        */
		static public void RecomputeLineMeshVertices (double[] topXValues, double[] topYValues, double[] bottomXValues, double[] bottomYValues, double width, Mesh mesh)
		{
			Vector3[] newVertices;
			
			//int resolutionOfPath = topXValues.Length;
		
			Vector3[] topVerts = makeStripVertices (topXValues, topYValues, width);
			Vector3[] bottomVerts = makeStripVertices (bottomXValues, bottomYValues, width);
			Vector3[] endVerts = MakeEndVertices (topXValues,topYValues,bottomYValues,width);
			Vector3[] side1Verts = MakeSide1Vertices(topXValues,topYValues,bottomXValues,bottomYValues);
			Vector3[] side2Verts = MakeSide2Vertices (topXValues,topYValues,bottomXValues,bottomYValues,width);

			newVertices = new Vector3[topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length + side2Verts.Length];
			topVerts.CopyTo (newVertices, 0);
			bottomVerts.CopyTo (newVertices, topVerts.Length);
			endVerts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length);
			side1Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length);
			side2Verts.CopyTo (newVertices, topVerts.Length + bottomVerts.Length + endVerts.Length + side1Verts.Length);

			mesh.vertices = newVertices;
			mesh.RecalculateNormals ();
			calculateMeshTangents(mesh);
		
		}

		/*****************************************************************************
		 * 
		 * 		Vertex generation functions
		 * 
		 * 
		 ******************************************************************************/
		private static Vector3[] makeStripVertices (double[] xPositions, double[] yPositions, double width)
		{
			
				if (xPositions.Length != yPositions.Length) {
						return new Vector3[0];
				}
				Vector3[] verts = new Vector3[xPositions.Length * 2];
				for (int i = 0; i < xPositions.Length; i++) {
			verts [2 * i] = new Vector3 ((float) xPositions [i], (float) yPositions [i], 0f);
			verts [2 * i + 1] = new Vector3 ((float) xPositions [i], (float) yPositions [i], (float) width);
						
				}
				return verts;
		}

		private static Vector3[] MakeEndVertices(double[] topXValues, double[] topYValues, double[] bottomYValues, double width){
			return new Vector3[] {
			new Vector3 ((float) topXValues [0], (float) topYValues [0], (float) 0),
			new Vector3 ((float) topXValues [0], (float) topYValues [0], (float) width),
			new Vector3 ((float) topXValues [0], (float) bottomYValues [0], (float) 0),
			new Vector3 ((float) topXValues [0], (float) bottomYValues [0], (float) width),
			new Vector3 ((float) topXValues [topXValues.Length - 1], (float) topYValues [topXValues.Length - 1], (float) 0),
			new Vector3 ((float) topXValues [topXValues.Length - 1], (float) topYValues [topXValues.Length - 1], (float) width),
			new Vector3 ((float) topXValues [topXValues.Length - 1], (float) bottomYValues [topXValues.Length - 1], (float) 0),
			new Vector3 ((float) topXValues [topXValues.Length - 1], (float) bottomYValues [topXValues.Length - 1], (float) width)
			};
		}

	private static Vector3[] MakeSide1Vertices(double[] topXValues, double[] topYValues, double[] bottomXValues, double[] bottomYValues){
			Vector3[] side1Verts = new Vector3[topYValues.Length * 2];
			for (int i = 0; i < topYValues.Length; i++) {
			side1Verts [2 * i] = new Vector3 ((float) topXValues [i], (float) topYValues [i], (float) 0);
			side1Verts [2 * i + 1] = new Vector3 ((float) bottomXValues [i], (float) bottomYValues [i], (float) 0);
			}
			return side1Verts;
		}

		private static Vector3[] MakeSide2Vertices(double[] topXValues, double[] topYValues, double[] bottomXValues, double[] bottomYValues, double width){
			Vector3[] side2Verts = new Vector3[topYValues.Length * 2];
			for (int i = 0; i < topYValues.Length; i++) {
			side2Verts [2 * i] = new Vector3 ((float) topXValues [i], (float) topYValues [i], (float) width);
			side2Verts [2 * i + 1] = new Vector3 ((float) bottomXValues [i], (float) bottomYValues [i], (float) width);
			}
			return side2Verts;
		}


		/*****************************************************************************
		 * 
		 * 		UV generation functions
		 * 		THESE ARE NOT COMPLETELY FUNCTIONAL AS THEY MAKE COORDINATES LARGER THAN 1
		 * 
		 ******************************************************************************/
		
		private static Vector2[] makeTopUVs (Vector3[] verts)
		{
			
				Vector2[] UVs = new Vector2[verts.Length];
				for (int i = 0; i < verts.Length/2; i++) {
						UVs [2 * i] = new Vector2 (i * 4.0f / verts.Length, 0);
						UVs [2 * i + 1] = new Vector2 (i * 4.0f / verts.Length, 0.15f);
				}
				return UVs;
		}

		private static Vector2[] MakeEndUVs (double[] topXValues, double width){
			return new Vector2[]{
			new Vector2 (0, 2.0f * 0.15f), new Vector2 ((float) (width / (topXValues [topXValues.Length - 1] - topXValues [0])), 2.0f * 0.15f),
			new Vector2 (0, 3.0f * 0.15f), new Vector2 ((float) (width / (topXValues [topXValues.Length - 1] - topXValues [0])), 3.0f * 0.15f),
			new Vector2 ((float) (width / (topXValues [topXValues.Length - 1] - topXValues [0])), 2.0f * 0.15f), new Vector2 ((float) (2 * width / (topXValues [topXValues.Length - 1] - topXValues [0])), 2.0f * 0.15f),
			new Vector2 ((float) (width / (topXValues [topXValues.Length - 1] - topXValues [0])), 3.0f * 0.15f), new Vector2 ((float) (2 * width / (topXValues [topXValues.Length - 1] - topXValues [0])), 3.0f * 0.15f)
			};
		}
		
		private static Vector2[] makeBottomUVs (Vector3[] verts)
		{
			
				Vector2[] UVs = new Vector2[verts.Length];
				for (int i = 0; i < verts.Length/2; i++) {
					UVs [2 * i] = new Vector2 ((float) (i * 3.0 / verts.Length), (float) 0.15);
					UVs [2 * i + 1] = new Vector2 ((float) (i * 3.0 / verts.Length), (float) (2 * 0.15));
				}
				return UVs;
		}

		private static Vector2[] MakeSide1UVs (Vector3[] side1Verts, double[] topYValues, int resolutionOfPath){
			Vector2[] side1UVs = new Vector2[side1Verts.Length];
			for (int i = 0; i < topYValues.Length; i++) {
			side1UVs [2 * i] = new Vector2 ((float) (i * 1.0 / resolutionOfPath), (float) (3.0 * 0.15 + 0.50 - (topYValues [i] + 1.0) / (Mathf.Max(ArrayUtils.toFloatArray(topYValues)) + 1.0) * 0.50));
			side1UVs [2 * i + 1] = new Vector2 ((float) (i * 1.0 / resolutionOfPath), (float) (3.0 * 0.15 + 0.50));
			} 
			return side1UVs;
		}

		private static Vector2[] MakeSide2UVs(Vector3[] side2Verts, double[] topYValues, int resolutionOfPath){
			Vector2[] side2UVs = new Vector2[side2Verts.Length];
			for (int i = 0; i < topYValues.Length; i++) {
			side2UVs [2 * i] = new Vector2 ((float) (i * 1.0 / resolutionOfPath), (float) (3.0 * 0.15 + 2.0 * 0.50 - (topYValues [i] + 1.0) / (Mathf.Max(ArrayUtils.toFloatArray(topYValues)) + 1.0f) * 0.50));
			side2UVs [2 * i + 1] = new Vector2 ((float) (i * 1.0 / resolutionOfPath), (float) (3.0 * 0.15 + 2.0 * 0.50));
			}
			return side2UVs;
		}

		/*****************************************************************************
		 * 
		 * 		Triangle generation functions
		 * 
		 * 
		 ******************************************************************************/
		
		private static int[] makeTopTris (Vector3[] verts)
		{
				if (verts.Length < 3) {
						return new int[0];		
				}
				int[] tris = new int[(verts.Length - 2) * 3];
				for (int i = 0; i < verts.Length-2; i++) {
						if (i % 2 == 0) {
								tris [3 * i] = i;
								tris [3 * i + 1] = i + 1;
								tris [3 * i + 2] = i + 2;
						} else {
								tris [3 * i] = i;
								tris [3 * i + 1] = i + 2;
								tris [3 * i + 2] = i + 1;
						}
				}
				return tris;
		}
		
		private static int[] makeBottomTris (Vector3[] verts)
		{
				if (verts.Length < 3) {
						return new int[0];		
				}
				int[] tris = new int[(verts.Length - 2) * 3];
				for (int i = 0; i < verts.Length-2; i++) {
						if (i % 2 == 0) {
								tris [3 * i] = i;
								tris [3 * i + 1] = i + 2;
								tris [3 * i + 2] = i + 1;
						} else {
								tris [3 * i] = i;
								tris [3 * i + 1] = i + 1;
								tris [3 * i + 2] = i + 2;
						}
				}
				return tris;
		}

		private static int[] MakeEndTris (int endsOffset){
			return new int[]{endsOffset,endsOffset + 2,endsOffset + 1, 
				endsOffset + 1,endsOffset + 2,endsOffset + 3, 
				endsOffset + 4,endsOffset + 5,endsOffset + 6, 
				endsOffset + 6,endsOffset + 5,endsOffset + 7};
		}

		private static int[] MakeSide1Tris (int side1Offset, Vector3[] topVerts){
			int[] side1 = new int[(topVerts.Length - 2) * 3];
			for (int i = 0; i < (topVerts.Length-2)/2; i++) {
				side1 [6 * i] = side1Offset + 2 * i;
				side1 [6 * i + 1] = side1Offset + 2 * i + 2;
				side1 [6 * i + 2] = side1Offset + 2 * i + 1;
				side1 [6 * i + 3] = side1Offset + 2 * i + 1;
				side1 [6 * i + 4] = side1Offset + 2 * i + 2;
				side1 [6 * i + 5] = side1Offset + 2 * i + 3;
			}
			return side1;
		}

		private static int[] MakeSide2Tris (int side2Offset, Vector3[] topVerts){
			int[] side2 = new int[(topVerts.Length - 2) * 3];
			for (int i = 0; i < (topVerts.Length-2)/2; i++) {
				side2 [6 * i] = side2Offset + 2 * i;
				side2 [6 * i + 1] = side2Offset + 2 * i + 1;
				side2 [6 * i + 2] = side2Offset + 2 * i + 2;
				side2 [6 * i + 3] = side2Offset + 2 * i + 2;
				side2 [6 * i + 4] = side2Offset + 2 * i + 1;
				side2 [6 * i + 5] = side2Offset + 2 * i + 3;
			}
			return side2;
		}
		public static void calculateMeshTangents(Mesh mesh)
		{
			//speed up math by copying the mesh arrays
			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uv = mesh.uv;
			Vector3[] normals = mesh.normals;
			
			//variable definitions
			int triangleCount = triangles.Length;
			int vertexCount = vertices.Length;
			
			Vector3[] tan1 = new Vector3[vertexCount];
			Vector3[] tan2 = new Vector3[vertexCount];
			
			Vector4[] tangents = new Vector4[vertexCount];
			
			for (long a = 0; a < triangleCount; a += 3)
			{
				long i1 = triangles[a + 0];
				long i2 = triangles[a + 1];
				long i3 = triangles[a + 2];
				
				Vector3 v1 = vertices[i1];
				Vector3 v2 = vertices[i2];
				Vector3 v3 = vertices[i3];
				
				Vector2 w1 = uv[i1];
				Vector2 w2 = uv[i2];
				Vector2 w3 = uv[i3];
				
				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;
				float z1 = v2.z - v1.z;
				float z2 = v3.z - v1.z;
				
				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;
				
				float r = 1.0f / (s1 * t2 - s2 * t1);
				
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
				
				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;
				
				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}
			
			
			for (long a = 0; a < vertexCount; ++a)
			{
				Vector3 n = normals[a];
				Vector3 t = tan1[a];
				
				//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
				//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
				Vector3.OrthoNormalize(ref n, ref t);
				tangents[a].x = t.x;
				tangents[a].y = t.y;
				tangents[a].z = t.z;
				
				tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			}
			
			mesh.tangents = tangents;
		}
}
