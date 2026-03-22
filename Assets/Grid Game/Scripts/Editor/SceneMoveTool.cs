
using System;
using UnityEditor;
using UnityEngine;


public class SceneMoveTool : EditorWindow
{
    private GameObject focusTarget;
    private bool followTarget;
    private bool lookAtTarget;
    
    [MenuItem("Tools/SceneMoveTool")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<SceneMoveTool>();
        wnd.titleContent = new GUIContent("Scene Move Tool");
    }


    private void OnGUI()
    {
        focusTarget = (GameObject)EditorGUILayout.ObjectField("Target", focusTarget, typeof(GameObject), true);
        followTarget = EditorGUILayout.Toggle("Follow target", followTarget);
        lookAtTarget = EditorGUILayout.Toggle("LookAtTarget", lookAtTarget);

    }
}
