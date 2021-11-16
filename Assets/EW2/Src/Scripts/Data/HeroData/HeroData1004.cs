using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroData1004 : HeroData
    {
        public ActiveSkill[] active;

        public PassiveSkill1[] passive1;

        public PassiveSkill2[] passive2;

        public PassiveSkill3[] passive3;

        public PoisonStatus[] poisonStatuses;

        [Serializable]
        public class PoisonStatus
        {
            public int level;
            public float timeLife;
            public float hpPerSecond;
        }

        [Serializable]
        public class ActiveSkill
        {
            public int level;
            public float cooldown;
            public float timeLife;
            public float damagePerSecond;
        }

        [Serializable]
        public class PassiveSkill1
        {
            public int level;
            public float ratioReductionSpeed;
            public ModifierType modifierType;
        }

        [Serializable]
        public class PassiveSkill2
        {
            public int level;
            public float chance;
            public float detectRange;
            public float timeLife;
        }

        [Serializable]
        public class PassiveSkill3
        {
            public int level;
            public float cooldown;
            public float regionDetect;
            public float regenHpPerSecond;
            public float ratioIncreaseAtkSpeed;
            public float timeLife;
            public ModifierType modifierType;
        }

        public override Dictionary<int, List<float>> GetDescStatSkill(int skillId)
        {
             Dictionary<int, List<float>> result = new Dictionary<int, List<float>>();
            
            switch (skillId)
            {
                case 0:
                {
                    result.Add(0, new List<float>());
            
                    result.Add(1, new List<float>());

                    foreach (var passiveData in this.active)
                    {
                        result[0].Add(passiveData.damagePerSecond);

                        result[1].Add(passiveData.timeLife);
                    }

                    break;
                }
                case 1:
                {
                    result.Add(0, new List<float>());

                    foreach (var passiveData in this.passive1)
                    {
                        result[0].Add(passiveData.ratioReductionSpeed * 100);
                    }

                    break;
                }
                case 2:
                {
                    result.Add(0, new List<float>());
                    result.Add(1, new List<float>());
                    result.Add(2, new List<float>());
                
                    foreach (var passiveData in this.passive2)
                    {
                        result[0].Add(passiveData.chance * 100);
                        result[1].Add(passiveData.detectRange);
                        result[2].Add(passiveData.timeLife);
                    }

                    break;
                }
                default:
                {
                    result.Add(0, new List<float>());
                    result.Add(1, new List<float>());
                    result.Add(2, new List<float>());
               
                    foreach (var passiveData in this.passive3)
                    {
                        result[0].Add(passiveData.regenHpPerSecond);
                        result[1].Add(passiveData.ratioIncreaseAtkSpeed * 100);
                        result[2].Add(passiveData.timeLife);
                    }

                    break;
                }
            }

            return result;
        }
    }

#if UNITY_EDITOR
    public class Hero1004Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/Hero_1004";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var heroId = "1004";

                    // get asset file
                    string nameAsset = nameof(HeroData1004) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    HeroData1004 gm = AssetDatabase.LoadAssetAtPath<HeroData1004>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroData1004>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/hero_{heroId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<HeroStatBase>(data.text);

                    //posion
                    string namePoisonCsv = $"{csvFormat}/hero_{heroId}_poison_status.csv";
                    TextAsset dataPoison = AssetDatabase.LoadAssetAtPath<TextAsset>(namePoisonCsv);
                    gm.poisonStatuses = CsvReader.Deserialize<HeroData1004.PoisonStatus>(dataPoison.text);

                    // get hero active file
                    gm.active = HeroData.LoadAsset<HeroData1004.ActiveSkill>(csvFormat, heroId, 0);

                    // get hero passive 1 file
                    gm.passive1 = HeroData.LoadAsset<HeroData1004.PassiveSkill1>(csvFormat, heroId, 1);

                    // get hero passive 2 file
                    gm.passive2 = HeroData.LoadAsset<HeroData1004.PassiveSkill2>(csvFormat, heroId, 2);

                    // get hero passive 3 file
                    gm.passive3 = HeroData.LoadAsset<HeroData1004.PassiveSkill3>(csvFormat, heroId, 3);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    //Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}