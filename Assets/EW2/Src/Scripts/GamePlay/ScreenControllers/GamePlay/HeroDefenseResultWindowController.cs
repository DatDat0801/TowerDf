using System;
using System.Collections.Generic;
using EW2.Tools;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class HeroDefenseResultWindowProperties : WindowProperties
    {
        public int mapId;
        public int modeId;
        public int wave;
        public Reward[] rewards;

        public Dictionary<int, (float, float, int)> infoAddHeroExp;

        public HeroDefenseResultWindowProperties(int defenseMapId, int defenseModeId, int wave, Reward[] rewards,
            Dictionary<int, (float, float, int)> infoAddHeroExp)
        {
            this.mapId = defenseMapId;
            this.modeId = defenseModeId;
            this.rewards = rewards;
            this.wave = wave;

            this.infoAddHeroExp = infoAddHeroExp;
        }
    }

    public class HeroDefenseResultWindowController : AWindowController<HeroDefenseResultWindowProperties>
    {
        [SerializeField] private Transform grid;
        [SerializeField] private Button backToLobbyBtn;
        [SerializeField] private Text titleTxt;
        [SerializeField] private Text highestScoreTxt;
        [SerializeField] private Text newRecordTxt;
        [SerializeField] private Text currentScoreTxt;
        [SerializeField] private Text rewardTxt;
        [SerializeField] private List<HeroEndBattle> heroesEndBattle;

        private GridReward _gridReward;

        protected override void Awake()
        {
            base.Awake();
            InTransitionFinished += controller => { PlayAnimation(); };
            this._gridReward = new GridReward(grid);
            OutTransitionFinished += controller => {
                Time.timeScale = 1;
            
                GamePlayController.Instance.IsPause = false;
            
                _gridReward.ReturnPool();
            };
            this.backToLobbyBtn.onClick.AddListener(BackToLobbyClick);
        }

        private void BackToLobbyClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.hero_defense_result);
            GamePlayControllerBase.RequestScreenOpenOnMenu =
                new RequestScreenOpen() {ScreenId = ScreenIds.hero_defend_mode_scene};
            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        private void BackToShopClick()
        {
            GamePlayControllerBase.RequestScreenOpenOnMenu =
                new RequestScreenOpen() {ScreenId = ScreenIds.shop_scene, Properties = new ShopWindowProperties(ShopTabId.DefensivePoint)};
            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        public override void SetLocalization()
        {
            base.SetLocalization();
            if (this.titleTxt)
            {
                this.titleTxt.text = L.playable_mode.result_txt.ToUpper();
            }

            this.backToLobbyBtn.GetComponentInChildren<Text>().text = L.playable_mode.lobby_txt;
            
            if (this.highestScoreTxt)
            {
                var highestScore = UserData.Instance.UserHeroDefenseData.GetHighestScore();
                this.highestScoreTxt.text =
                    string.Format(L.playable_mode.best_record_waves_txt, (highestScore).ToString());
            }

            if (this.newRecordTxt)
            {
                this.newRecordTxt.text = L.playable_mode.new_record_txt;
                this.newRecordTxt.transform.parent.gameObject.SetActive(UserData.Instance.UserHeroDefenseData.IsNewRecord());
            }

            if (this.currentScoreTxt)
            {
                this.currentScoreTxt.text = string.Format(L.playable_mode.current_record_waves_txt, (Properties.wave).ToString());
            }

            if (this.rewardTxt)
            {
                this.rewardTxt.text = L.gameplay.rewards;
            }
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            foreach (var hero in heroesEndBattle)
            {
                hero.gameObject.SetActive(false);
            }
            this.grid.DespawnAllChildren();
            
            HeroTrialRemainCheck();
            SetLocalization();
        }

        void HeroTrialRemainCheck()
        {
            var userHeroDefenseData = UserData.Instance.UserHeroDefenseData;
            if (userHeroDefenseData.IsOutOfDFSPTrial() && !userHeroDefenseData.CheckDefensePointUnlocked(userHeroDefenseData.defensePointId))
            {
                userHeroDefenseData.showTrialPopupOneTime = true;
                UserData.Instance.Save();
                var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, L.playable_mode.no_more_trial_txt,
                    PopupNoticeWindowProperties.PopupType.OneOption, L.button.go_to_shop_btn, BackToShopClick);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
            }
        }

        private void PlayAnimation()
        {
            var heroes = GamePlayControllerBase.heroList;
            var countHeroes = heroes.Count;

            if (countHeroes > 0)
            {
                for (int i = 0; i < countHeroes; i++)
                {
                    if (!UserData.Instance.UserHeroData.CheckHeroUnlocked(heroes[i]))
                    {
                        continue;
                    }

                    if (Properties.infoAddHeroExp.ContainsKey(heroes[i]))
                    {
                        var (originExpPercentage, currentExpPercentage, numberLevelUp) =
                            Properties.infoAddHeroExp[heroes[i]];
                        heroesEndBattle[i].Init(heroes[i], true, originExpPercentage, currentExpPercentage,
                            numberLevelUp);
                    }
                    else
                    {
                        heroesEndBattle[i].Init(heroes[i], false);
                    }
                }
            }

            //SetStateAnimation(true);
            //DoPlayAnimation();

            //skeletonGraphic.gameObject.SetActive(true);

            //StartCoroutine(IPlayAnimation(star));
            _gridReward.SetData(Properties.rewards);
        }
    }
}