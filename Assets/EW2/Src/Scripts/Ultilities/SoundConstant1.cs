using JetBrains.Annotations;
using UnityEngine;

namespace EW2
{
    public partial class SoundConstant
    {
        public const string BUTTON_CLICK = "Sounds/Sfx/UI/sfx_ui_click";
        public const string POPUP_CLOSE = "Sounds/Sfx/UI/sfx_ui_popup_close";
        public const string POPUP_OPEN = "Sounds/Sfx/UI/sfx_ui_popup_open";
        public const string VICTORY = "Sounds/Sfx/UI/sfx_ui_victory";
        public const string DEFEAT = "Sounds/Sfx/UI/sfx_ui_defeat";
        public const string STAR_RESULT = "Sounds/Sfx/UI/sfx_ui_star";
        public const string CONFIRM = "Sounds/Sfx/UI/sfx_ui_confirm";
        public const string TOWER_UPGRADE = "Sounds/Sfx/UI/sfx_tower_build";
        public const string TOWER_SELL = "Sounds/Sfx/UI/sfx_tower_sell";
        public const string FINAL_WAVE = "Sounds/Sfx/UI/sfx_ui_finalwave";
        public const string PURCHASE = "Sounds/Sfx/UI/sfx_ui_buy";

        /// <summary>
        /// Click upgrade life time user's wealth like Tower, Power-Up, Hero Skill
        /// </summary>
        public const string USER_UPGRADE = "Sounds/Sfx/UI/sfx_ui_skill_select";

        public const string GEM_COUNT = "Sounds/Sfx/UI/sfx_ui_gem_count";

        #region HERO
        public const string HERO_REVIVE = "Sounds/Sfx/1001/sfx_hero_lyna_active";
        public const string HERO_1001_PASSIVE_1 = "Sounds/Sfx/1001/sfx_hero_miria_active_explode_3";
        public const string HERO_1001_SKILL_IMPACT  = "Sounds/Sfx/1001/sfx_hero_1001_active_impact";
        public const string HERO_1001_SKILL_JUMP = "Sounds/Sfx/1001/sfx_hero_1001_active_jump";

        //public static string Hero1001Melee = string.Format("Sounds/Sfx/1001/sfx_hero_j4_normalatk_{1}");
        public static string Hero1001Melee()
        {
            return $"Sounds/Sfx/1001/sfx_hero_j4_normalatk_{Random.Range(1, 4).ToString()}";
        }
        public const string HERO_1001_PASSIVE_3 = "Sounds/Sfx/1001/sfx_hero_federyc_passive_3_Throw";
        public const string HERO_1002_SHOOT = "Sounds/Sfx/1002/sfx_hero_arryn_active_shoot_1";
        public const string HERO_1002_EXPLODE = "Sounds/Sfx/1002/sfx_hero_miria_active_explode_1";
        

        public static string Hero1002NormalAttack()
        {
            return $"Sounds/Sfx/1002/sfx_cast_archer_{Random.Range(8, 10).ToString()}";
        }

        public static string Hero1002Die()
        {
            return $"Sounds/Sfx/1002/sfx_hero_arryn_die_{Random.Range(1,3).ToString()}";
        }

        public static string Hero1003BasicAttackForm1()
        {
            return $"Sounds/Sfx/1003/sfx_hero_thraldin_normalatk_{Random.Range(1, 3).ToString()}";
        }
        public static string Hero1003BasicAttackForm2()
        {
            return $"Sounds/Sfx/1003/sfx_item_galio_attack_{Random.Range(1, 4).ToString()}";
        }
        public const string HERO_1003_FORM1_TO_FORM2 = "Sounds/Sfx/1003/sfx_hero_thirios_active_wolf";
        public const string HERO_1003_FORM2_TO_FORM1 = "Sounds/Sfx/1003/sfx_hero_thirios_active_human";
        public const string HERO_1003_PASSIVE_1_FORM_1 = "Sounds/Sfx/1003/sfx_hero_leon_die";
        public const string HERO_1003_PASSIVE_1_FORM_2 ="Sounds/Sfx/1003/sfx_hero_leon_passive3";
        public const string HERO_1003_PASSIVE_2 = "Sounds/Sfx/1003/sfx_hero_leon_active_start";
        public const string HERO_1003_PASSIVE_3 = "Sounds/Sfx/1003/sfx_hero_zelos_skill_passive3_getsoul";
        public const string HERO_1003_DIE_FORM_1 = "Sounds/Sfx/1003/sfx_hero_thirios_die_human";
        public const string HERO_1003_DIE_FORM_2 = "Sounds/Sfx/1003/sfx_hero_thirios_die_wolf";
        //1005
        public const string HERO_1005_MELEE = "Sounds/Sfx/1005/sfx_impact_magic_2";
        public const string HERO_1005_RANGE = "Sounds/Sfx/1005/sfx_hero_miria_normalatk_shoot";
        public const string HERO_1005_ACTIVE_SKILL_CHARGE = "Sounds/Sfx/1005/sfx_active";
        public const string HERO_1005_ACTIVE_SKILL_IMPACT = "Sounds/Sfx/1005/sfx_hero_miria_passive1_1";
        public const string HERO_1005_PASSIVE1 = "Sounds/Sfx/1005/sfx_hero_miria_passive2_1";
        public const string HERO_1005_PASSIVE2 = "Sounds/Sfx/1005/sfx_hero_miria_active_shoot_2";
        public const string HERO_1005_DIE = "Sounds/Sfx/1005/sfx_hero_miria_die";
        public const string HERO_1005_REVIVE = "Sounds/Sfx/1005/sfx_revive";
        
        #endregion

        #region Enemies

        public const string ENEMY_3005_RAGE_ACTIVATE = "Sounds/Sfx/Enemy/sfx_cast_barrack_431_transform";
        public const string ENEMY_1001_DIE = "Sounds/Sfx/1001/sfx_hero_j4_die_1";
        public const string ENEMY_3007_BUFF_HEAL = "Sounds/Sfx/Enemy/sfx_enemy_orcshaman_cast_1";
        //public static string EnemySummon = $"Sounds/Sfx/Enemy/sfx_boss_warlock_summon_{Random.Range(1,2).ToString()}";

        public static string EnemySummon()
        {
           return $"Sounds/Sfx/Enemy/sfx_boss_warlock_summon_{Random.Range(1,3).ToString()}";
        }
        public const string ENEMY_3010_BREAK = "Sounds/Sfx/Enemy/sfx_cast_golem_411_explode_2";
        public const string ENEMY_3015_DIE = "Sounds/Sfx/Enemy/sfx_enemy_troll_die";
        public const string ENEMY_3015_RAGE_ACTIVATE = "Sounds/Sfx/Enemy/sfx_enemy_troll_roar";
        public const string ENEMY_3016_DIE = "Sounds/Sfx/Enemy/sfx_boss_warlock_die";
        public const string ENEMY_3016_TOWER_CONTROL = "Sounds/Sfx/Enemy/sfx_enemy_orcshaman_cast_2";

        #endregion

        #region Ambience

        public const string AMBIENCE_1_16 = "Sounds/Music/Ambience/sfx_amb_map_1_2";
        public const string AMBIENCE_17_20 = "Sounds/Music/Ambience/sfx_amb_map_6";
        public const string POISON = "Sounds/Music/Ambience/sfx_amb_map_6";
        public const string ALLIGATOR_BIT = "Sounds/Music/Ambience/sfx_hero_thirios_normalatk_wolf_3";
        public const string CAVALRY_READY = "Sounds/Music/Ambience/horse_exterior_whinny_05";
        public const string CAVALRY_ATTACK = "Sounds/Music/Ambience/horse_gallop_9";

        #endregion

        #region Spell
        //4001
        public const string SPELL_4001_APPEAR = "Sounds/Sfx/4001/sfx spell 4001 blizzard";
        //4003
        public const string SPIRIT_EXPLOSION = "Sounds/Sfx/4003/sfx_item_meteorite_1";
        //4005
        public const string SPELL_4005_HEAL = "Sounds/Sfx/4005/sfx_item_healing";
        //4006
        public const string SPELL_4006_EXPLOSION = "Sounds/Sfx/4006/sfx_envi_pinenut";
        //4011
        public const string SPELL_4011_APPEAR = "Sounds/Sfx/4011/sfx_hero_leon_active_launch_3";
        public const string SPELL_4011_DISAPPEAR = "Sounds/Sfx/4011/sfx_cast_mage_412_launch_2";
        public const string SPELL_4011_PASSIVE = "Sounds/Sfx/4011/horror_spell_01";
        //4004
        public const string SPELL_4004_MELEE_1 = "Sounds/Sfx/4004/sfx_spell_4004_star_guardian_atk_1";
        public const string SPELL_4004_MELEE_2 = "Sounds/Sfx/4004/sfx_spell_4004_star_guardian_atk_2";
        public const string SPELL_4004_APPEAR = "Sounds/Sfx/4004/sfx_spell_4004_star_guardian";

        #endregion
    }
}