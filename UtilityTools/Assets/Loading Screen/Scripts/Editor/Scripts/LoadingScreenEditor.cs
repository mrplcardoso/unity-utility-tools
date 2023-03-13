using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds Inspector GUI for LoadingScreen components
/// </summary>
[CustomEditor(typeof(LoadingScreen), true), CanEditMultipleObjects]
public class LoadingScreenEditor : Editor
{
	protected SerializedProperty debugProgress;
	protected SerializedProperty loadProgressType;
	protected SerializedProperty showPorcentage;
	protected SerializedProperty showLoading;
	protected SerializedProperty waitForInput;
	protected SerializedProperty minDuration;

	protected virtual void OnEnable()
	{
		debugProgress = serializedObject.FindProperty("debugProgress");
		loadProgressType = serializedObject.FindProperty("loadProgressType");
		showPorcentage = serializedObject.FindProperty("showPorcentage");
		showLoading = serializedObject.FindProperty("showLoading");
		waitForInput = serializedObject.FindProperty("waitForInput");
		minDuration = serializedObject.FindProperty("minDuration");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(loadProgressType, new GUIContent("Progress Type"));
		switch (loadProgressType.enumValueIndex)
		{
			case 1:
			{
				EditorGUILayout.PropertyField(showPorcentage);
				EditorGUILayout.PropertyField(showLoading);
				break;
			}
			case 2:
			{
				EditorGUILayout.PropertyField(showPorcentage);
				EditorGUILayout.PropertyField(showLoading);
				break;
			}
		}

		EditorGUILayout.PropertyField(debugProgress);
		EditorGUILayout.PropertyField(waitForInput);
		EditorGUILayout.PropertyField(minDuration, new GUIContent("Min. screen duration"));

		serializedObject.ApplyModifiedProperties();
	}
}
