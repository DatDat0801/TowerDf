using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class LevelEnhanceData
    {
        public int level;
        public int dustReq;
        public int crystalReq;
    }

    [Serializable]
    public class RuneEnhanceData
    {
        public int rarity;
        public LevelEnhanceData[] levelEnhanceDatas;

        public LevelEnhanceData GetLevelEnhanceData(int level)
        {
            if (level <= levelEnhanceDatas.Length)
                return levelEnhanceDatas[level - 1];
            return levelEnhanceDatas[levelEnhanceDatas.Length - 1];
        }
    }

    public class RuneEnhanceDatabase : ScriptableObject
    {
        public RuneEnhanceData[] runeEnhanceDatas;

        public RuneEnhanceData GetRuneEnhanceData(int rarity)
        {
            foreach (var enhanceData in runeEnhanceDatas)
            {
                if (enhanceData.rarity == rarity)
                    return enhanceData;
            }

            return null;
        }
    }

#if UNITY_EDITOR

    public class RuneEnhanceDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Runes/";
            string csvDataBase = $"{csvFormat}rune_enhance_data.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RuneEnhanceDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    RuneEnhanceDatabase gm = AssetDatabase.LoadAssetAtPath<RuneEnhanceDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RuneEnhanceDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.runeEnhanceDatas = CsvReader.Deserialize<RuneEnhanceData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}