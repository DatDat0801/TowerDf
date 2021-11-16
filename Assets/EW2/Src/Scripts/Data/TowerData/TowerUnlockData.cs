using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class TowerUnlock
    {
        public enum LevelType
        {
            Level,
            Skill
        }

        /// <summary>
        /// stage that to be unlocked this level
        /// </summary>
        public int stage;

        public LevelType type;

        public int level;

        // /// <summary>
        // /// upgrade branch, 0 is center, 1 is left, 2 is right
        // /// </summary>
        public int branch;
    }

    public class TowerUnlockData : ScriptableObject
    {
        public TowerUnlock[] unlocks;
    }
#if UNITY_EDITOR
    public class TowerUnlockDataPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/tower_unlock_data.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TowerUnlockData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerUnlockData gm = AssetDatabase.LoadAssetAtPath<TowerUnlockData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerUnlockData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    // // get stat base
                    // string nameBaseCsv = $"{csvFormat}/tower_{towerId}_base.csv";
                    //
                    // TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    //
                    // gm.id = 2001;
                    //
                    gm.unlocks = CsvReader.Deserialize<TowerUnlock>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}