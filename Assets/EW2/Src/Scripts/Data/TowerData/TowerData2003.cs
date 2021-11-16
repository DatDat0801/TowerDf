using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;
using Zitga.Localization;

namespace EW2
{
    public class TowerData2003 : TowerData
    {
        public TowerStatBase[] statBase;

        public Skill1[] skill1;

        public Skill2[] skill2;

        public Tower2003BonusStat BonusStat
        {
            get
            {
                var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2003);
                if (towerStat == null)
                {
                    return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2003(1);
                }

                var upgradeLevel = towerStat.towerLevel;
                return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2003(upgradeLevel);
            }
        }

        [Serializable]
        public class Skill1
        {
            public int level;
            public float cooldown;
            public float damage;
        }

        [Serializable]
        public class Skill2
        {
            public int level;
            public float slowPercent;
            public ModifierType modifierType;
            public float time;
            public float cooldown;
            public float damage;
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
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2003);

            if (towerStat == null)
                return GetDataStatBaseByLevel(level);

            var towerStatFinal = GetDataStatBaseByLevel(level).Clone();

            var upgradeLevel = towerStat.towerLevel;
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2003(upgradeLevel);

            towerStatFinal.damage *= (1 + totalStat.bonusDamage);
            towerStatFinal.attackSpeed *= (1 + totalStat.bonusAttackSpeed);
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
                if (BonusStat.level >= 6)
                {
                    return string.Format(desc, 3, skill1[level].damage * (1 + BonusStat.bonusDamageSkill1)) + ". " +
                           string.Format(L.tower.tower_skill_des_2003_0_update, BonusStat.level6Stat.secondDamage,
                               BonusStat.level6Stat.slowDown * 100);
                }
                else
                {
                    return string.Format(desc, 3, skill1[level].damage * (1 + BonusStat.bonusDamageSkill1));
                }
            }
            else
            {
                return string.Format(desc, skill2[level].damage);
            }
        }
    }

#if UNITY_EDITOR
    public class Tower2003Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/Tower_2003";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get tower id
                    var towerId = "2003";

                    // get asset file
                    string nameAsset = nameof(TowerData2003) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerData2003 gm = AssetDatabase.LoadAssetAtPath<TowerData2003>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerData2003>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}/tower_{towerId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    gm.id = 2003;

                    gm.statBase = CsvReader.Deserialize<TowerStatBase>(data.text);

                    // get skill1 file
                    gm.skill1 = TowerData.LoadAsset<TowerData2003.Skill1>(csvFormat, towerId, 0);

                    // get skill2 file
                    gm.skill2 = TowerData.LoadAsset<TowerData2003.Skill2>(csvFormat, towerId, 1);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}