using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroStatsInfoController : MonoBehaviour
    {
        [SerializeField] private Image heroAvatar;

        [SerializeField] private Image iconSpellActive;

        [SerializeField] private Image iconSpellPassive;

        [SerializeField] private Text txtLabelShowStat;

        [SerializeField] private GameObject iconShowStat;

        [SerializeField] private Button btnShowInfo;

        [SerializeField] private Transform groupMoreInfo;

        [SerializeField] private Transform groupLessInfo;

        [SerializeField] private Transform panelEffectStatChange;

        [SerializeField] private ScrollRect scrollRect;

        private int heroId;

        private HeroCacheData heroStatCache;

        private HeroCacheData heroStatCurrent;

        private void Awake()
        {
            btnShowInfo.onClick.AddListener(ShowInfoClick);
        }

        private void OnDisable()
        {
            foreach (Transform child in panelEffectStatChange)
            {
                Destroy(child.gameObject);
            }
        }

        public void ShowInfoHero(HeroCacheData heroCacheData, bool isRefresh = false)
        {
            this.heroId = heroCacheData.HeroId;

            this.heroStatCurrent = heroCacheData;

            if (heroStatCache == null || heroStatCache.HeroId != heroId)
            {
                heroStatCache = new HeroCacheData();

                heroStatCache.InsertData(heroCacheData);
            }


            if (!isRefresh)
            {
                InitUi();
            }

            
            if (!UserData.Instance.OtherUserData.isShowMoreInfoHero)
            {
                ChangeUiShowLess();
                ShowInfoLess();
                // CoroutineUtils.Instance.StartCoroutine(ShowEffectStatChange(ShowInfoLess()));
            }
            else
            {
                ChangeUiShowMore();
                // CoroutineUtils.Instance.StartCoroutine(ShowEffectStatChange(ShowInfoMore()));
            }

            CoroutineUtils.Instance.StartCoroutine(ShowEffectStatChange(ShowInfoMore()));
            
            ShowSpellInfo();

            scrollRect.content.anchoredPosition = Vector2.zero;
        }

        private void InitUi()
        {
            heroAvatar.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_large_{heroId}");
        }

        private void ShowInfoClick()
        {
            var scale = iconShowStat.transform.localScale;

            if (scale.x > 0)
            {
                ChangeUiShowMore();
                ShowInfoMore();
                UserData.Instance.OtherUserData.isShowMoreInfoHero = true;
                UserData.Instance.Save();
            }
            else
            {
                ChangeUiShowLess();
                ShowInfoLess();
                UserData.Instance.OtherUserData.isShowMoreInfoHero = false;
                UserData.Instance.Save();
            }
        }

        private void ChangeUiShowMore()
        {
            var scale = iconShowStat.transform.localScale;

            scale.x = -1;

            iconShowStat.transform.localScale = scale;

            txtLabelShowStat.text = L.button.less_info_btn;

            groupLessInfo.gameObject.SetActive(false);

            groupMoreInfo.gameObject.SetActive(true);
        }

        private void ChangeUiShowLess()
        {
            var scale = iconShowStat.transform.localScale;

            scale.x = 1;

            iconShowStat.transform.localScale = scale;

            txtLabelShowStat.text = L.button.more_info_btn;

            groupLessInfo.gameObject.SetActive(true);

            groupMoreInfo.gameObject.SetActive(false);
        }

        private Dictionary<RPGStatType, float> ShowInfoLess()
        {
            var dictStatChange = new Dictionary<RPGStatType, float>();

            var child = groupLessInfo.GetComponentsInChildren<ItemStatInfo>();

            foreach (var item in child)
            {
                if (!item.gameObject.activeSelf) continue;

                var resultCheck = ShowInfoStat(item);

                if (Math.Abs(resultCheck.Item2) > 0)
                    dictStatChange.Add(resultCheck.Item1, resultCheck.Item2);
            }

            return dictStatChange;
        }

        private Dictionary<RPGStatType, float> ShowInfoMore()
        {
            var dictStatChange = new Dictionary<RPGStatType, float>();

            foreach (Transform item in groupMoreInfo)
            {
                foreach (Transform stat in item)
                {
                    var control = stat.GetComponent<ItemStatInfo>();

                    if (control)
                    {
                        var resultCheck = ShowInfoStat(control);

                        if (Math.Abs(resultCheck.Item2) > 0.001f)
                            dictStatChange.Add(resultCheck.Item1, resultCheck.Item2);
                    }
                }
            }

            return dictStatChange;
        }

        private (RPGStatType, float) ShowInfoStat(ItemStatInfo itemStatInfo)
        {
            var resultCheck = CheckStatChange(itemStatInfo);

            var statCurr = heroStatCurrent.StatData.GetStat(itemStatInfo.Type).StatValue;

            var statNext = heroStatCurrent.StatDataNextLvl.GetStat(itemStatInfo.Type).StatValue;

            var statIncrease = statNext - statCurr;
            statIncrease = (float) Math.Round(statIncrease, 1);

            if (itemStatInfo.Type == RPGStatType.CritChance || itemStatInfo.Type == RPGStatType.CritDamage ||
                itemStatInfo.Type == RPGStatType.LifeSteal || itemStatInfo.Type == RPGStatType.CooldownReduction)
            {
                itemStatInfo.ShowInfoStat(heroStatCurrent.StatData.GetStat(itemStatInfo.Type).StatValue, statIncrease,
                    true);
            }
            else
            {
                itemStatInfo.ShowInfoStat(heroStatCurrent.StatData.GetStat(itemStatInfo.Type).StatValue, statIncrease);
            }

            return resultCheck;
        }

        private (RPGStatType, float) CheckStatChange(ItemStatInfo itemStatInfo)
        {
            var delta = 0f;

            var startCache = heroStatCache.StatData;

            var startCurr = heroStatCurrent.StatData;

            if (itemStatInfo.Type == RPGStatType.CooldownReduction)
                delta = startCurr.GetStat(itemStatInfo.Type).StatValue -
                        startCache.GetStat(itemStatInfo.Type).StatValue;
            else
                delta = startCurr.GetStat(itemStatInfo.Type).StatValue -
                        startCache.GetStat(itemStatInfo.Type).StatBaseValue;

            return (itemStatInfo.Type, delta);
        }

        private IEnumerator ShowEffectStatChange(Dictionary<RPGStatType, float> dictStatChange)
        {
            if (dictStatChange.Count <= 0) yield return null;

            foreach (var statChange in dictStatChange)
            {
                var goFx = ResourceUtils.GetVfx("UI", "fx_stat_change", Vector3.zero, Quaternion.identity,
                    panelEffectStatChange);

                if (goFx)
                {
                    goFx.GetComponent<EffectStatChangeController>()
                        .ShowEffect(statChange.Key, statChange.Value.ToString("n1"), statChange.Value > 0);
                }

                yield return new WaitForSeconds(0.1f);
            }

            heroStatCache.InsertData(heroStatCurrent);
        }

        private void ShowSpellInfo()
        {
            var spellId = heroStatCurrent.HeroItemData.spellId;

            if (spellId > 0)
            {
                iconSpellActive.sprite = ResourceUtils.GetSpriteAtlas("spell", $"{spellId}_0");
                iconSpellPassive.sprite = ResourceUtils.GetSpriteAtlas("spell", $"{spellId}_1");

                iconSpellActive.gameObject.SetActive(true);
                iconSpellPassive.gameObject.SetActive(true);
            }
            else
            {
                iconSpellActive.gameObject.SetActive(false);
                iconSpellPassive.gameObject.SetActive(false);
            }
        }
    }
}