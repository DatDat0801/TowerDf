using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class RerollDataConfig
    {
        public int priceChangeHero;
        public int valueIncrease;
        public int typeMoneyChangeHero;
        public int priceChangeBuff;
        public int typeMoneyChangeBuff;
    }

    public class RerollHeroBuffConfig : ScriptableObject
    {
        public RerollDataConfig[] rerollDataConfigs;
    }

#if UNITY_EDITOR

    public class RerollHeroBuffConfigPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament/reroll_hero_buff_config.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RerollHeroBuffConfig) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tournament/" + nameAsset;
                    RerollHeroBuffConfig gm = AssetDatabase.LoadAssetAtPath<RerollHeroBuffConfig>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RerollHeroBuffConfig>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.rerollDataConfigs = CsvReader.Deserialize<RerollDataConfig>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}