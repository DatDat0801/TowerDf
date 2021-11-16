using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class GamePlayUIManager : MonoBehaviour
    {
        public static GamePlayUIManager Instance { get; private set; }

        private bool isLock;

        public bool IsLock
        {
            get => isLock;
            set => isLock = value;
        }

        private UIGameplay currentUI;

        public UIGameplay CurrentUi
        {
            get => currentUI;
            set => currentUI = value;
        }

        private List<UIGameplay> multiUI;

        private void AddMultiUI(UIGameplay ui)
        {
            if (multiUI == null)
                multiUI = new List<UIGameplay>();

            multiUI.Add(ui);
        }

        private bool CloseMultiUI(UIGameplay ui)
        {
            if (multiUI != null && multiUI.Contains(ui))
            {
                ui.Close();
                return multiUI.Remove(ui);
            }

            return false;
        }

        private void CloseAllMultiUI()
        {
            foreach (var ui in multiUI)
            {
                ui.Close();
            }

            multiUI.Clear();
        }

        private void OpenMultiUI()
        {
            foreach (var uiElement in multiUI)
            {
                uiElement.Open();
            }
        }

        private UI_STATE myUIState;

        public UI_STATE MyUIState
        {
            set
            {
                if (myUIState != value)
                {
                    myUIState = value;
                    EventManager.EmitEventData(GamePlayEvent.UIStateChanged, value);
                }
            }
            get { return myUIState; }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (GamePlayController.Instance.State == GamePlayState.End)
            {
                CloseCurrentUI(false);
                CloseAllUI();
            }
        }

        #region Handle UI State

        public void TryOpenUI(UIGameplay ui)
        {
            if (!IsLock)
            {
                CloseCurrentUI(false);
                currentUI = ui;
                MyUIState = ui.GetUIType();
                currentUI.Open();
            }
        }

        public void TryOpenMultiUI(UIGameplay ui)
        {
            if (!IsLock)
            {
                CloseCurrentUI(false);
                AddMultiUI(ui);
                if (MyUIState != ui.GetUIType())
                    MyUIState = ui.GetUIType();
                OpenMultiUI();
            }
        }

        public void CloseCurrentUI(bool isFree)
        {
            if (currentUI != null)
                currentUI.Close();

            if (isFree)
                myUIState = UI_STATE.Free;

            currentUI = null;
        }

        public void CloseUI(UIGameplay ui)
        {
            if (multiUI != null)
                CloseMultiUI(ui);
        }

        public void CloseAllUI()
        {
            if (multiUI != null)
                CloseAllMultiUI();
        }

        #endregion
    }
}