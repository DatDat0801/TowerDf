using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;

namespace EW2
{
    public enum OptionShowInfoTournament
    {
        Buff = 0,
        Nerf = 1,
        Ban = 2
    }

    public class PanelBanBuffNerfInfo : MonoBehaviour
    {
        [SerializeField] private List<UnitViewUI> listUnitUi;
        [SerializeField] private UnitViewSpellUi spellUnitUi;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private Text txtDesc;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnChangeHeroBuffTournament, OnChangeHeroBuffTournament);
        }

        private void OnChangeHeroBuffTournament()
        {
            if (this.dropdown.value == (int)OptionShowInfoTournament.Buff)
            {
                ShowBuffInfo();
            }
        }

        private void OnEnable()
        {
            AddDropdownItem();
            this.dropdown.value = (int)OptionShowInfoTournament.Buff;
            this.dropdown.RefreshShownValue();
            ShowBuffInfo();
        }

        void AddDropdownItem()
        {
            this.dropdown.options.Clear();

            this.dropdown.options.Add(new Dropdown.OptionData(L.playable_mode.tournament_buff_txt));
            this.dropdown.options.Add(
                new Dropdown.OptionData($"<color=#ff3c3c>{L.playable_mode.tournament_nerf_txt}</color>"));
            this.dropdown.options.Add(
                new Dropdown.OptionData($"<color=#ff3c3c>{L.playable_mode.tournament_ban_txt}</color>"));
        }

        public void ListenDropChange()
        {
            this.spellUnitUi.gameObject.SetActive(false);
            
            if (this.dropdown.value == (int)OptionShowInfoTournament.Buff)
            {
                ShowBuffInfo();
            }
            else if (this.dropdown.value == (int)OptionShowInfoTournament.Nerf)
            {
                ShowNerfInfo();
            }
            else if (this.dropdown.value == (int)OptionShowInfoTournament.Ban)
            {
                ShowBanInfo();
            }
        }

        private void ShowBuffInfo()
        {
            var listHeroBuff = UserData.Instance.TournamentData.listHeroBuff;
            for (int i = 0; i < listUnitUi.Count; i++)
            {
                if (i < listHeroBuff.Count)
                {
                    listUnitUi[i].ShowUi(OptionShowInfoTournament.Buff, listHeroBuff[i].heroId);
                    this.listUnitUi[i].gameObject.SetActive(true);
                }
                else
                {
                    this.listUnitUi[i].gameObject.SetActive(false);
                }
            }

            this.txtDesc.text = GetDescBuff(UserData.Instance.TournamentData.buffStatId);
        }

        private void ShowNerfInfo()
        {
            for (int i = 0; i < listUnitUi.Count; i++)
            {
                if (i == 0 && UserData.Instance.TournamentData.heroNerfId > 0)
                {
                    listUnitUi[i].ShowUi(OptionShowInfoTournament.Nerf, UserData.Instance.TournamentData.heroNerfId);
                    this.listUnitUi[i].gameObject.SetActive(true);
                }
                else
                {
                    this.listUnitUi[i].gameObject.SetActive(false);
                }
            }

            this.txtDesc.text = GetDescNerf(UserData.Instance.TournamentData.nerfStatId);
        }

        private void ShowBanInfo()
        {
            for (int i = 0; i < listUnitUi.Count; i++)
            {
                this.listUnitUi[i].gameObject.SetActive(false);
            }

            this.txtDesc.text = "";
            
            this.spellUnitUi.ShowUi(OptionShowInfoTournament.Ban, UserData.Instance.TournamentData.spellBanId);
            this.spellUnitUi.gameObject.SetActive(true);
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
    }
}