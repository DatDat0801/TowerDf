using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2.Spell
{
    [Serializable]
    public class SpellStatData
    {
        public int id;

        public int rarity;
    }

    [Serializable]
    public class SpellUpgradeData
    {
        public int level;
        public long cost;
        public int moneyType;
        public int reqFragment;
    }

    [Serializable]
    public class SpellDataBase
    {
        public SpellStatData spellStatDatas;

        public SpellUpgradeData[] spellUpgradeDatas;
    }

    public class SpellDataBases : ScriptableObject
    {
        public SpellDataBase[] spellDataBases;

        public SpellDataBase GetSpellDataBase(int spellId)
        {
            foreach (var spellDataBase in spellDataBases)
            {
                if (spellDataBase.spellStatDatas.id == spellId)
                    return spellDataBase;
            }

            return null;
        }

        public int GetSpellLevelMax(int spellId)
        {
            foreach (var spellDataBase in spellDataBases)
            {
                if (spellDataBase.spellStatDatas.id == spellId)
                    return spellDataBase.spellUpgradeDatas.Length;
            }

            return 0;
        }

        public SpellUpgradeData GetSpellDataUpgrade(int spellId, int level)
        {
            foreach (var spellDataBase in spellDataBases)
            {
                if (spellDataBase.spellStatDatas.id == spellId)
                {
                    if (level >= spellDataBase.spellUpgradeDatas.Length)
                        return spellDataBase.spellUpgradeDatas[spellDataBase.spellUpgradeDatas.Length - 1];
                    else
                        return spellDataBase.spellUpgradeDatas[level - 1];
                }
            }

            return null;
        }

        public Dictionary<int, List<int>> GetDictSpellByRarity()
        {
            var results = new Dictionary<int, List<int>>();
            foreach (var spellDataBase in spellDataBases)
            {
                if (!results.ContainsKey(spellDataBase.spellStatDatas.rarity))
                {
                    results.Add(spellDataBase.spellStatDatas.rarity, new List<int>() {spellDataBase.spellStatDatas.id});
                }
                else
                {
                    results[spellDataBase.spellStatDatas.rarity].Add(spellDataBase.spellStatDatas.id);
                }
            }

            return results;
        }

        public int GetTotalFragmentToLevelMax(int spellId)
        {
            var total = 0;
            foreach (var spellDataBase in spellDataBases)
            {
                if (spellDataBase.spellStatDatas.id == spellId)
                {
                    foreach (var dataUpgrade in spellDataBase.spellUpgradeDatas)
                    {
                        total += dataUpgrade.reqFragment;
                    }
                }
            }

            return total;
        }

        public int GetTotalFragmentUsed(int spellId, int level)
        {
            var total = 0;
            foreach (var spellDataBase in spellDataBases)
            {
                if (spellDataBase.spellStatDatas.id == spellId)
                {
                    for (int i = 0; i < spellDataBase.spellUpgradeDatas.Length; i++)
                    {
                        if (i < level)
                            total += spellDataBase.spellUpgradeDatas[i].reqFragment;
                        else break;
                    }
                }
            }

            return total;
        }
#if UNITY_EDITOR
        public static T[] LoadAssetUpgradeData<T>(string csvFormat, int spellId)
        {
            string skillCsv = $"{csvFormat}/spell_{spellId}_upgrade.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(skillCsv);

            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }

#if UNITY_EDITOR

    public class SpellDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/";
            string csvDataBase = $"{csvFormat}spell_base.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(SpellDataBases) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    SpellDataBases gm = AssetDatabase.LoadAssetAtPath<SpellDataBases>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<SpellDataBases>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    var spellStatDataEditor = CsvReader.Deserialize<SpellStatData>(data.text);

                    gm.spellDataBases = new SpellDataBase[spellStatDataEditor.Length];
                    for (var i = 0; i < spellStatDataEditor.Length; i++)
                    {
                        var spellDataBase = new SpellDataBase();
                        spellDataBase.spellStatDatas = spellStatDataEditor[i];
                        spellDataBase.spellUpgradeDatas =
                            SpellDataBases.LoadAssetUpgradeData<SpellUpgradeData>(csvFormat, spellStatDataEditor[i].id);

                        gm.spellDataBases[i] = spellDataBase;
                    }

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}