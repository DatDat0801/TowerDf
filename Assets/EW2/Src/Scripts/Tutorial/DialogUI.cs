using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EW2.Tutorial.General;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;

namespace EW2.Tutorial.UI
{
    public class DialogUI : MonoBehaviour
    {
        [SerializeField] private Text heroTitleTxt;
        [SerializeField] private Image heroIconImage;
        [SerializeField] private Text dialogTxt;
        [SerializeField] private Button nextBtn;
        [SerializeField] private GameObject dialogElement;


        private void Start()
        {
            nextBtn.onClick.AddListener(OnNextBtnClick);
        }

        public void ShowDialog(int heroId, string dialog, bool isForceDesc = false)
        {
            dialogElement.SetActive(true);
            heroIconImage.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{heroId}");
            heroTitleTxt.text = Ultilities.GetNameHero(heroId);
            //StartCoroutine(CoComputeDisplayDialogTxt(dialog));
            CoComputeDisplayDialogTxt(dialog, isForceDesc);
        }

        public void HideDialog()
        {
            dialogElement.SetActive(false);
        }

        private void CoComputeDisplayDialogTxt(string dialog, bool isForceDesc = false)
        {
            var calculatedDialog = dialog;
            if (!isForceDesc)
                calculatedDialog = Localization.Current.Get(LocalizeCategory.TUTORIAL, dialog);
            dialogTxt.DOText(calculatedDialog, 1f).SetEase(Ease.Linear).From(string.Empty).SetUpdate(true);
        }

        private void OnNextBtnClick()
        {
            TutorialManager.Instance.CompleteCurrentTutorial();
        }
    }
}