using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EW2.Tutorial.General;
using EW2.Tutorial.UI;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class GamePlayWindowProperties : WindowProperties
    {
        public int campaignId;
        public GameMode gameMode;

        public GamePlayWindowProperties(int data, GameMode mode = GameMode.CampaignMode)
        {
            campaignId = data;
            this.gameMode = mode;
        }
    }

    public class GamePlayWindowCommon : AWindowController<GamePlayWindowProperties>
    {
        [SerializeField] protected List<HeroButton> listButtonHero;
        [SerializeField] protected List<SkillButton> listButtonSkill;
        [SerializeField] protected Button btnSpawnHero;
        [SerializeField] protected Button btnSpawnEnemy;
        [SerializeField] protected Button btnSpawnDummy;
        [SerializeField] protected Button btnSpawnEnemyLine0;
        [SerializeField] protected Button btnUpdateGold;
        [SerializeField] protected Button btnCheatResult;
        [SerializeField] protected Button btnCheatHp;

        [SerializeField] protected Button btnSetting;
        [SerializeField] protected Button btnSkipTutorial;
        [SerializeField] protected Button btnCheat;
        [SerializeField] protected ButtonNewEnemyController btnNewEnemy;
        [SerializeField] protected InputField fieldIdHero;
        [SerializeField] protected InputField hpField;
        [SerializeField] protected InputField fieldIdEnemy;
        [SerializeField] protected InputField fieldIdDummy;
        [SerializeField] protected InputField fieldEnemyLine0;
        [SerializeField] protected InputField fieldGold;
        [SerializeField] protected InputField fieldCheatResult;
        [SerializeField] protected Text lbGold;
        [SerializeField] protected Text lbHealth;
        [SerializeField] protected Text lbWave;
        [SerializeField] protected GameObject cheat;
        [SerializeField] protected SpellBarController spellBarController;
        protected int _numberHeroInScene;

        protected override void Awake()
        {
            base.Awake();
            lbGold.text = "0";
            // cheat
            btnSpawnHero.onClick.AddListener(SpawnHeroClick);
            btnSpawnEnemy.onClick.AddListener(SpawnEnemyClick);
            btnSpawnDummy.onClick.AddListener(SpawnDummyClick);
            btnSpawnEnemyLine0.onClick.AddListener(SpawnEnemyLine0Click);
            btnUpdateGold.onClick.AddListener(GoldChange);
            btnCheatResult.onClick.AddListener(CheatResult);

            btnSkipTutorial.onClick.AddListener(OnSkipTutorialClick);
            this.btnCheat.onClick.AddListener(CheatClick);
            if (btnCheatHp)
            {
                btnCheatHp.onClick.AddListener(CheatHp);
            }

            btnSetting.onClick.AddListener(OnClickPause);

            InTransitionFinished += controller => { EventManager.EmitEvent(GamePlayEvent.CloseLoading); };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (this.btnCheat)
                this.btnCheat.gameObject.SetActive(GameLaunch.isCheat);
            this.cheat.SetActive(false);
        }

        protected virtual void SpawnHeroList()
        {
            _numberHeroInScene = 0;

            foreach (var heroId in GamePlayControllerBase.heroList)
            {
                SpawnHero(heroId);
            }

            InitializeSpell();
            
        }

        protected void InitializeSpell()
        {
            spellBarController.Initialize();
        }

        public void OnClickPause()
        {
            GamePlayUIManager.Instance.CloseCurrentUI(true);

            GamePlayUIManager.Instance.CloseAllUI();

            GamePlayController.Instance.IsPause = true;

            Time.timeScale = 0;

            UIFrame.Instance.OpenWindow(ScreenIds.game_pause);
        }

        protected async void PlayAmbienceMusic(int mapId)
        {
            var backgroundMusic = ResourceSoundManager.GetMusicMapByLvl(mapId);
            EazySoundManager.PlayMusic(ResourceSoundManager.GetAudioMusic(backgroundMusic),
                EazySoundManager.GlobalMusicVolume, true, false);

            await UniTask.Delay(2500);
            var nameMusic = ResourceSoundManager.GetAmbienceMusicNameByLevel(mapId);
            EazySoundManager.PlaySound(ResourceUtils.LoadSound(nameMusic), true);
        }

        protected void SetUpCamera()
        {
            var borderMap = GamePlayController.Instance.SpawnController.MapController.BorderMap;

            GamePlayController.Instance.GetCameraController().Setup(borderMap.topBorder.position.y,
                borderMap.downBorder.position.y,
                borderMap.leftBorder.position.x, borderMap.rightBorder.position.x);
        }

        #region Button Cheat

        protected void SpawnHeroClick()
        {
            if (string.IsNullOrEmpty(fieldIdHero.text))
            {
                var defaultHeroId = 1001;
                SpawnHero(defaultHeroId);
            }
            else
            {
                var heroId = Int32.Parse(fieldIdHero.text);
                SpawnHero(heroId);
            }
        }


        public void SpawnHero(int heroId)
        {
            var heroButton = listButtonHero[_numberHeroInScene];
            var skillButton = listButtonSkill[_numberHeroInScene];

            var hero = GamePlayController.Instance.SpawnController.SpawnHero(heroButton, skillButton, heroId.ToString(),
                _numberHeroInScene);


            GamePlayControllerBase.Instance.AddHero(hero);


            if (hero != null)
            {
                heroButton.InitData(hero);
                heroButton.gameObject.SetActive(true);


                //TODO Refactor
                if (heroId == 1003)
                {
                    skillButton.InitData(hero, true);
                }
                else
                {
                    skillButton.InitData(hero);
                }

                skillButton.gameObject.SetActive(true);

                _numberHeroInScene++;
            }
            else
            {
                throw new Exception("Can't spawn Hero: " + fieldIdHero.text);
            }
        }

        public void SpawnEnemyClick()
        {
            GamePlayController.Instance.SpawnController.SpawnEnemyRandomLand(fieldIdEnemy.text);
        }

        protected void CheatClick()
        {
            if (this.cheat.activeSelf)
                this.cheat.SetActive(false);
            else
                this.cheat.SetActive(true);
        }

        protected void OnSkipTutorialClick()
        {
            TutorialManager.Instance.ExecuteSkipTutorial();
            var tutUi = FindObjectOfType<TutorialUI>();
            if (tutUi)
            {
                tutUi.DialogUI.HideDialog();
                tutUi.FocusUI.HideFocus();
            }

            GamePlayController.Instance.ShowEndGame(true, 3);
        }

        public void SpawnDummyClick()
        {
            GamePlayController.Instance.SpawnController.SpawnDummy(fieldIdDummy.text);
        }

        public void SpawnEnemyLine0Click()
        {
            GamePlayController.Instance.SpawnController.SpawnEnemyLine0(fieldEnemyLine0.text);
        }

        public void GoldChange()
        {
            var text = fieldGold.text;
            lbGold.text = "Gold: " + text;
            int value = int.Parse(text);
            GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.Gold,
                GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold));
            GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, value);
        }

        protected void CheatResult()
        {
            var star = int.Parse(fieldCheatResult.text);

            GamePlayController.Instance.ShowEndGame(star > 0, star);
        }

        protected void CheatHp()
        {
            if (string.IsNullOrEmpty(this.hpField.text))
            {
                GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.LifePoint, 99999);
                return;
            }

            try
            {
                var hp = int.Parse(this.hpField.text);
                GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.LifePoint,
                    GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold) - 1);
                GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.LifePoint, hp);
            }
            catch (Exception e)
            {
                GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.LifePoint, 99999);
            }
        }
        

        #endregion
    }
}