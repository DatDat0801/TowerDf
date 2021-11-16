using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class GachaRuneDataBase
    {
        public int id;
        public int numberKey;
        public int moneyTypeKey;
        public int costExchange;
        public int moneyTypeExchange;
        public float timeCountdownFree;
        public SummonType summonType;
        public int expPerOnce;
    }

    [Serializable]
    public class GachaRuneRateData
    {
        public SummonType summonType;
        public int rarity;
        public float rate;
    }

    public class GachaRuneData : ScriptableObject
    {
        public GachaRuneDataBase[] gachaRuneDataBases;
        public GachaRuneRateData[] gachaRuneRateDatas;

        public GachaRuneDataBase GetDataGacha(int numberGacha, SummonType summonType)
        {
            foreach (var gachaData in gachaRuneDataBases)
            {
                if (gachaData.numberKey == numberGacha && gachaData.summonType == summonType)
                    return gachaData;
            }

            return null;
        }

        public List<GachaRuneRateData> GetDataGachaRate(SummonType summonType)
        {
            var result = new List<GachaRuneRateData>();

            foreach (var gachaDataRate in gachaRuneRateDatas)
            {
                if (gachaDataRate.summonType == summonType)
                    result.Add(gachaDataRate);
            }

            return result;
        }
#if UNITY_EDITOR
        public static T[] LoadAssetRateData<T>(string csvFormat, string nameCsv)
        {
            string rateCsv = $"{csvFormat}/{nameCsv}.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(rateCsv);

            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }

#if UNITY_EDITOR

    public class GachaRuneDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Gacha/";
            string csvDataBase = $"{csvFormat}gacha_rune.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GachaRuneData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Gacha/" + nameAsset;
                    GachaRuneData gm = AssetDatabase.LoadAssetAtPath<GachaRuneData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GachaRuneData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.gachaRuneDataBases = CsvReader.Deserialize<GachaRuneDataBase>(data.text);

                    gm.gachaRuneRateDatas =
                        GachaRuneData.LoadAssetRateData<GachaRuneRateData>(csvFormat, "gacha_rune_drop_rate");

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}