using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class NewHeroEventCondition : ScriptableObject
    {
        public EventCondition[] eventConditions;

        [Serializable]
        public class EventCondition
        {
            public int mapUnlock;
            public int duration;
        }
    }

#if UNITY_EDITOR

    public class NewHeroEventConditionPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/NewHero/new_hero_condition.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(NewHeroEventCondition) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    NewHeroEventCondition gm = AssetDatabase.LoadAssetAtPath<NewHeroEventCondition>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<NewHeroEventCondition>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.eventConditions = CsvReader.Deserialize<NewHeroEventCondition.EventCondition>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}