using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TreasureSoldier6049Data : ScriptableObject
    {
        public DataTreasureSoldier[] data;

        [System.Serializable]
        public class DataTreasureSoldier
        {
            public int goldReceive;

            public float timeCooldown;

        }

        public DataTreasureSoldier GetDataTreasureSoldier()
        {
            return data[0];
        }
    }

#if UNITY_EDITOR
    public class DataTreasureSoldierPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/treasure_soldier_6049.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TreasureSoldier6049Data) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TreasureSoldier6049Data gm = AssetDatabase.LoadAssetAtPath<TreasureSoldier6049Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TreasureSoldier6049Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.data = CsvReader.Deserialize<TreasureSoldier6049Data.DataTreasureSoldier>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}