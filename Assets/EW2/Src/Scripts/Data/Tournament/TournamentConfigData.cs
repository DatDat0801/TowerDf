using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TournamentConfigData : ScriptableObject
    {
        public TournamentConfig[] configs;

        public TournamentConfig GetTournamentConfig()
        {
            return this.configs[0];
        }
    }
    [Serializable]
    public struct TournamentConfig
    {
        public int mapUnlock;
    }
#if UNITY_EDITOR

    public class TournamentConfigPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament/tournament_config.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TournamentConfigData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tournament/" + nameAsset;
                    TournamentConfigData gm = AssetDatabase.LoadAssetAtPath<TournamentConfigData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TournamentConfigData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.configs = CsvReader.Deserialize<TournamentConfig>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}