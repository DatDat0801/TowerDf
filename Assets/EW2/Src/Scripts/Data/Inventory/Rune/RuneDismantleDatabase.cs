using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class RuneDismantleData
    {
        public int runeId;
        public int rarityId;
        public int expValue;
    }
    public class RuneDismantleDatabase : ScriptableObject
    {
        public RuneDismantleData[] runeDismantles;

        public RuneDismantleData GetDataBy(int runeId, int rarityId)
        {
            return Array.Find(runeDismantles, o => o.rarityId == rarityId && o.runeId == runeId);
        }

        public int GetTotalExp(List<RuneItem> runeItems)
        {
            try
            {
                int expCount = 0;
                for (var i = 0; i < runeItems.Count; i++)
                {
                    if (runeItems[i].Level > 1)
                    {
                        expCount += TotalExpOfUpgradedRune(runeItems[i]);
                        continue;
                    }
                    var id = InventoryDataBase.GetRuneId(runeItems[i].RuneIdConvert);
                    var runeData = Array.Find(runeDismantles, o => o.rarityId == runeItems[i].Rarity && o.runeId == id.Item1);
                    expCount += runeData.expValue;
                }
                return expCount;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return 0;
            }

        }
        private int TotalExpOfUpgradedRune(RuneItem runeItem)
        {
            RuneEnhanceData runeEnhanceData =  GameContainer.Instance.Get<InventoryDataBase>().Get<RuneEnhanceDatabase>()
                   .GetRuneEnhanceData(runeItem.Rarity);
            float dustReqUpgraded = 0;
            float refundRate = GameContainer.Instance.Get<InventoryDataBase>().Get<RuneDismantleRefundDatabase>().runeDismantleRefundData.refundRate;
            for (int i = 1; i < runeItem.Level; i++)
            {
                dustReqUpgraded += runeEnhanceData.GetLevelEnhanceData(i).dustReq;
            }
            var id = InventoryDataBase.GetRuneId(runeItem.RuneIdConvert);
            var runeData = Array.Find(runeDismantles, o => o.rarityId == runeItem.Rarity && o.runeId == id.Item1);
            return (int)(dustReqUpgraded * refundRate) + runeData.expValue;
        }
    }
#if UNITY_EDITOR

    public class RuneDismantleDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Runes/";
            string csvDataBase = $"{csvFormat}rune_dismantle.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RuneDismantleDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    var gm = AssetDatabase.LoadAssetAtPath<RuneDismantleDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RuneDismantleDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.runeDismantles = CsvReader.Deserialize<RuneDismantleData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
