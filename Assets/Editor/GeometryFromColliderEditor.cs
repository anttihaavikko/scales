using UnityEditor;
using UnityEngine;
using AnttiStarterKit.Visuals;

namespace Editor
{
	[CustomEditor(typeof(GeometryFromColliderScript))]
	public class GeometryFromColliderEditor : UnityEditor.Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var myScript = (GeometryFromColliderScript)target;

			if(GUILayout.Button("Generate Geometry")) {
				myScript.GenerateGeometry();
			}
		}
	}
}
