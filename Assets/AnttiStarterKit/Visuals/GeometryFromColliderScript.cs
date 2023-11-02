using UnityEngine;

namespace AnttiStarterKit.Visuals
{
	public class GeometryFromColliderScript : MonoBehaviour {

		public float outlineOffset = 0f;

		public float meshDepth = 0f;
		public float outlineDepth = 0f;
	
		public void GenerateGeometry() {
			MeshFilter mf = GetComponent<MeshFilter> ();
			LineRenderer lr = GetComponent<LineRenderer> ();

			PolygonCollider2D poly = GetComponent<PolygonCollider2D> ();

			Mesh mesh = new Mesh();

			// Create the Vector3 vertices
			Vector3[] vertices = new Vector3[poly.points.Length];
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i] = new Vector3(poly.points[i].x, poly.points[i].y, meshDepth);
			}

			Triangulator tr = new Triangulator(poly.points);
			int[] indices = tr.Triangulate();

			mesh.vertices = vertices;
			mesh.triangles = indices;

			if (mf) {
				mf.mesh = mesh;
			}

			for (int i = 0; i < vertices.Length; i++) {
				int prev = (i > 0) ? i - 1 : vertices.Length - 1;
				int next = (i < vertices.Length - 1) ? i + 1 : 0;

				Vector3 from = (vertices [i] - vertices [prev]).normalized;
				Vector3 to = (vertices [i] - vertices [next]).normalized;
				Vector3 norm = Quaternion.Euler(new Vector3(0, 0, 90)) * (to - from).normalized;

//			Debug.DrawRay (transform.position + vertices [i], norm, Color.red, 0.5f);

				vertices [i] += norm * outlineOffset;
				vertices [i].z = outlineDepth;
			}

			if (lr) {
				lr.useWorldSpace = false;
				lr.loop = true;
				lr.positionCount = vertices.Length;
				lr.SetPositions (vertices);
			}
		}
	}
}
