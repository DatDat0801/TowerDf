using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class TournamentResultWindowProperties : WindowProperties
    {
        public int tournamentMapId;
        public bool isNewRecord;
        public Dictionary<int, (float, float, int)> infoAddHeroExp;

        public TournamentResultWindowProperties(int tournamentMapId, Dictionary<int, (float, float, int)> infoAddHeroExp, bool isNewRecord)
        {
            this.tournamentMapId = tournamentMapId;

            this.infoAddHeroExp = infoAddHeroExp;
            this.isNewRecord = isNewRecord;
        }
    }
    public class TournamentResultWindowController : AWindowController<TournamentResultWindowProperties>
    {
        [SerializeField] private Text titleTxt;
        [SerializeField] private Text newRecordTxt;
        [SerializeField] private Text scoreTxt;
        [SerializeField] private Text backToLobbyLabel;
        [SerializeField] private GameObject newRecord;
        [SerializeField] private Button backToLobbyBtn;
        
        [SerializeField] private List<HeroEndBattle> heroesEndBattle;
        
        protected override void Awake()
        {
            base.Awake();
            InTransitionFinished += controller => { PlayAnimation(); };

            OutTransitionFinished += controller => {
                Time.timeScale = 1;
            
                GamePlayController.Instance.IsPause = false;

            };
            this.backToLobbyBtn.onClick.AddListener(BackToLobbyClick);
        }
        private void BackToLobbyClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.tournament_result);
            GamePlayControllerBase.RequestScreenOpenOnMenu =
                new RequestScreenOpen() {ScreenId = ScreenIds.tournament_lobby};
            LoadSceneUtils.LoadScene(SceneName.Start);
        }
        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            foreach (var hero in heroesEndBattle)
            {
                hero.gameObject.SetActive(false);
            }
            SetLocalization();
        }

        public override void SetLocalization()
        {
            base.SetLocalization();
            if (this.titleTxt)
            {
                this.titleTxt.text = L.playable_mode.result_txt.ToUpper();
            }

            if (this.newRecord)
            {
                this.newRecordTxt.text = L.playable_mode.new_record_txt;
                this.newRecord.SetActive(Properties.isNewRecord);
            }

            if (this.scoreTxt)
            {
                this.scoreTxt.text = $"{L.gameplay.score}: {GamePlayControllerBase.Instance.SpawnController.KilledEnemy.ToString()}";
            }

            if (this.backToLobbyLabel)
            {
                this.backToLobbyLabel.text = L.playable_mode.lobby_txt;;
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
        }
    }
}