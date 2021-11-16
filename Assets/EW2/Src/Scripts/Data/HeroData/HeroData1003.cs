using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroData1003 :HeroData
    {
        public HeroData1003.ActiveSkill[] active;

        public HeroData1003.PassiveSkill1[] passive1;

        public HeroData1003.PassiveSkill2[] passive2;

        public HeroData1003.PassiveSkill3[] passive3;
        public override Dictionary<int, List<float>> GetDescStatSkill(int skillId)
        {
            Dictionary<int, List<float>> result = new Dictionary<int, List<float>>();

            if (skillId == 0)
            {
                result.Add(0, new List<float>());

                result.Add(1, new List<float>());

                for (int i = 0; i < active.Length; i++)
                {
                    result[0].Add(active[i].recoverHp *100);
                    result[1].Add(active[i].bonusNormalDamage *100);
                }
            }
            else if (skillId == 1)
            {
                result.Add(0, new List<float>());

                result.Add(1, new List<float>());

                result.Add(2, new List<float>());

                for (int i = 0; i < passive1.Length; i++)
                {
                    result[0].Add(passive1[i].roarRatio * 100);
                    result[1].Add(passive1[i].bonusArmor * 100);
                    result[2].Add(passive1[i].duration);
                }
            }
            else if (skillId == 2)
            {
                result.Add(0, new List<float>());

                for (int i = 0; i < passive2.Length; i++)
                {
                    result[0].Add(passive2[i].counterAttackRatio * 100);
                }
            }
            else if (skillId == 3)
            {
                result.Add(0, new List<float>());
                for (int i = 0; i < passive3.Length; i++)
                {
                    result[0].Add(passive3[i].recoverHpPerKilledEnemy);
                }
            }

            return result;
        }
        
        [System.Serializable]
        public class ActiveSkill
        {
            public int level;
            public float recoverHp;
            //public float increaseAttackSpeed;
            //Physic damage
            public float duration;
            public float bonusNormalDamage;
            public float cooldown;
            public ModifierType modifierType;
        }

        [System.Serializable]
        public class PassiveSkill1
        {
            public int level;
            //each receive from normal attack
            public float roarRatio;
            public float roarRadiusAffected;
            public float bonusArmor;
            public float duration;
            public float cooldown;
            public ModifierType modifierType;
        }

        [System.Serializable]
        public class PassiveSkill2
        {
            public int level;
            public float counterAttackRatio;
            public ModifierType modifierType;
        }

        [System.Serializable]
        public class PassiveSkill3
        {
            public int level;
            public float recoverHpPerKilledEnemy;
            public ModifierType modifierType;
        }
    }
    #if UNITY_EDITOR
    public class Hero1003Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/Hero_1003";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var heroId = "1003";

                    // get asset file
                    string nameAsset = nameof(HeroData1003) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    HeroData1003 gm = AssetDatabase.LoadAssetAtPath<HeroData1003>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroData1003>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/hero_{heroId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<HeroStatBase>(data.text);

                    // get hero active file
                    gm.active = HeroData.LoadAsset<HeroData1003.ActiveSkill>(csvFormat, heroId, 0);

                    // get hero passive 1 file
                    gm.passive1 = HeroData.LoadAsset<HeroData1003.PassiveSkill1>(csvFormat, heroId, 1);

                    // get hero passive 2 file
                    gm.passive2 = HeroData.LoadAsset<HeroData1003.PassiveSkill2>(csvFormat, heroId, 2);

                    // get hero passive 3 file
                    gm.passive3 = HeroData.LoadAsset<HeroData1003.PassiveSkill3>(csvFormat, heroId, 3);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}