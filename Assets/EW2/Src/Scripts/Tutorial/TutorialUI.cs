using System;
using EW2.Tutorial.Step;
using UnityEngine;
using Zitga.UIFramework;
using FocusType = EW2.Tutorial.Step.FocusType;

namespace EW2.Tutorial.UI
{
    public class TutorialUI : AWindowController
    {
        public DialogUI DialogUI { get; set; }
        public FocusUI FocusUI { get; set; }

        [SerializeField] private GameObject blockRegion;
        

        protected override void Awake()
        {
            base.Awake();
            DialogUI = GetComponentInChildren<DialogUI>();
            FocusUI = GetComponentInChildren<FocusUI>();
            DialogType.ShowingDialog = DialogUI.ShowDialog;
            DialogType.HidingDialog = DialogUI.HideDialog;
            FocusType.ShowingFocus = FocusUI.ShowFocus;
            FocusType.HidingFocus = FocusUI.HideFocus;
            DialogUI.HideDialog();
            FocusUI.HideFocus();
            InactiveBlockRegion();
        }

        public void ActiveBlockRegion()
        {
            blockRegion.SetActive(true);
        }
        
        public void InactiveBlockRegion()
        {
            blockRegion.SetActive(false);
        }

        public void ExecuteSkipTutorial()
        {
            gameObject.SetActive(false);
        }
    }
}