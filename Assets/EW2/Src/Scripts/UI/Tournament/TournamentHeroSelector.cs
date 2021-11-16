using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class TournamentHeroSelector : MonoBehaviour
    {
        [SerializeField] private GameObject unitUITemplate;
        [SerializeField] private GameObject unitUIRectTemplate;

        //BUFF
        [SerializeField] private Transform buffItems;
        [SerializeField] private Text buffText;

        [SerializeField] private Text buffStatText;

        //NERF
        [SerializeField] private Transform nerfItems;
        [SerializeField] private Text nerfText;

        [SerializeField] private Text nerfStatText;

        //BAN
        [SerializeField] private Transform banItems;
        [SerializeField] private Text banText;
        [SerializeField] private Text banStatText;

        //SELECT HEROES
        [SerializeField] private HeroSelectUIItem[] heroSelectUis;
        [SerializeField] private Text selectHeroTxt;

        //SELECT TOWER
        [SerializeField] private TowerSelectUIItem[] towerSelectUis;

        [SerializeField] private Text selectTowerTxt;

        //START
        [SerializeField] private Text starTxt;
        [SerializeField] private Text ticketTxt;

        [SerializeField] private Button startBtn;

        //MAP
        [SerializeField] private Text mapNameTxt;
        [SerializeField] private Image miniMap;

        private void Awake()
        {
            SetListeners();
        }

        private void OnEnable()
        {
            SetMiniMap();
            RepaintHeroesUI();
            ShowLocalize();
            ShowBuffInfo();
            ShowBanInfo();
            ShowNerfInfo();
            SetTowers();
        }

        void SetTowers()
        {
            for (int i = 1; i <= 4; i++)
            {
                towerSelectUis[i - 1].Repaint(2000 + i);
            }
        }

        private void ShowLocalize()
        {
            this.buffText.text = L.playable_mode.tournament_buff_txt;
            this.nerfText.text = $"<color=#ff3c3c>{L.playable_mode.tournament_nerf_txt}</color>";
            this.banText.text = $"<color=#ff3c3c>{L.playable_mode.tournament_ban_txt}</color>";
            this.selectHeroTxt.text = L.playable_mode.hero_selection_txt;
            this.selectTowerTxt.text = L.playable_mode.tower_selection_txt;
            if (this.starTxt)
            {
                this.starTxt.text = L.button.btn_start;
            }

            if (this.ticketTxt)
            {
                this.ticketTxt.text = "1";
            }
        }

        public void RepaintHeroesUI()
        {
            var userData = UserData.Instance.TournamentData;
            for (var i = 0; i < this.heroSelectUis.Length; i++)
            {
                if (userData.listHeroSelected.Count < i + 1) { this.heroSelectUis[i].ResetUI(); }
                else { this.heroSelectUis[i].Repaint(userData.listHeroSelected[i]); }
            }
        }

        public void SetMiniMap()
        {
            if (this.miniMap)
            {
                var userData = UserData.Instance.TournamentData;
                var tournamentData = GameContainer.Instance.GetTournamentData();
                var map = tournamentData.GetTournamentMap(userData.currentMapId);
                miniMap.sprite =
                    ResourceUtils.GetSprite("MiniMaps", $"map_{map.worldId.ToString()}_{map.mapId.ToString()}");
                this.mapNameTxt.text = Localization.Current.Get("stages",
                    $"stage_name_{map.worldId + 1}_{map.mapId + 1}").ToUpper();
            }
        }

        void SetListeners()
        {
            this.startBtn.onClick.AddListener(StartClick);
        }

        private void StartClick()
        {
            var userData = UserData.Instance.TournamentData;
            if (userData.listHeroSelected.Count <= 0)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.tournament_select_heroes_popup);
                return;
            }

            var ticket = UserData.Instance.GetMoney(MoneyType.TournamentTicket);
            if (ticket < 1)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.popup_out_of_tournament_ticket);
                return;
            }

            var heroes = UserData.Instance.TournamentData.GetSelectedHeroItems();
            var bannedSpellId = UserData.Instance.TournamentData.spellBanId;
            foreach (HeroItem heroItem in heroes)
            {
                if (heroItem.spellId == bannedSpellId)
                {
                    if (!IsAskToday())
                    {
                        var properties = new SpellBanTournamentWindowProperties(heroItem.heroId, GoToGamePlay);
                        UIFrame.Instance.OpenWindow(ScreenIds.popup_tournament_spell_banned, properties);
                        return;
                    }

                    break;
                }
            }
    
            
            GoToGamePlay();
        }


        private void GoToGamePlay()
        {
            var userData = UserData.Instance.TournamentData;
            FirebaseLogic.Instance.StartATournamentGame(userData.listHeroSelected, userData.spellBanId, userData.heroNerfId, -1, userData.buffStatId, userData.nerfStatId);
            UserData.Instance.SubMoney(MoneyType.TournamentTicket, 1L, "tournament_lobby", "start_button");
            ConfigGamePlay();

            LoadSceneUtils.LoadScene(SceneName.TournamentGameplay);
        }

        private void ConfigGamePlay()
        {
            UserData.Instance.UserHeroDefenseData.battleId++;

            GamePlayControllerBase.CampaignId = UserData.Instance.TournamentData.currentMapId;

            GamePlayControllerBase.gameMode = GameMode.TournamentMode;

            GamePlayControllerBase.heroList.Clear();

            GamePlayControllerBase.heroList.AddRange(UserData.Instance.TournamentData.GetListHeroes());
        }

        #region Ban Buff Nerf

        private void ShowBuffInfo()
        {
            foreach (Transform child in buffItems)
            {
                child.gameObject.SetActive(false);
            }

            var listHeroBuff = UserData.Instance.TournamentData.GetHeroIdBuff();
            for (int i = 0; i < listHeroBuff.Length; i++)
            {
                GameObject item = GetItem(this.buffItems);

                if (item != null)
                {
                    var scriptControl = item.GetComponent<UnitViewUI>();
                    if (scriptControl != null)
                    {
                        scriptControl.ShowUi(OptionShowInfoTournament.Buff, listHeroBuff[i]);
                        item.SetActive(true);
                    }
                }
                else
                {
                    item = Instantiate(this.unitUITemplate, this.buffItems, false);
                    if (item)
                    {
                        var scriptControl = item.GetComponent<UnitViewUI>();
                        if (scriptControl != null)
                        {
                            scriptControl.ShowUi(OptionShowInfoTournament.Buff, listHeroBuff[i]);
                        }
                    }
                }
            }

            this.buffStatText.text = GetDescBuff(UserData.Instance.TournamentData.buffStatId);
        }

        private void ShowNerfInfo()
        {
            foreach (Transform child in this.nerfItems)
            {
                child.gameObject.SetActive(false);
            }


            GameObject item = GetItem(this.nerfItems);

            if (item != null)
            {
                var scriptControl = item.GetComponent<UnitViewUI>();
                if (scriptControl != null)
                {
                    scriptControl.ShowUi(OptionShowInfoTournament.Nerf, UserData.Instance.TournamentData.heroNerfId);
                    item.SetActive(true);
                }
            }
            else
            {
                item = Instantiate(this.unitUITemplate, this.nerfItems, false);
                if (item)
                {
                    var scriptControl = item.GetComponent<UnitViewUI>();
                    if (scriptControl != null)
                    {
                        scriptControl.ShowUi(OptionShowInfoTournament.Nerf,
                            UserData.Instance.TournamentData.heroNerfId);
                    }
                }
            }

            this.nerfStatText.text = GetDescNerf(UserData.Instance.TournamentData.nerfStatId);
        }

        private void ShowBanInfo()
        {
            foreach (Transform child in this.banItems)
            {
                child.gameObject.SetActive(false);
            }


            GameObject item = GetItem(this.banItems);

            if (item != null)
            {
                var scriptControl = item.GetComponent<UnitViewSpellUi>();
                if (scriptControl != null)
                {
                    scriptControl.ShowUi(OptionShowInfoTournament.Ban, UserData.Instance.TournamentData.spellBanId);
                    item.SetActive(true);
                }
            }
            else
            {
                item = Instantiate(this.unitUIRectTemplate, this.banItems, false);
                if (item)
                {
                    var scriptControl = item.GetComponent<UnitViewSpellUi>();
                    if (scriptControl != null)
                    {
                        scriptControl.ShowUi(OptionShowInfoTournament.Ban,
                            UserData.Instance.TournamentData.spellBanId);
                    }
                }
            }

            this.banStatText.text = "";
        }

        private GameObject GetItem(Transform panel)
        {
            foreach (Transform child in panel)
            {
                if (!child.gameObject.activeSelf)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        private string GetDescBuff(int buffId)
        {
            var desc = Localization.Current.Get("playable_mode", $"tournament_buff_{buffId}");
            var buffData = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentBuffConfig>();

            var valueBuff = 0;
            for (int i = 0; i < buffData.tournamentBuffDatas.Length; i++)
            {
                if (buffData.tournamentBuffDatas[i].statId == buffId)
                {
                    if (buffData.tournamentBuffDatas[i].ratioBonus > 0)
                    {
                        valueBuff = (int)(buffData.tournamentBuffDatas[i].ratioBonus * 100);
                    }
                    else
                    {
                        valueBuff = buffData.tournamentBuffDatas[i].valueBonus;
                    }

                    break;
                }
            }

            desc = string.Format(desc, valueBuff);

            return desc;
        }

        private string GetDescNerf(int nerfId)
        {
            var desc = Localization.Current.Get("playable_mode", $"tournament_nerf_{nerfId}");
            var buffData = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentNerfConfig>();

            var valueBuff = 0;
            for (int i = 0; i < buffData.tournamentNerfDatas.Length; i++)
            {
                if (buffData.tournamentNerfDatas[i].statId == nerfId)
                {
                    if (buffData.tournamentNerfDatas[i].ratioBonus > 0)
                    {
                        valueBuff = (int)(buffData.tournamentNerfDatas[i].ratioBonus * 100);
                    }
                    else
                    {
                        valueBuff = buffData.tournamentNerfDatas[i].valueBonus;
                    }

                    break;
                }
            }

            desc = string.Format(desc, valueBuff);

            return desc;
        }

        #endregion

        bool IsAskToday()
        {
            try
            {
                var json = PlayerPrefs.GetString(SpellBannedTournamentWindow.TOURNAMENT_SPELL_BANNED_DONT_ASK_TODAY);
                var ask = JsonConvert.DeserializeObject<DontAskToday>(json);
                return ask.lastAskTime.Date.Equals(TimeManager.NowUtc.Date);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                PlayerPrefs.DeleteKey(SpellBannedTournamentWindow.TOURNAMENT_SPELL_BANNED_DONT_ASK_TODAY);
                return false;
            }
        }
    }
}