using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class GloryRoadTierItem
    {
        public int tierId;
        public Reward[] rewards;
        public int[] criticalRewards;
        public int point;

        public Reward GetFreeReward()
        {
            // if (tierId == tierQuantity)
            // {
            //     return rewards[0];
            // }
            if (rewards.Length > 1)
            {
                return rewards[0];
            }

            return null;
        }

        public Reward[] GetPremiumReward(bool ownHero1003)
        {
            switch (rewards.Length)
            {
                case 2:
                    return new []{rewards[1]} ;
                case 1:
                    return new []{rewards[0]};
                case 3:
                    return ownHero1003 ? new []{rewards[1]}  : new []{rewards[2]} ;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public Reward[] GeneratePremiumRewards(bool ownHero1003)
        {
            switch (rewards.Length)
            {
                case 2:
                    return Reward.GenRewards(new []{rewards[1]});
                case 1:
                    return Reward.GenRewards(new []{rewards[0]});
                case 3:
                    //var test = Reward.GenRewards(new []{rewards[1]});
                    return ownHero1003 ? new []{rewards[1]} : Reward.GenRewards(new []{rewards[2]});
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public bool IsCriticalFreeReward()
        {
            if (criticalRewards.Length == 1)
                return false;
            return criticalRewards[0] > 0;
        }

        public bool IsCriticalPremiumReward()
        {
            if (criticalRewards.Length == 1)
                return criticalRewards[0] > 0;
            return criticalRewards[1] > 0;
        }
    }

    public class GloryRoadDatabase : ScriptableObject
    {
        public GloryRoadTierItem[] items;

        public bool IsCriticalFreeReward(int tierId)
        {
            var reward = Array.Find(items, item => item.tierId == tierId);
            if (tierId == items.Length)
            {
                return false;
            }

            return reward.criticalRewards[0] > 0;
        }

        public bool IsCriticalPremiumReward(int tierId)
        {
            var reward = Array.Find(items, item => item.tierId == tierId);
            if (tierId == items.Length)
            {
                //The last reward have only 1 reward
                return reward.criticalRewards[0] > 0;
            }

            return reward.criticalRewards[1] > 0;
        }

        public int GetGloryDataPoint(int tierId)
        {
            if (tierId == 0)
            {
                return items[tierId].point;
            }

            return items[tierId - 1].point;
        }
    }
#if UNITY_EDITOR

    public class GloryRoadDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/HeroAcademy/GloryRoad/";
            string csvDataBase = $"{csvFormat}glory_road.csv";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(GloryRoadDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/GloryRoad/" + nameAsset;
                    GloryRoadDatabase gm = AssetDatabase.LoadAssetAtPath<GloryRoadDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<GloryRoadDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    var rewardedAdEntity = CsvReader.Deserialize<GloryRoadTierItem>(data.text);

                    gm.items = rewardedAdEntity;

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}