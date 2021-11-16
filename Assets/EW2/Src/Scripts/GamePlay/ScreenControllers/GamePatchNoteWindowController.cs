using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class GamePatchNoteWindowController : AWindowController
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtVersion;
        [SerializeField] private Text txtContent;

        protected override void Awake()
        {
            base.Awake();
            
            btnClose.onClick.AddListener(() =>
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                UIFrame.Instance.CloseCurrentWindow();
            });
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtTitle.text = L.popup.patch_note_txt;
            
            txtVersion.text = $"{L.popup.version_txt} {Application.version}";

            txtContent.text = L.patch_note.new_feed;
        }
        
        
    }

}
