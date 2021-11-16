using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;

#endif


namespace EW2
{
    public class HeroData1001 : HeroData
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
            public float baseDamage;
            //public float damageBaseOnAtk;
            public float lifeTime;
        }

        [System.Serializable]
        public class PassiveSkill1
        {
            public int level;
            public int basicAttackCountEffect;
            public float damageBaseOnAtk;
        }

        [System.Serializable]
        public class PassiveSkill2
        {
            public int level;
            public float reducePhysicDamage;
            public ModifierType modifierType;
        }

        [System.Serializable]
        public class PassiveSkill3
        {
            public int level;
            public float valueShield;
            public float bonusPhysicArmor;
            public ModifierType modifierType;
            public float region;
            public float time;
            public float cooldown;
        }


        public override Dictionary<int, List<float>> GetDescStatSkill(int skillId)
        {
            Dictionary<int, List<float>> result = new Dictionary<int, List<float>>();

            if (skillId == 0)
            {
                result.Add(0, new List<float>());

                result.Add(1, new List<float>());

                for (int i = 0; i < active.Length; i++)
                {
                    result[0].Add(active[i].baseDamage);
                    
                    result[1].Add(active[i].lifeTime);
                }
            }
            else if (skillId == 1)
            {
                result.Add(0, new List<float>());

                for (int i = 0; i < passive1.Length; i++)
                {
                    result[0].Add(passive1[i].damageBaseOnAtk * 100);
                }
            }
            else if (skillId == 2)
            {
                result.Add(0, new List<float>());

                for (int i = 0; i < passive2.Length; i++)
                {
                    result[0].Add(passive2[i].reducePhysicDamage * 100);
                }
            }
            else
            {
                result.Add(0, new List<float>());

                result.Add(1, new List<float>());

                for (int i = 0; i < passive3.Length; i++)
                {
                    result[0].Add(passive3[i].valueShield);

                    result[1].Add(passive3[i].bonusPhysicArmor * 100);
                }
            }

            return result;
        }
    }

#if UNITY_EDITOR
    public class Hero1001Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/Hero_1001";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get hero id
                    var heroId = "1001";

                    // get asset file
                    string nameAsset = nameof(HeroData1001) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    HeroData1001 gm = AssetDatabase.LoadAssetAtPath<HeroData1001>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroData1001>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/hero_{heroId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.stats = CsvReader.Deserialize<HeroStatBase>(data.text);

                    // get hero active file
                    gm.active = HeroData.LoadAsset<HeroData1001.ActiveSkill>(csvFormat, heroId, 0);

                    // get hero passive 1 file
                    gm.passive1 = HeroData.LoadAsset<HeroData1001.PassiveSkill1>(csvFormat, heroId, 1);

                    // get hero passive 2 file
                    gm.passive2 = HeroData.LoadAsset<HeroData1001.PassiveSkill2>(csvFormat, heroId, 2);

                    // get hero passive 3 file
                    gm.passive3 = HeroData.LoadAsset<HeroData1001.PassiveSkill3>(csvFormat, heroId, 3);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}