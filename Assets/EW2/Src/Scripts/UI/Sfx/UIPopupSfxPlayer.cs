using System;
using Hellmade.Sound;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    [RequireComponent(typeof(IUIScreenController))]
    public class UIPopupSfxPlayer : MonoBehaviour
    {
        private IUIScreenController window;

        private void Awake()
        {
            window = GetComponent<IUIScreenController>();
        }

        private void OnDisable()
        {
            window.InTransitionFinished -= OnOpen;
            window.CloseRequest -= OnClose;
        }

        private void OnEnable()
        {
            window.InTransitionFinished += OnOpen;

            window.CloseRequest += OnClose;
        }

        private void OnClose(IUIScreenController obj)
        {
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip1, EazySoundManager.GlobalSoundsVolume);
        }

        private void OnOpen(IUIScreenController obj)
        {
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.POPUP_OPEN);
            EazySoundManager.PlaySound(audioClip1, EazySoundManager.GlobalSoundsVolume);
        }
    }
}