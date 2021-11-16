using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class Tower2002BulletData
    {
        public float damageReduceBullet;

        public float lifeTimeBullet;

        public float bulletSpeed;
    }

    [Serializable]
    public class Tower2002TowerStatBase : TowerStatBase
    {
        public Tower2002BulletData bulletData;
    }

    public class TowerData2002 : TowerData
    {
        public Tower2002TowerStatBase[] statBase;

        public Skill1[] skill1;

        public Skill2[] skill2;
        public Tower2002BonusStat BonusStat
        {
            get
            {
                var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2002);
                if (towerStat == null)
                {
                    return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(1);
                }

                var upgradeLevel = towerStat.towerLevel;
                return GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(upgradeLevel);
            }
        }
        [Serializable]
        public class Skill1
        {
            public int level;
            public float cooldown;
            public float damagePerShot;
            public float delayAttack;
            public int numberMagicBall;
        }

        [Serializable]
        public class Skill2
        {
            public int level;
            public int numberMaxTarget;
            public float timeLife;
            public float cooldown;
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
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2002);

            if (towerStat == null)
                return GetDataStatBaseByLevel(level);
            
            var towerStatFinal = GetDataStatBaseByLevel(level).Clone();
            
            var upgradeLevel = towerStat.towerLevel;
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2002(upgradeLevel);

            towerStatFinal.damage *= (1 + totalStat.bonusMagicDamage);
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
                return string.Format(desc, skill1[level].numberMagicBall + BonusStat.upgradedBall, skill1[level].damagePerShot);
            }
            else
            {
                if (BonusStat.level >= 3)
                {
                    return L.upgrade_tower.tower_up_des_2002_2;
                }
                else
                {
                    return string.Format(desc, skill2[level].numberMaxTarget);
                }
                
            }
        }
    }

#if UNITY_EDITOR
    public class Tower2002Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/Tower_2002";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get tower id
                    var towerId = "2002";

                    // get asset file
                    string nameAsset = nameof(TowerData2002) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerData2002 gm = AssetDatabase.LoadAssetAtPath<TowerData2002>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerData2002>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}/tower_{towerId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    gm.id = 2002;

                    gm.statBase = CsvReader.Deserialize<Tower2002TowerStatBase>(data.text);

                    // get skill1 file
                    gm.skill1 = TowerData.LoadAsset<TowerData2002.Skill1>(csvFormat, towerId, 0);

                    // get skill2 file
                    gm.skill2 = TowerData.LoadAsset<TowerData2002.Skill2>(csvFormat, towerId, 1);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}