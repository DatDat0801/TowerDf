using System;
using DG.Tweening;
using Hellmade.Sound;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class GameDefeatWindowProperties : WindowProperties
    {
        public int modedId;
        public int worldId;
        public int stageId;

        public GameDefeatWindowProperties(int modeId, int worldId, int stageId)
        {
            this.modedId = modeId;
            this.worldId = worldId;
            this.stageId = stageId;
        }
    }
    public class GameDefeatWindowController : AWindowController<GameDefeatWindowProperties>
    {
        private enum ButtonState
        {
            Home,
            Replay,
            HeroRoom,
            Upgrade,
            Shop
        }

        [SerializeField] private SkeletonGraphic skeletonGraphic;

        [SerializeField] private Button btnHome;

        [SerializeField] private Button btnReplay;

        [SerializeField] private Button btnHero;

        [SerializeField] private Button btnUpgrade;

        [SerializeField] private Button btnShop;

        [SerializeField] private GameObject txtStrengthen;

        private ButtonState buttonState;

        protected override void Awake()
        {
            base.Awake();

            btnHome.onClick.AddListener(OnPlaySound);
            btnHome.onClick.AddListener(OnHome);

            btnReplay.onClick.AddListener(OnPlaySound);
            btnReplay.onClick.AddListener(OnReplay);

            btnHero.onClick.AddListener(OnPlaySound);
            btnHero.onClick.AddListener(OnHeroRoom);

            btnUpgrade.onClick.AddListener(OnPlaySound);
            btnUpgrade.onClick.AddListener(OnUpgradeClick);

            btnShop.onClick.AddListener(OnPlaySound);
            btnShop.onClick.AddListener(OnShopClick);

            //txtStrengthen.GetComponent<Text>().text = L.gameplay.upgrade_power;
        }

        protected override void OnPropertiesSet()
        {
            //Debug.Log("DEFEAT");

            base.OnPropertiesSet();

            PlayAnimation();

            btnReplay.GetComponentInChildren<Text>().text = L.button.btn_replay.ToUpper();

            btnHome.GetComponentInChildren<Text>().text = L.button.home_btn.ToUpper();
        }

        private void PlayAnimation()
        {
            SetActive();

            skeletonGraphic.AnimationState.SetAnimation(0, "appear", false);
            skeletonGraphic.AnimationState.AddAnimation(0, "idle", true, 0);
        }

        private void OnPlaySound()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
        }

        private void OnHome()
        {
            //FirebaseLogic.Instance.ButtonClick("stage", "home", Properties.stageId);
            buttonState = ButtonState.Home;

            GamePlayController.playMode = PlayMode.None;

            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        private void OnReplay()
        {
            //FirebaseLogic.Instance.ButtonClick("stage", "replay", Properties.stageId);
            buttonState = ButtonState.Replay;

            GamePlayController.playMode = PlayMode.ReplayCampaign;
            
            GamePlayController.gameMode = GameMode.CampaignMode;
            
            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start);
        }

        void OnShopClick()
        {
            buttonState = ButtonState.Shop;

            GamePlayController.playMode = PlayMode.None;

            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start, () => { UIFrame.Instance.OpenWindow(ScreenIds.shop_scene); });
        }

        void OnUpgradeClick()
        {
            buttonState = ButtonState.Upgrade;

            GamePlayController.playMode = PlayMode.None;

            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start,
                () => { UIFrame.Instance.OpenWindow(ScreenIds.tower_upgrade_system); });
        }

        private void OnHeroRoom()
        {
            buttonState = ButtonState.HeroRoom;

            GamePlayController.playMode = PlayMode.None;

            Time.timeScale = 1;

            LoadSceneUtils.LoadScene(SceneName.Start, () =>
            {
                var data = new HeroRoomWindowProperties((int) HeroType.Jave);

                UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene, data);
            });
        }

        private void ShowComingSoon()
        {
            UIFrame.Instance.ShowPanel(ScreenIds.toast_short_noti_panel,
                new ToastShortNotiPanelProperties(L.common.coming_soon));
        }

        private void SetActive()
        {
            txtStrengthen.GetComponent<DOTweenAnimation>().DORestart();
        }
    }
}