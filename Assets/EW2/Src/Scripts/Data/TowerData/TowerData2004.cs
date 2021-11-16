using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TowerData2004 : TowerData
    {
        public StatBaseBarrack[] statBaseBarracks;

        public Skill1[] skill1;

        public Skill2[] skill2;

        public Soldier2004Data[] dataSoldiers;

        [System.Serializable]
        public class StatBaseBarrack
        {
            public int level;
            public int soldierNumber;
            public float rangeRally;
        }

        [System.Serializable]
        public class Skill1
        {
            public int level;
            public float chance;
            public float damage;
        }

        [System.Serializable]
        public class Skill2
        {
            public int level;
            public float armorBonus;
            public ModifierType modifierTypeArmor;
            public float magicResBonus;
            public ModifierType modifierTypeMagicRes;
        }

        [System.Serializable]
        public class Soldier2004Data : SoldierData
        {
        }

        public StatBaseBarrack GetStatBarrackByLevel(int level)
        {
            if (level - 1 < statBaseBarracks.Length)
                return statBaseBarracks[level - 1];

            return null;
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

        public Soldier2004Data GetDataSoldierByLevel(int level)
        {
            if (level - 1 < dataSoldiers.Length)
                return dataSoldiers[level - 1];

            return null;
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

        public override TowerStatBase GetDataStatBaseByLevel(int level)
        {

            var soldierData = GetDataSoldierByLevel(level);
        
            var towerStatBase = new TowerStatBase
            {
                damage = soldierData.damage,
                attackSpeed = soldierData.attackSpeed,
                searchTarget = soldierData.searchTarget
            };


            return towerStatBase;
        }

        public override TowerStatBase GetDataStatFinalByLevel(int level)
        {
            var towerStat = UserData.Instance.UserTowerData.GetTowerStat(2004);

            if (towerStat == null)
                return GetDataStatBaseByLevel(level);

            var towerStatFinal = GetDataStatBaseByLevel(level).Clone();
            
            var upgradeLevel = towerStat.towerLevel;
            var totalStat = GameContainer.Instance.GetTowerUpgradeData().GetTotalBonusStat2004(upgradeLevel);
            
            towerStatFinal.damage *= (1 + totalStat.bonusDamage);

            return towerStatFinal;
        }

        public override string GetInfoSkill(int skillId, int level, string desc)
        {
            if (skillId == 0)
            {
                return string.Format(desc, skill1[level].chance * 100, skill1[level].damage);
            }
            else
            {
                return string.Format(desc, skill2[level].armorBonus * 100, skill2[level].magicResBonus * 100);
            }
        }
    }

#if UNITY_EDITOR
    public class Tower2004Postprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/Tower_2004";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get tower id
                    var towerId = "2004";

                    // get asset file
                    string nameAsset = nameof(TowerData2004) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerData2004 gm = AssetDatabase.LoadAssetAtPath<TowerData2004>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerData2004>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}/tower_{towerId}_base.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    gm.id = 2004;

                    gm.statBaseBarracks = CsvReader.Deserialize<TowerData2004.StatBaseBarrack>(data.text);

                    // get skill1 file
                    gm.skill1 = TowerData.LoadAsset<TowerData2004.Skill1>(csvFormat, towerId, 0);

                    // get skill2 file
                    gm.skill2 = TowerData.LoadAsset<TowerData2004.Skill2>(csvFormat, towerId, 1);

                    //soldier
                    string nameCsv = $"{csvFormat}/soldier_2004.csv";
                    TextAsset dataParse = AssetDatabase.LoadAssetAtPath<TextAsset>(nameCsv);
                    gm.dataSoldiers = CsvReader.Deserialize<TowerData2004.Soldier2004Data>(dataParse.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}