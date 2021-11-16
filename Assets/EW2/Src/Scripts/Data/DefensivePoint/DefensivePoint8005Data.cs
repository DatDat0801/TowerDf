using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class DefensivePoint8005Data : DefensivePointData
    {
        public DefensivePoint8005Passive[] passiveStats;
    }
    [Serializable]
    public class DefensivePoint8005Passive
    {
        public int attackTime;
        public float strikeDamage;
        public float range;
        public int maxTargets;
        public float decreaseRatioPerTarget;
    }
#if UNITY_EDITOR
    public class DefensivePoint8005DataPostprocessor : AssetPostprocessor
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
                    var defensivePointId = "8005";

                    // get asset file
                    string nameAsset = $"DefensivePoint{defensivePointId}Data" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    DefensivePoint8005Data gm = AssetDatabase.LoadAssetAtPath<DefensivePoint8005Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DefensivePoint8005Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/defensive_point_{defensivePointId}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<DefensivePointStatBase>(data.text);
                    gm.passiveStats =CsvReader.Deserialize<DefensivePoint8005Passive>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}