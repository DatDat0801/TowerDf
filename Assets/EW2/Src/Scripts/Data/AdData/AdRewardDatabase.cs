using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Zitga.CsvTools;

namespace EW2
{
    public class AdRewardDatabase : ScriptableObject
    {
        public AdEntity[] ads;

        public AdEntity GetAdEntity(int id)
        {
            var e = Array.Find(ads, entity => entity.adId == id);
            Assert.IsNotNull(e, "e != null, check scriptable object AdRewardDatabase");
            return e;
        }
        
    }
#if UNITY_EDITOR

    public class AdRewardDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/RewardedAd/";
            string csvDataBase = $"{csvFormat}rewarded_ad.csv";
            
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(AdRewardDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/RewardedAd/" + nameAsset;
                    AdRewardDatabase gm = AssetDatabase.LoadAssetAtPath<AdRewardDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<AdRewardDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    var rewardedAdEntity = CsvReader.Deserialize<AdEntity>(data.text);

                    gm.ads = rewardedAdEntity;

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}