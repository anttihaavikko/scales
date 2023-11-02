using UnityEditor;
using UnityEngine;
using AnttiStarterKit.Visuals;

namespace Editor
{
	[CustomEditor(typeof(BoxOutlinerScript))]
	public class BoxOutlinerEditor : UnityEditor.Editor {

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var myScript = (BoxOutlinerScript)target;
			if(GUILayout.Button("Fix outline"))
			{
				myScript.DoOutline();
			}
		}
	}
}
