using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hellmade.Sound;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class GameVictoryWindowProperties : WindowProperties
    {
        public int star;
        public int modedId;
        public int worldId;
        public int stageId;

        public Reward[] rewards;

        //
        public Dictionary<int, (float, float, int)> infoAddHeroExp;

        public GameVictoryWindowProperties(int modeId, int worldId, int stageId, int star, Reward[] rewards,
            Dictionary<int, (float, float, int)> infoAddHeroExp)
        {
            this.star = star;

            this.rewards = rewards;

            this.infoAddHeroExp = infoAddHeroExp;

            this.modedId = modeId;
            this.worldId = worldId;
            this.stageId = stageId;
        }
    }

    public class GameVictoryWindowController : AWindowController<GameVictoryWindowProperties>
    {
        public enum ButtonState
        {
            NextStage,
            Replay,
            Home
        }

        [SerializeField] private Transform grid;
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] private Button btnNextStage;
        [SerializeField] private Button btnReplay;
        [SerializeField] private Button btnHome;

        [SerializeField] private GameObject txtRewards;
        [SerializeField] private List<HeroEndBattle> heroesEndBattle;

        private ButtonState buttonState;

        private GridReward gridReward;

        protected override void Awake()
        {
            base.Awake();


            gridReward = new GridReward(grid);

            btnNextStage.onClick.AddListener(OnPlaySoundClick);
            btnNextStage.onClick.AddListener(OnNextStage);

            btnReplay.onClick.AddListener(OnPlaySoundClick);
            btnReplay.onClick.AddListener(OnReplay);
            btnHome.onClick.AddListener(OnHome);

            // btnReplay.GetComponentInChildren<Text>().text = L.button.btn_replay;
            // btnNextStage.GetComponentInChildren<Text>().text = L.button.btn_next;

            OutTransitionFinished += controller => {
                Time.timeScale = 1;

                GamePlayController.Instance.IsPause = false;

                gridReward.ReturnPool();
            };

            InTransitionFinished += controller => { PlayAnimation(Properties.star); };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            foreach (var hero in heroesEndBattle)
            {
                hero.gameObject.SetActive(false);
            }

            SetStateAnimation(false);

            skeletonGraphic.AnimationState.Event -= HandleEvent;
            skeletonGraphic.GetComponent<RectTransform>().localPosition = new Vector3(0, 2000, 0);
            skeletonGraphic.gameObject.SetActive(false);
        }

        private void PlayAnimation(int star)
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

            SetStateAnimation(true);
            DoPlayAnimation();

            skeletonGraphic.gameObject.SetActive(true);

            StartCoroutine(IPlayAnimation(star));
        }

        IEnumerator IPlayAnimation(int star)
        {
            yield return new WaitForEndOfFrame();

            skeletonGraphic.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            string appear = $"appear_{star.ToString()}_star";
            string idle = $"idle_{star.ToString()}_star";
            skeletonGraphic.AnimationState.SetAnimation(0, appear, false);
            skeletonGraphic.AnimationState.AddAnimation(0, idle, true, 0);
            skeletonGraphic.AnimationState.Event += HandleEvent;
        }

        public virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            Debug.Log("Event fired! " + e.Data.Name);
            switch (e.Data.Name)
            {
                case "appear_avatar_hero":
                    break;
                case "appear_next_stage":
                    break;
                case "appear_replay":
                    break;
                case "appear_reward":

                    gridReward.SetData(Properties.rewards);
                    grid.localScale = Vector3.zero;
                    grid.DOScale(1, 0.4f).SetEase(Ease.OutBounce).SetUpdate(true);
                    break;
                case "sfx_star_3":
                case "sfx_star_2":
                case "sfx_star_1":
                    var audioClip = ResourceUtils.LoadSound(SoundConstant.STAR_RESULT);
                    EazySoundManager.PlaySound(audioClip);
                    break;
            }
        }

        private void OnPlaySoundClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
        }

        private void OnNextStage()
        {
            //check unlocked nightmare
            var winStage = UserData.Instance.CampaignData.HighestResultLevel();
            var isShowNoticeUnlocked = UserData.Instance.CampaignData.IsShowUnlockNightmareNotice(0);
            if (winStage == GameConfig.MAX_STAGE_ID_WORLD_1 + 1 && isShowNoticeUnlocked == false)
            {
                UserData.Instance.CampaignData.AddNewWorldNightmareUnlocked();
                UserData.Instance.Save();
                return;
            }

            //execute next button
            buttonState = ButtonState.NextStage;
            GamePlayControllerBase.playMode = PlayMode.Campaign;

            var nextCampaign = GameContainer.Instance.Get<MapDataBase>().GetNextCampaign(
                GamePlayController.Instance.WorldId,
                GamePlayController.Instance.MapId);
            int maxCampaign = GameContainer.Instance.Get<MapDataBase>().GetNextCampaign(
                GamePlayController.Instance.WorldId, GameConfig.MAX_STAGE_ID_WORLD_1);
            if (nextCampaign > 0)
            {
                if (nextCampaign >= maxCampaign)
                {
                    nextCampaign = 1;
                }
                GamePlayControllerBase.CampaignId = nextCampaign;
            }

            FirebaseLogic.Instance.ButtonClick("stage", "next", Properties.stageId);
            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        void OnNightmareUnlocked()
        {
            var content = string.Format(L.popup.unlock_nightmare_txt, L.stages.difficult_nightmare);
            var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, content,
                PopupNoticeWindowProperties.PopupType.OneOption, L.button.go_to_btn, AutoNextNightmareStage);

            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);

            void AutoNextNightmareStage()
            {
                buttonState = ButtonState.NextStage;
                GamePlayControllerBase.playMode = PlayMode.Campaign;
                var highestStageUnlocked = UserData.Instance.CampaignData.HighestStageUnlocked(1);

                var nextCampaign = MapCampaignInfo.GetCampaignId(0, highestStageUnlocked, 1);
                GamePlayControllerBase.CampaignId = nextCampaign;

                FirebaseLogic.Instance.ButtonClick("stage", "next", Properties.stageId);
                LoadSceneUtils.LoadScene(SceneName.Start);
            }
        }

        private void OnHome()
        {
            //FirebaseLogic.Instance.ButtonClick("stage", "home", Properties.stageId);
            buttonState = ButtonState.Home;

            GamePlayControllerBase.playMode = PlayMode.None;

            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        private void OnReplay()
        {
            //FirebaseLogic.Instance.ButtonClick("stage", "replay", Properties.stageId);
            buttonState = ButtonState.Replay;

            GamePlayControllerBase.playMode = PlayMode.ReplayCampaign;

            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        void SetStateAnimation(bool enable)
        {
            btnNextStage.gameObject.SetActive(enable);
            btnReplay.gameObject.SetActive(enable);
            btnHome.gameObject.SetActive(enable);
            txtRewards.SetActive(enable);
        }

        private void DoPlayAnimation()
        {
            int state = Properties.stageId == GameConfig.MAX_STAGE_ID_WORLD_1 ? 0 : Properties.stageId;
            bool isNextStageAvailable =
                UserData.Instance.CampaignData.IsUnlockedHardStage(Properties.worldId, state + 1);
            if (Properties.star == 3 && Properties.modedId == 1 && !isNextStageAvailable)
            {
                btnNextStage.gameObject.SetActive(false);
                btnReplay.gameObject.SetActive(true);
                btnHome.gameObject.SetActive(true);
            }
            else
            {
                btnNextStage.gameObject.SetActive(true);
                btnHome.gameObject.SetActive(false);
                btnReplay.gameObject.SetActive(true);
            }

            txtRewards.SetActive(true);

            if (btnReplay.gameObject.activeInHierarchy)
            {
                btnReplay.GetComponent<DOTweenAnimation>().DORestart();
            }

            if (btnHome.gameObject.activeInHierarchy)
            {
                btnHome.GetComponent<DOTweenAnimation>().DORestart();
            }

            if (btnNextStage.gameObject.activeInHierarchy)
            {
                btnNextStage.GetComponent<DOTweenAnimation>().DORestart();
            }

            if (txtRewards.gameObject.activeInHierarchy)
            {
                txtRewards.GetComponent<DOTweenAnimation>().DORestart();
            }
        }
    }
}
