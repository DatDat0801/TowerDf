using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class DefensivePoint8004Data : DefensivePointData
    {
        public DefensivePoint8004Passive[] passiveStats;
    }
    [Serializable]
    public class DefensivePoint8004Passive
    {
        public float increaseHp;
        public float internalTime;
    }
#if UNITY_EDITOR
    public class DefensivePoint8004DataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/DefensivePoint";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    var defensivePointId = "8004";

                    // get asset file
                    string nameAsset = $"DefensivePoint{defensivePointId}Data" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    DefensivePoint8004Data gm = AssetDatabase.LoadAssetAtPath<DefensivePoint8004Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DefensivePoint8004Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/defensive_point_{defensivePointId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<DefensivePointStatBase>(data.text);
                    gm.passiveStats =CsvReader.Deserialize<DefensivePoint8004Passive>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}