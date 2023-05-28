using MugenForever.IO.PAL;
using UnityEditor;
using UnityEngine;

namespace MugenForever.Editor
{
    [CustomEditor(typeof(PaletteImpl))]
    public class PaletteImplEditor : UnityEditor.Editor
    {
        public int line = 16;
        PaletteImpl palette;
        Color32[] colors;

        public void OnEnable()
        {
            palette = (PaletteImpl)target;
            colors = palette.PalleteColor;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty _paletteColor = serializedObject.FindProperty("_paletteColor");

            EditorGUILayout.PropertyField(_paletteColor.FindPropertyRelative("Array.size"));
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < _paletteColor.arraySize; i++)
            {
                bool newLine = i % line == 0 && i > 0;

                if (newLine)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                Color32 color = colors[i];
                GUILayoutOption[] options = new GUILayoutOption[1];
                options[0] = GUILayout.Width(16);

                colors[i] = EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, options);
            }

            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
