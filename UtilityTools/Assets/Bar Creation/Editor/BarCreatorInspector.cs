using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BarCreator))]
public class BarCreatorInspector : Editor
{
	GUIStyle boldText = new GUIStyle();
	int prefabIndex;
	int tiles;
	public BarCreator current
	{
		get
		{
			return (BarCreator)target;
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		EditorGUILayout.Space();
		Inputs();
		EditorGUILayout.Space();
		Buttons();
	}

	void Inputs()
	{
		boldText.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Configure a new bar:", boldText);
		prefabIndex = EditorGUILayout.IntField(
			new GUIContent("Prefab ID", "Indicates the prefab from HolderList to be cloned"), prefabIndex);
		tiles = EditorGUILayout.IntField(
			new GUIContent("Numer of tiles", "How many times the bar structure will repeat"), tiles);
	}

	void Buttons()
	{
		if (GUILayout.Button("Create Bar"))
		{ current.CreateBar(prefabIndex, tiles); }

		if (GUILayout.Button("Clear"))
		{ current.Clear(); }
	}
}