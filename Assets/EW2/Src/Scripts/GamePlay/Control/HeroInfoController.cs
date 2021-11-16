using System.Linq;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroInfoController : MonoBehaviour
    {
        [SerializeField] private Image avatarHero;

        [SerializeField] private Text nameHero;

        [SerializeField] private Text tagHero;

        [SerializeField] private Text statHealth;

        [SerializeField] private Text statClass;

        [SerializeField] private Text statMeleeAtk;

        [SerializeField] private Text statRangeAtk;

        [SerializeField] private Text statArmor;

        [SerializeField] private Text statResist;

        [SerializeField] private Text statAtkSpeed;

        [SerializeField] private Text statMoveSpeed;

        private HeroBase heroBase;
        readonly Vector3 anchoredPos = new Vector3(65f, -29f, 0);
        readonly Vector2 anchoredMin = new Vector2(0f, 1f);
        readonly Vector2 pivot = new Vector2(0.5f, 1f);

        public void ShowInfo(HeroBase owner)
        {
            if (heroBase && owner.HeroStatBase.id != heroBase.HeroStatBase.id)
            {
                HideInfoPopup();

                return;
            }

            SetPos();

            this.heroBase = owner;

            var statBase = heroBase.Stats;

            nameHero.text = Ultilities.GetNameHero( heroBase.HeroStatBase.id);

            tagHero.text = Ultilities.GetTagHero( heroBase.HeroStatBase.id);

            avatarHero.sprite = ResourceUtils.GetSpriteAtlas("hero_icons", "hero_icon_info_" + heroBase.HeroStatBase.id);
            
            statHealth.text = statBase.GetStat(RPGStatType.Health).StatValue + "";

            statClass.text = heroBase.HeroStatBase.priorityTarget.Contains(PriorityTargetType.Fly)
                ? L.heroes.hero_class_ranged
                : L.heroes.hero_class_melee;

            statMeleeAtk.text = statBase.GetStat(RPGStatType.Damage).StatValue+ "";

            statRangeAtk.text = statBase.GetStat(RPGStatType.RangeDetect).StatValue+ "";

            statArmor.text = statBase.GetStat(RPGStatType.Armor).StatValue + "";

            statResist.text = statBase.GetStat(RPGStatType.Resistance).StatValue + "";

            statAtkSpeed.text = statBase.GetStat(RPGStatType.AttackSpeed).StatValue + "";

            statMoveSpeed.text = statBase.GetStat(RPGStatType.MoveSpeed).StatValue + "";

            gameObject.SetActive(true);
        }

        public void HideInfoPopup()
        {
            heroBase = null;

            LeanPool.Despawn(gameObject);
        }


        private void SetPos()
        {
            var rect = GetComponent<RectTransform>();

            rect.anchoredPosition = anchoredPos;

            rect.anchorMin = anchoredMin;

            rect.anchorMax = Vector2.one;

            rect.pivot = pivot;

            rect.offsetMin = new Vector2(500, rect.offsetMin.y);

            rect.offsetMax = new Vector2(-340f, rect.offsetMax.y);

            transform.localScale = Vector3.one;
        }
    }
}