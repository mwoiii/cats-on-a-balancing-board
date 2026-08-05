#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using OMC;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(WeightTypeRegistry))]
public class WeightDropperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty config = serializedObject.FindProperty("weightTypeConfig");
        EditorGUILayout.PropertyField(config);

        TextAsset csv = config.objectReferenceValue as TextAsset; // Wow

        EditorGUILayout.Space();

        if (csv == null){EditorGUILayout.HelpBox("Come back with a csv file nerd",MessageType.Info);}
        else
        {
            List<WeightTypeConfigRow> rows = WeightTypeConfigCSV.Parse(csv.text);
            SerializedProperty slots = serializedObject.FindProperty("weightPrefabSlots");

            EditorGUILayout.LabelField("Weight Prefabs", EditorStyles.boldLabel);

            foreach (var row in rows)
            {
                EditorGUILayout.LabelField($"{row.typeName}");
                EditorGUI.indentLevel++;

                foreach(string shape in row.shapes)
                {
                    int i = SmartGetIndex(slots,row.typeName,shape);
                    var slot = slots.GetArrayElementAtIndex(i);
                    var prefab = slot.FindPropertyRelative("prefab");
                    EditorGUILayout.PropertyField(prefab,new GUIContent($"{row.typeName} {shape} Prefab"));
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script","weightTypeConfig","weightPrefabSlots");
        serializedObject.ApplyModifiedProperties();
    }

    int SmartGetIndex(SerializedProperty slots, string typeName, string shape)
    {
        for (int i = 0; i < slots.arraySize; i++)
        {
            var slot = slots.GetArrayElementAtIndex(i);
            if (slot.FindPropertyRelative("typeName").stringValue == typeName && slot.FindPropertyRelative("shapeName").stringValue == shape){return i;}
        }

        int j = slots.arraySize;
        slots.InsertArrayElementAtIndex(j);

        var newSlot = slots.GetArrayElementAtIndex(j);
        newSlot.FindPropertyRelative("typeName").stringValue = typeName;
        newSlot.FindPropertyRelative("shapeName").stringValue = shape;
        newSlot.FindPropertyRelative("prefab").objectReferenceValue = null;
        return j;
    }
}
#endif