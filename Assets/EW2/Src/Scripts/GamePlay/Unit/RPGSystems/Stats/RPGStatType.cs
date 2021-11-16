using UnityEngine;
using System.Collections;

/// <summary>
/// Used to list off all stats that can be used
/// within the RPGStatCollection class
/// </summary>
public enum 
    RPGStatType
{
    None = 0,
    Health = 1,
    MoveSpeed = 2,
    AttackSpeed = 3,
    Armor = 4,
    Resistance = 5,
    Damage = 6,
    CritChance = 8,
    /// <summary>
    /// increase crit damage percent, for example: Damage =10, CritDamage =0.5 => damage = 15
    /// </summary>
    CritDamage = 9,
    DetectBlock = 10,
    HpRegeneration = 11,
    CooldownRegeneration = 12,
    TimeRevive = 13,
    BlockEnemy = 14,
    CooldownReduction = 15,
    LifeSteal = 16,
    RangeDetect = 17,
    DamageReduceBullet = 18,
    SpawnTimeBarrack = 19,
    /// <summary>
    /// increase crit damage, for example: Damage=10, CritDamage =0.5, CritDamageBonus=3, =>Damage =18
    /// </summary>
    CritDamageBonus = 20,
    SkillActiveCooldown = 21
    
}