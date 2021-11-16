using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroUnlockConditionData : ScriptableObject
    {
        public HeroCondition[] heroConditions;
    }
    [System.Serializable]
    public class HeroCondition
    {
        public int heroId;
        public int unlockStage;
    }
#if UNITY_EDITOR

    public class HeroUnlockCondition : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
           string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/hero_unlock_condition.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroUnlockConditionData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    HeroUnlockConditionData gm = AssetDatabase.LoadAssetAtPath<HeroUnlockConditionData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroUnlockConditionData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.heroConditions = CsvReader.Deserialize<HeroCondition>(data.text);
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
