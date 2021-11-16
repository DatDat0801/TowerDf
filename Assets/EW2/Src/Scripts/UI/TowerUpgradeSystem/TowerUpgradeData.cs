using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TowerUpgradeData : ScriptableObject
    {
        //public int type;//MoneyType
        public GradientUnit[] gradientUnits;
        public Tower2001BonusStat[] bonusStat2001;
        public Tower2002BonusStat[] bonusStat2002;
        public Tower2003BonusStat[] bonusStat2003;
        public Tower2004BonusStat[] bonusStat2004;
        public int resetCost;


        /// <summary>
        /// all
        /// </summary>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        public int GetNeededStar(int currencyType)
        {
            int r = 0;
            for (var i = 0; i < gradientUnits.Length; i++)
            {
                if (gradientUnits[i].currencyType == currencyType)
                {
                    r += gradientUnits[i].currencyQuantity.Sum();
                }
            }

            return r;
        }
        /// <summary>
        /// Get the needed star to upgrade
        /// </summary>
        /// <param name="starType">basically starType=8 is silver, 9 is golden</param>
        /// <param name="towerId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetStarQuantity(int starType, int towerId, int level)
        {
            try
            {
                var gradient = Array.Find(gradientUnits,
                    unit => unit.towerId == towerId && unit.currencyType == starType);
                var index = Array.FindIndex(gradient.levels, i => i == level);
                return gradient.currencyQuantity[index];
            }
            catch(Exception e)
            {
                return 0;
            }

            
        }

        public Tower2001BonusStat GetBonusStat2001ByLevel(int level)
        {
            try
            {
                return Array.Find(bonusStat2001, stat => stat.level == level);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        public Tower2002BonusStat GetBonusStat2002ByLevel(int level)
        {
            try
            {
                return Array.Find(bonusStat2002, stat => stat.level == level);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        public Tower2003BonusStat GetBonusStat2003ByLevel(int level)
        {
            try
            {
                return Array.Find(bonusStat2003, stat => stat.level == level);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        public Tower2004BonusStat GetBonusStat2004ByLevel(int level)
        {
            try
            {
                return Array.Find(bonusStat2004, stat => stat.level == level);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public Tower2001BonusStat GetTotalBonusStat2001(int level)
        {
            var totalStat = new Tower2001BonusStat();
            for (int i = 1; i <= level; i++)
            {
                totalStat += GetBonusStat2001ByLevel(i);
            }

            return totalStat;
        }
        
        public Tower2002BonusStat GetTotalBonusStat2002(int level)
        {
            var totalStat = new Tower2002BonusStat();
            for (int i = 1; i <= level; i++)
            {
                totalStat += GetBonusStat2002ByLevel(i);
            }

            return totalStat;
        }
        
        public Tower2003BonusStat GetTotalBonusStat2003(int level)
        {
            var totalStat = new Tower2003BonusStat();
            for (int i = 1; i <= level; i++)
            {
                totalStat += GetBonusStat2003ByLevel(i);
            }

            return totalStat;
        }
        
        public Tower2004BonusStat GetTotalBonusStat2004(int level)
        {
            var totalStat = new Tower2004BonusStat();
            for (int i = 1; i <= level; i++)
            {
                totalStat += GetBonusStat2004ByLevel(i);
            }

            return totalStat;
        }
    }

    [Serializable]
    public class GradientUnit
    {
        public int currencyType;
        public int towerId;
        public int[] levels;
        public int[] currencyQuantity;
    }

#if UNITY_EDITOR
    public class TowerUpgradeDataPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/TowerUpgradeSystem";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TowerUpgradeData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/UpgradeTowerSystem/" + nameAsset;
                    TowerUpgradeData gm = AssetDatabase.LoadAssetAtPath<TowerUpgradeData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerUpgradeData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get tower 2001 bonus stats
                    TextAsset textData2001 = GetTextAsset(2001);
                    gm.bonusStat2001 = CsvReader.Deserialize<Tower2001BonusStat>(textData2001.text);
                    var textData2002 = GetTextAsset(2002);
                    gm.bonusStat2002 = CsvReader.Deserialize<Tower2002BonusStat>(textData2002.text);
                    var textData2003 = GetTextAsset(2003);
                    gm.bonusStat2003 = CsvReader.Deserialize<Tower2003BonusStat>(textData2003.text);
                    var textData2004 = GetTextAsset(2004);
                    gm.bonusStat2004 = CsvReader.Deserialize<Tower2004BonusStat>(textData2004.text);

                    string nameBaseCsv = $"{csvFormat}/tower_gradient_data.csv";
                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.gradientUnits = CsvReader.Deserialize<GradientUnit>(data.text);
                    //
                    // gm.allStatsLevel = CsvReader.Deserialize<EnemyStatBase>(data.text);
                    //
                    // gm.allSkill1 = EnemyData.LoadAsset<EnemyData3016.EnemyData3016Skill1>(csvFormat, enemyId, 1);
                    // gm.allSkill2 = EnemyData.LoadAsset<EnemyData3016.EnemyData3016Skill2>(csvFormat, enemyId, 2);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }

        private static TextAsset GetTextAsset(int towerId)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/TowerUpgradeSystem";
            string nameBaseCsv = $"{csvFormat}/tower_{towerId}_bonus_stats.csv";
            TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
            return data;
        }
    }
#endif
}