using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MugenForever.Snd.Snd))]
public class SndEditor: Editor
{
    int group = 0;
    int sample = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MugenForever.Snd.Snd snd = (MugenForever.Snd.Snd)target;

        AudioSource aSource = snd.gameObject.GetComponent<AudioSource>();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Test Play", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Group");
        group = EditorGUILayout.IntField(group);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sample");
        sample = EditorGUILayout.IntField(sample);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Play"))
        {

            SndClip sndClip = snd.soundList[group][sample];

            if (sndClip == null)
            {
                EditorUtility.DisplayDialog("Error", "Impossible to FIND sound!", "ok");
            }

            if (aSource != null )
            {
                aSource.PlayOneShot(sndClip.clip);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Impossible to PLAY sound!", "ok");
            }
            //attr.Calculate();
        }

        GUILayout.Space(10);
    }
}