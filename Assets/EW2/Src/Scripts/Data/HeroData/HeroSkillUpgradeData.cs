using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroSkillUpgradeData : ScriptableObject
    {
        public SkillUpgradeData[] skillUpgradeDatas;
        
        [Serializable]
        public class SkillUpgradeData
        {
            public int level;

            public int cost;

            public int typeCurrency;
        }
    }
    
#if UNITY_EDITOR
    public class HeroSkillUpgradeDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/hero_skill_upgrade.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroSkillUpgradeData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/HeroRoom/" + nameAsset;
                    HeroSkillUpgradeData gm = AssetDatabase.LoadAssetAtPath<HeroSkillUpgradeData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroSkillUpgradeData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.skillUpgradeDatas = CsvReader.Deserialize<HeroSkillUpgradeData.SkillUpgradeData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}