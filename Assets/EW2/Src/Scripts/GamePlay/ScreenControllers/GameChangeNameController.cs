using System;
using System.Text.RegularExpressions;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class GameChangeNameController : AWindowController
    {
        [SerializeField] private Text txtTitle;

        [SerializeField] private Text txtWarning;

        [SerializeField] private InputField inputNewName;

        [SerializeField] private Button btnConfirm;

        [SerializeField] private Button btnConfirmDisable;

        [SerializeField] private Button btnClose;

        Regex regexItem = new Regex("^[a-zA-Z0-9]*$");

        protected override void Awake()
        {
            base.Awake();

            btnConfirm.onClick.AddListener(ConfirmClick);

            btnClose.onClick.AddListener(CloseClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            InitUI();
        }

        private void InitUI()
        {
            txtTitle.text = L.popup.change_name;

            txtWarning.text = L.popup.letters_limit;

            if (UserData.Instance.AccountData.isChangedName)
            {
                inputNewName.placeholder.GetComponent<Text>().text = UserData.Instance.AccountData.userName;
            }
            else
            {
                inputNewName.placeholder.GetComponent<Text>().text = L.popup.enter_name_box;
            }

            inputNewName.text = String.Empty;

            btnConfirmDisable.gameObject.SetActive(true);
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        private void ConfirmClick()
        {
            var newName = inputNewName.text;

            if (CheckName(newName))
            {
                var acc = UserData.Instance.AccountData;

                acc.userName = newName;

                if (!acc.isChangedName)
                    acc.isChangedName = true;

                UserData.Instance.Save();

                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.change_successful);

                EventManager.EmitEvent(GamePlayEvent.OnChangeNameSuccess);

                CloseClick();
                
            }
            FirebaseLogic.Instance.ButtonClick("profile", "change_name", 4);
        }

        private bool CheckName(string name)
        {
            if (name.Length < 4 || name.Length > 18)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.name_too_short_or_long);

                return false;
            }
            else if (!regexItem.IsMatch(name))
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.name_contain_special_characters);

                return false;
            }
            else if (UserData.Instance.AccountData.userName.Equals(name))
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.already_exists_name);

                return false;
            }

            return true;
        }

        public void OnChangedValue()
        {
            if (inputNewName.text.Length > 0)
                btnConfirmDisable.gameObject.SetActive(false);
            else
                btnConfirmDisable.gameObject.SetActive(true);
        }
    }
}