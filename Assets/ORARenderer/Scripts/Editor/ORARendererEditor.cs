using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ORARenderer))]
public class ORARendererEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ORARenderer script = (ORARenderer)target;

		if (GUILayout.Button("Read Files", GUILayout.Height(40)))
			script.ReadFiles();
	}
}
