using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroDefendModeConfig : ScriptableObject
    {
        public DataConfig[] defendModeDataConfigs;

        [Serializable]
        public class DataConfig
        {
            public int mapId;
            public long seasonDuration;
            public long seasonStartTime;
            public int ticketFree;
            public int numberWatchAds;
            public int numberTrial;
            public int mapUnlock;
            public int numberHeroMinimum;
            public int goldDrop;
            public float tricksterBaseDamage;
        }

        public DataConfig GetDataConfig()
        {
            if (this.defendModeDataConfigs.Length > 0)
                return this.defendModeDataConfigs[0];

            return null;
        }
    }

#if UNITY_EDITOR

    public class HeroDefendModeConfigprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/HeroDefendMode/hero_defend_mode_config.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroDefendModeConfig) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/HeroDefendMode/" + nameAsset;

                    // data orgin
                    HeroDefendModeConfig gm = AssetDatabase.LoadAssetAtPath<HeroDefendModeConfig>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroDefendModeConfig>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.defendModeDataConfigs = CsvReader.Deserialize<HeroDefendModeConfig.DataConfig>(data.text);

                    EditorUtility.SetDirty(gm);

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}