using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TowerData2001 : TowerData
    {
        public TowerStatBase[] statBase;

        public Skill1[] skill1;

        public Skill2[] skill2;
        
        public Tower2001BonusStat BonusStat
        {
            get
            {
                var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2001);
                if (towerStat == null)
                {
                    return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2001(1);
                }
                var upgradeLevel = towerStat.towerLevel;
                return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2001(upgradeLevel);
            }
        }

        [System.Serializable]
        public class Skill1
        {
            public int level;
            public float cooldown;
            public float damage;
            public int numberArrow;
            public float delayAttack;
        }

        [System.Serializable]
        public class Skill2
        {
            public int level;
            public float atkSpeedBonusPercent;
            public ModifierType modifierType;
        }

        public Skill1 GetDataSkill1ByLevel(int level)
        {
            if (level - 1 < skill1.Length)
                return skill1[level - 1];

            return null;
        }

        public Skill2 GetDataSkill2ByLevel(int level)
        {
            if (level - 1 < skill2.Length)
                return skill2[level - 1];

            return null;
        }

        public override TowerStatBase GetDataStatBaseByLevel(int level)
        {
            if (level - 1 < statBase.Length)
                return statBase[level - 1];

            return null;
        }

        public override TowerStatBase GetDataStatFinalByLevel(int level)
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2001);

            if (towerStat == null)
                return GetDataStatBaseByLevel(level);

            var towerStatFinal = GetDataStatBaseByLevel(level).Clone();
            
            var upgradeLevel = towerStat.towerLevel;
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2001(upgradeLevel);
            
            towerStatFinal.damage *= (1 + totalStat.bonusDamage);
            towerStatFinal.critDamage *= (1 + totalStat.bonusCrit);
            towerStatFinal.detectRangeAttack *= (1 + totalStat.bonusDetectRange);

            return towerStatFinal;
        }

        public override BuildingInfoContent GetInfo(int level)
        {
            var data = GetDataStatFinalByLevel(level);
            string attackType = string.Empty;
            switch (data.searchTarget)
            {
                case MoveType.None:
                    break;
                case MoveType.Ground:
                    attackType = L.gameplay.enemy_type_ground;
                    break;
                case MoveType.Fly:
                    attackType = L.gameplay.enemy_type_fly;
                    break;
                case MoveType.All:
                    attackType = L.gameplay.tower_attack_type_all;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new BuildingInfoContent()
            {
                damage = data.damage.ToString(CultureInfo.InvariantCulture),
                atkSpeed = data.attackSpeed.ToString(CultureInfo.CurrentCulture),
                
                atkType = attackType
            };
        }

        public override string GetInfoSkill(int skillId, int level, string desc)
        {
            if (skillId == 0)
            {
                return string.Format(desc, skill1[level].numberArrow, skill1[level].damage * (1+ BonusStat.bonusDamage));
            }
            else
            {
                return string.Format(desc, skill2[level].atkSpeedBonusPercent * 100);
            }
        }
    }

#if UNITY_EDITOR
    public class Tower2001Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/Tower_2001";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get tower id
                    var towerId = "2001";

                    // get asset file
                    string nameAsset = nameof(TowerData2001) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerData2001 gm = AssetDatabase.LoadAssetAtPath<TowerData2001>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerData2001>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}/tower_{towerId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    gm.id = 2001;

                    gm.statBase = CsvReader.Deserialize<TowerStatBase>(data.text);

                    // get skill1 file
                    gm.skill1 = TowerData.LoadAsset<TowerData2001.Skill1>(csvFormat, towerId, 0);

                    // get skill2 file
                    gm.skill2 = TowerData.LoadAsset<TowerData2001.Skill2>(csvFormat, towerId, 1);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}