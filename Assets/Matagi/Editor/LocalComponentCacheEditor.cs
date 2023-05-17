// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace Matagi.Editor
{
    [CustomEditor(typeof(LocalComponentCache), true), CanEditMultipleObjects]
    public class LocalComponentCacheEditor : UnityEditor.Editor
    {
        private Vector2 _scrollPosition = new(0, 0);

        public override void OnInspectorGUI()
        {
            var r = target as LocalComponentCache;

            if (r == null) return;
            r.defaultColor = EditorGUILayout.ColorField("Visualize color", r.defaultColor);
            r.visualize = GUILayout.Toggle(r.visualize, "Visualize", EditorStyles.miniButton);
            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                foreach (var keyValuePair in r.VisualizeCacheDictionary)
                {
                    var id = keyValuePair.Key;
                    var obj = EditorUtility.InstanceIDToObject(id);
                    EditorGUILayout.ObjectField(obj, obj.GetType(), allowSceneObjects: false);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical();
                    {
                        foreach (var component in keyValuePair.Value)
                        {
                            EditorGUILayout.ObjectField(component.gameObject, component.gameObject.GetType(),
                                allowSceneObjects: false);
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}