using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class TowerRaiseCost
    {
        public int id;
        public int[] raiseLevelCost = new int[3];
        public int[] raiseSkill1Cost = new int[3];
        public int[] raiseSkill2Cost = new int[1];

        public int BuildCost => raiseLevelCost[0];

        public int GetLevelCost(int level)
        {
            try
            {
                return raiseLevelCost[level - 1];
            }
            catch
            {
                return 0;
            }
        }
        
        public int GetSkillCost(BranchType branchType, int level)
        {
            try
            {
                if (branchType == BranchType.Skill1)
                {
                    return raiseSkill1Cost[level];
                }

                return raiseSkill2Cost[level];
            }
            catch
            {
                return 0;
            }
        }

        public int GetSellCost(int raiseLevel, int skill1Level, int skill2Level)
        {
            var total = 0;
            for (int i = 0; i < raiseLevel; i++)
            {
                total += raiseLevelCost[i];
            }

            if (raiseLevel == raiseLevelCost.Length)
            {
                for (int j = 0; j < skill1Level; j++)
                {
                    total += raiseSkill1Cost[j];
                }
                
                for (int j = 0; j < skill2Level; j++)
                {
                    total += raiseSkill2Cost[j];
                }
            }

            var sellRate = GamePlayController.Instance.State == GamePlayState.Init
                ? 1
                : GameContainer.Instance.Get<UnitDataBase>().Get<TowerCost>().sellRate;
            
            return Mathf.RoundToInt(total * sellRate);
        }
    }
    
    [Serializable]
    public class TowerCost : ScriptableObject
    {
        public float sellRate;
        public IntTowerRaiseCostDictionary raiseCosts;
    }

    public class TowerCostCsv
    {
        public float sellRate;
        public TowerRaiseCsv[] raise;

        public class TowerRaiseCsv
        {
            public int id;
            public int lv1;
            public int lv2;
            public int lv3;
            public int skill1Level1;
            public int skill1Level2;
            public int skill1Level3;
            public int skill2Level1;
        }
    }
    
    #if UNITY_EDITOR
    public class TowerCostPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/tower_cost";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TowerCost) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerCost gm = AssetDatabase.LoadAssetAtPath<TowerCost>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerCost>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    var towerCostCsv = CsvReader.Deserialize<TowerCostCsv>(data.text)[0];

                    gm.sellRate = towerCostCsv.sellRate;
                    gm.raiseCosts = new IntTowerRaiseCostDictionary();
                    
                    for (int i = 0; i < towerCostCsv.raise.Length; i++)
                    {
                        var raise = towerCostCsv.raise[i];
                        
                        var info = new TowerRaiseCost();
                        gm.raiseCosts[raise.id] = info;

                        info.id = raise.id;
                        info.raiseLevelCost[0] = raise.lv1;
                        info.raiseLevelCost[1] = raise.lv2;
                        info.raiseLevelCost[2] = raise.lv3;

                        info.raiseSkill1Cost[0] = raise.skill1Level1;
                        info.raiseSkill1Cost[1] = raise.skill1Level2;
                        info.raiseSkill1Cost[2] = raise.skill1Level3;
                        
                        info.raiseSkill2Cost[0] = raise.skill2Level1;
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