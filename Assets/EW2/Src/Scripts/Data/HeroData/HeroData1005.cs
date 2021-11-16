using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroData1005 : HeroData
    {
        public ActiveSkill[] active;

        public PassiveSkill1[] passive1;

        public PassiveSkill2[] passive2;

        public PassiveSkill3[] passive3;

        [System.Serializable]
        public class ActiveSkill
        {
            public int level;
            public float cooldown;
            public float range;
            public float damage;
            public DamageType damageType;
            public float cooldownDecrease;
        }

        [System.Serializable]
        public class PassiveSkill1
        {
            public int level;
            public float range;
            public float damage;
            public DamageType damageType;
            public float explosionRatio;
        }

        [System.Serializable]
        public class PassiveSkill2
        {
            public int level;
            public float attackSpeedIncrease;
            public float aoeDuration;
            public ModifierType modifierType;
        }

        [System.Serializable]
        public class PassiveSkill3
        {
            public int level;
            public float stunInSeconds;
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
                        result[0].Add(passiveData.damage);

                        result[1].Add(passiveData.cooldownDecrease);
                    }

                    break;
                }
                case 1:
                {
                    result.Add(0, new List<float>());

                    result.Add(1, new List<float>());

                    foreach (var passiveData in this.passive1)
                    {
                        result[0].Add(passiveData.explosionRatio * 100);

                        result[1].Add(passiveData.damage);
                    }

                    break;
                }
                case 2:
                {
                    result.Add(0, new List<float>());

                    result.Add(1, new List<float>());

                    foreach (var passiveData in this.passive2)
                    {
                        result[0].Add(passiveData.attackSpeedIncrease * 100);

                        result[1].Add(passiveData.aoeDuration);
                    }

                    break;
                }
                default:
                {
                    result.Add(0, new List<float>());

                    foreach (var passiveData in this.passive3)
                    {
                        result[0].Add(passiveData.stunInSeconds);
                    }

                    break;
                }
            }

            return result;
        }
    }
#if UNITY_EDITOR
    public class Hero1005Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/Hero_1005";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var heroId = "1005";

                    // get asset file
                    string nameAsset = nameof(HeroData1005) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    HeroData1005 gm = AssetDatabase.LoadAssetAtPath<HeroData1005>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroData1005>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/hero_{heroId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<HeroStatBase>(data.text);

                    // get hero active file
                    gm.active = HeroData.LoadAsset<HeroData1005.ActiveSkill>(csvFormat, heroId, 0);

                    // get hero passive 1 file
                    gm.passive1 = HeroData.LoadAsset<HeroData1005.PassiveSkill1>(csvFormat, heroId, 1);

                    // get hero passive 2 file
                    gm.passive2 = HeroData.LoadAsset<HeroData1005.PassiveSkill2>(csvFormat, heroId, 2);

                    // get hero passive 3 file
                    gm.passive3 = HeroData.LoadAsset<HeroData1005.PassiveSkill3>(csvFormat, heroId, 3);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    //Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}