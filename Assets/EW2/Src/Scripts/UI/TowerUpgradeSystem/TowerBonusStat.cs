using System;

namespace EW2
{
    [Serializable]
    public class Tower2001BonusStat
    {
        public int level;
        public float bonusDamage; //4
        public float bonusCrit; //2
        public float bonusDetectRange; //1
        public int targets; //3
        public float bonusDamagePerArrowSkill1; //5
        public Level6Stat level6Stat; //6

        [Serializable]
        public class Level6Stat
        {
            public float criticalRatio;
            public float criticalDamage;
            public DamageType damageType;

            public static Level6Stat operator +(Level6Stat a, Level6Stat b)
            {
                if (a == null)
                    a = new Level6Stat() {damageType = b.damageType};
                return new Level6Stat()
                {
                    criticalDamage = a.criticalDamage + b.criticalDamage,
                    criticalRatio = a.criticalRatio + b.criticalRatio, damageType = b.damageType
                };
            }
        }

        public static Tower2001BonusStat operator +(Tower2001BonusStat a, Tower2001BonusStat b)
        {
            var totalBonusStat = new Tower2001BonusStat();

            totalBonusStat.bonusDamage = a.bonusDamage + b.bonusDamage;
            totalBonusStat.bonusCrit = a.bonusCrit + b.bonusCrit;
            totalBonusStat.bonusDetectRange = a.bonusDetectRange + b.bonusDetectRange;
            totalBonusStat.targets = a.targets + b.targets;
            totalBonusStat.level6Stat = a.level6Stat + b.level6Stat;
            totalBonusStat.bonusDamagePerArrowSkill1 = a.bonusDamagePerArrowSkill1 + b.bonusDamagePerArrowSkill1;

            totalBonusStat.level = b.level;
            return totalBonusStat;
        }

        /// <summary>
        /// Get bonus value by tower level
        /// </summary>
        public float GetValueByLevel()
        {
            switch (level)
            {
                case 5:
                    return bonusDamagePerArrowSkill1;
                case 4:
                    return bonusDamage;
                case 3: return targets;
                case 2:
                    return bonusCrit;
                case 1:
                    return bonusDetectRange;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    public class Tower2002BonusStat
    {
        public int level;
        public float bonusDetectRange; //1
        public float bonusMagicDamage; //2
        public float damageReduceBulletDecrease; //4

        public float distanceUnit; //3

        //public Level5Stat level5Stat;
        public int upgradedBall; //5
        public Level6Stat level6Stat;

        [Serializable]
        public class Level6Stat
        {
            public float range;
            public float damageType;
            public float damage;

            public static Level6Stat operator +(Level6Stat a, Level6Stat b)
            {
                if (a == null)
                    a = new Level6Stat() {damageType = b.damageType};
                return new Level6Stat()
                {
                    range = a.range + b.range,
                    damage = a.damage + b.damage, damageType = b.damageType
                };
            }
        }

        // [Serializable]
        // public class Level5Stat
        // {
        //     public int standardBall;
        //     public int upgradedBall;
        //     public static Level5Stat operator +(Level5Stat a, Level5Stat b)
        //     {
        //         if (a == null)
        //             a = new Level5Stat() ;
        //         return new Level5Stat()
        //         {
        //             standardBall = a.standardBall + b.upgradedBall,
        //             upgradedBall = a.upgradedBall + b.upgradedBall
        //         };
        //     }
        // }

        public static Tower2002BonusStat operator +(Tower2002BonusStat a, Tower2002BonusStat b)
        {
            var totalBonusStat = new Tower2002BonusStat();

            totalBonusStat.bonusMagicDamage = a.bonusMagicDamage + b.bonusMagicDamage;
            totalBonusStat.damageReduceBulletDecrease = a.damageReduceBulletDecrease + b.damageReduceBulletDecrease;
            totalBonusStat.bonusDetectRange = a.bonusDetectRange + b.bonusDetectRange;
            totalBonusStat.distanceUnit = a.distanceUnit + b.distanceUnit;
            totalBonusStat.level6Stat = a.level6Stat + b.level6Stat;
            //totalBonusStat.level5Stat = a.level5Stat + b.level5Stat;
            totalBonusStat.upgradedBall = a.upgradedBall + b.upgradedBall;

            totalBonusStat.level = b.level;
            return totalBonusStat;
        }

        /// <summary>
        /// Get bonus value by tower level
        /// </summary>
        public float GetValueByLevel()
        {
            switch (level)
            {
                case 1:
                    return bonusDetectRange;
                case 2:
                    return bonusMagicDamage;
                case 4:
                    return damageReduceBulletDecrease;
                case 3:
                    return distanceUnit;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    public class Tower2003BonusStat
    {
        public int level;
        public float bonusDetectRange;
        public float bonusDamage;
        public float bonusAttackSpeed;
        public float bonusDamageSkill1;
        public Level3Stat level3Stat;
        public Level6Stat level6Stat;

        [Serializable]
        public class Level3Stat
        {
            public float fireBulletRatio;
            public float fireBulletDamage;
            public float poisonousFire;
            public float poisonousFireRate;
            public float affectedTime;

            public static Level3Stat operator +(Level3Stat a, Level3Stat b)
            {
                if (a == null)
                    a = new Level3Stat();
                return new Level3Stat()
                {
                    fireBulletRatio = a.fireBulletRatio + b.fireBulletRatio,
                    fireBulletDamage = a.fireBulletDamage + b.fireBulletDamage,
                    poisonousFire = a.poisonousFire + b.poisonousFire,
                    poisonousFireRate = a.poisonousFireRate + b.poisonousFireRate,
                    affectedTime = a.affectedTime + b.affectedTime
                };
            }
        }

        [Serializable]
        public class Level6Stat
        {
            public float secondDamage;
            public float slowDown;
            public float slowTime;

            public static Level6Stat operator +(Level6Stat a, Level6Stat b)
            {
                if (a == null)
                    a = new Level6Stat();
                return new Level6Stat()
                {
                    secondDamage = a.secondDamage + b.secondDamage,
                    slowDown = a.slowDown + b.slowDown,
                    slowTime = a.slowTime + b.slowTime
                };
            }
        }

        public static Tower2003BonusStat operator +(Tower2003BonusStat a, Tower2003BonusStat b)
        {
            var totalBonusStat = new Tower2003BonusStat();

            totalBonusStat.bonusDamage = a.bonusDamage + b.bonusDamage;
            totalBonusStat.bonusAttackSpeed = a.bonusAttackSpeed + b.bonusAttackSpeed;
            totalBonusStat.bonusDetectRange = a.bonusDetectRange + b.bonusDetectRange;
            totalBonusStat.bonusDamageSkill1 = a.bonusDamageSkill1 + b.bonusDamageSkill1;
            totalBonusStat.level3Stat = a.level3Stat + b.level3Stat;
            totalBonusStat.level6Stat = a.level6Stat + b.level6Stat;
            totalBonusStat.level = b.level;


            return totalBonusStat;
        }

        /// <summary>
        /// Get bonus value by tower level
        /// </summary>
        public float GetValueByLevel()
        {
            switch (level)
            {
                case 1:
                    return bonusDetectRange;
                case 2:
                    return bonusDamage;
                case 4:
                    return bonusAttackSpeed;
                case 5:
                    return bonusDamageSkill1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    public class Tower2004BonusStat
    {
        public int level;
        public float bonusHeath;
        public float bonusDamage;
        public float bonusSpawnTime;
        public int troopNumber;

        public Level5Stat level5Stat;
        public float ghostExistTime;

        [Serializable]
        public class Level5Stat
        {
            public float skill2Armor;
            public float skill2MagicResistance;

            public static Level5Stat operator +(Level5Stat a, Level5Stat b)
            {
                if (a == null)
                    a = new Level5Stat();
                return new Level5Stat()
                {
                    skill2Armor = a.skill2Armor + b.skill2Armor,
                    skill2MagicResistance = a.skill2MagicResistance + b.skill2MagicResistance
                };
            }
        }

        public static Tower2004BonusStat operator +(Tower2004BonusStat a, Tower2004BonusStat b)
        {
            var totalBonusStat = new Tower2004BonusStat();

            totalBonusStat.bonusDamage = a.bonusDamage + b.bonusDamage;
            totalBonusStat.bonusHeath = a.bonusHeath + b.bonusHeath;
            totalBonusStat.bonusSpawnTime = a.bonusSpawnTime + b.bonusSpawnTime;
            totalBonusStat.level5Stat = a.level5Stat + b.level5Stat;
            totalBonusStat.troopNumber = a.troopNumber + b.troopNumber;
            totalBonusStat.ghostExistTime = a.ghostExistTime + b.ghostExistTime;
            totalBonusStat.level = b.level;
            return totalBonusStat;
        }

        /// <summary>
        /// Get bonus value by tower level
        /// </summary>
        public float GetValueByLevel()
        {
            switch (level)
            {
                case 1:
                    return bonusHeath;
                case 2:
                    return bonusDamage;
                case 4:
                    return bonusSpawnTime;
                case 3: return troopNumber;
                case 6: return ghostExistTime;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}