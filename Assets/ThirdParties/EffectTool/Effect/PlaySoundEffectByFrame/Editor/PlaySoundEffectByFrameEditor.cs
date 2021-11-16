using UnityEditor;
using UnityEngine;

namespace EW2
{
    [CustomEditor(typeof(PlaySoundEffectByFrame))]
    public class PlaySoundEffectByFrameEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PlaySoundEffectByFrame effectPlayer = (PlaySoundEffectByFrame)target;
            //GUILayout.Space(10);
            //if (GUILayout.Button("Revert Sfx Config", GUILayout.Height(30)))
            //{
            //    Undo.RecordObject(effectPlayer, "Revert Sfx Config");
            //    effectPlayer.RevertConfig(Config.xluaUrl);
            //    PrefabUtility.RecordPrefabInstancePropertyModifications(effectPlayer);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();
            //}
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Preview Animation", GUILayout.Height(30)))
            {
                effectPlayer.PreviewAnimation();
            }

            //GUILayout.Space(20);
            //if (GUILayout.Button("Gen Csv Config", GUILayout.Height(30)))
            //{
            //    GenCsvFile(effectPlayer.fileName, effectPlayer);
            //}
        }
    }
}
