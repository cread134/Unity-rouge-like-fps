using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(LayoutClass))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class LayoutCustomInspector : UnityEditor.Editor
{
    SerializedProperty roomsList;
    SerializedObject GetTarget;

    LayoutClass targetLayout;
    int ListSize;
    private void OnEnable()
    {
        targetLayout = (LayoutClass)target;
        GetTarget = new SerializedObject(targetLayout);
        roomsList = GetTarget.FindProperty("rooms");
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        GetTarget.Update();

        Undo.RecordObject(targetLayout, "UndoLayoutChange"); // allow for undo

        EditorGUILayout.LabelField("Floor Layout", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Add Room"))
        {
            targetLayout.rooms.Add(new LayoutClass.Room());
        }

        if (GUILayout.Button("Clear Rooms"))
        {
            targetLayout.rooms.Clear();
            targetLayout.rooms = new List<LayoutClass.Room>();
        }

        //EditorGUILayout.PropertyField(roomsList, new GUIContent("Rooms"));
        //render list
        #region

        //renders size of list
        ListSize = roomsList.arraySize;
        ListSize = EditorGUILayout.IntField("List Size", ListSize);
        #endregion

        EditorGUILayout.PropertyField(roomsList);

        GetTarget.ApplyModifiedProperties(); //apply properties
    }
}
