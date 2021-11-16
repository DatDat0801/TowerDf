using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class GachaSpellDataBase
    {
        public int id;
        public int numberKey;
        public int moneyTypeKey;
        public int costExchange;
        public int moneyTypeExchange;
        public float timeCountdownFree;
    }

    [Serializable]
    public class GachaSpellRateData
    {
        public int rarity;
        public float rate;
    }

    public class GachaSpellData : ScriptableObject
    {
        public GachaSpellDataBase[] gachaSpellDataBases;
        public GachaSpellRateData[] gachaSpellRateDatas;

#if UNITY_EDITOR
        public static T[] LoadAssetRateData<T>(string csvFormat, string nameCsv)
        {
            string rateCsv = $"{csvFormat}/{nameCsv}.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(rateCsv);

            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }
}