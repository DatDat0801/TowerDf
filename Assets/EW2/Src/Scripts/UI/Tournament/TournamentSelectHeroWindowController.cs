using System;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class TournamentSelectHeroWindowController : AWindowController
    {
        [SerializeField] private HeroBarTournamentController heroBarTournamentController;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmButton;

        private void Start()
        {
            EventManager.StartListening(GamePlayEvent.OnUpdateSaleBundle, OnHeroPurchased);
        }

        private void OnHeroPurchased()
        {
            Repaint();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            AutoFillHeroesOnUI();

            Repaint();
        }
        
        protected override void AddListeners()
        {
            base.AddListeners();
            this.closeButton.onClick.AddListener(CloseClick);
            this.confirmButton.onClick.AddListener(ConfirmClick);
        }

        private void ConfirmClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.tournament_select_heroes_popup);
            var lobbyWindow =  UIFrame.Instance.FindWindow(ScreenIds.tournament_lobby) as TournamentLobbyWindow;
            if (lobbyWindow != null)
            {
                var heroSelectorLobby=  lobbyWindow.GetComponentInChildren<TournamentHeroSelector>();
                heroSelectorLobby.RepaintHeroesUI();
            }

        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseWindow(ScreenIds.tournament_select_heroes_popup);
        }

        private void AutoFillHeroesOnUI()
        {
            var userHeroData = UserData.Instance.TournamentData;

            heroBarTournamentController.heroSelectController.SetInfo(userHeroData.listHeroSelected);
        }

        void Repaint()
        {
            this.heroBarTournamentController.SetInfo();
        }
    }
}