using System;
using System.Collections.Generic;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class AvatarSelectionController : AWindowController, IUpdateTabBarChanged
    {
        [SerializeField] private Image iconAvatar;

        [SerializeField] private Text txtTitle;

        [SerializeField] private Text txtApply;

        [SerializeField] private Button btnApply;

        [SerializeField] private Button btnClose;

        [SerializeField] private Text txtUsed;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private List<TabContainer> tabContainers;

        private int currTabIndex;

        private int currAvatar;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            currTabIndex = -1;

            txtTitle.text = L.popup.avatar_select;

            txtApply.text = L.button.btn_confirm;

            txtUsed.text = L.popup.in_use_txt;

            tabsManager.InitTabManager(this, 0);
        }

        protected override void Awake()
        {
            base.Awake();

            btnApply.onClick.AddListener(ApplyClick);

            btnClose.onClick.AddListener(() =>
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                UIFrame.Instance.CloseCurrentWindow();
            });

            UpdateUiAvatar(UserData.Instance.AccountData.avatarId);

            EventManager.StartListening(nameof(AvatarContainerController), OnAvatarIndexChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventManager.StopListening(nameof(AvatarContainerController), OnAvatarIndexChange);
        }

        private void ApplyClick()
        {
            if (currAvatar != UserData.Instance.AccountData.avatarId)
            {
                UserData.Instance.AccountData.avatarId = currAvatar;

                UserData.Instance.Save();

                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.change_successful);

                EventManager.EmitEvent(GamePlayEvent.OnChangeAvatarSuccess);
                
                UpdateUiAvatar(UserData.Instance.AccountData.avatarId);
            }
        }

        private void OnAvatarIndexChange()
        {
            var index = EventManager.GetData<int>(nameof(AvatarContainerController));

            currAvatar = index;

            UpdateUiAvatar(currAvatar);
        }

        public void OnTabBarChanged(int indexActive)
        {
            if (currTabIndex == indexActive) return;

            currTabIndex = indexActive;

            UpdateUiAvatar(UserData.Instance.AccountData.avatarId);

            if (currTabIndex < tabContainers.Count)
            {
                for (int i = 0; i < tabContainers.Count; i++)
                {
                    if (i == currTabIndex)
                        tabContainers[i].ShowContainer();
                    else
                        tabContainers[i].HideContainer();
                }
            }
        }

        private void UpdateUiAvatar(int index)
        {
            iconAvatar.sprite = ResourceUtils.GetSpriteAvatar(index);

            txtUsed.gameObject.SetActive(AvaterUsed(index));

            btnApply.gameObject.SetActive(!AvaterUsed(index));
        }
        
        private bool AvaterUsed(int id)
        {
            return UserData.Instance.AccountData.avatarId == id;
        }
    }
}