using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public enum TypeBtnSetting
    {
        Sound = 0,
        Music = 1,
        Shake = 2,
        Notification = 3,
        BatterySaver = 4,
        Rate = 5,
        None,
    }

    public class ButtonSetting : MonoBehaviour
    {
        const string NameBgrOn = "bg_button_blue";

        const string NameBgrOff = "bg_button_blue_unselect";

        [SerializeField] private Image icon;

        [SerializeField] private TypeBtnSetting typeBtnSetting;

        public TypeBtnSetting TypeBtnSetting => typeBtnSetting;

        private Button button;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            InitUi();
        }

        private void OnClick()
        {
            if (typeBtnSetting == TypeBtnSetting.Music)
            {
                var currValue = UserData.Instance.SettingData.music;

                UserData.Instance.SettingData.music = !currValue;

                if (UserData.Instance.SettingData.music)
                {
                    EazySoundManager.GlobalMusicVolume = 1;
                    //EazySoundManager.ResumeAllMusic();
                }
                else
                {
                    EazySoundManager.GlobalMusicVolume = 0.001f;
                    //EazySoundManager.PauseAllMusic();
                }

                UserData.Instance.Save();

                UpdateStatusSetting(UserData.Instance.SettingData.music);
            }
            else if (typeBtnSetting == TypeBtnSetting.Sound)
            {
                var currValue = UserData.Instance.SettingData.sound;

                UserData.Instance.SettingData.sound = !currValue;

                if (UserData.Instance.SettingData.sound)
                {
                    EazySoundManager.GlobalSoundsVolume = 1;

                    //EazySoundManager.ResumeAllSounds();
                }
                else
                {
                    EazySoundManager.GlobalSoundsVolume = 0;

                    //EazySoundManager.PauseAllSounds();
                }

                UserData.Instance.Save();

                UpdateStatusSetting(UserData.Instance.SettingData.sound);
            }
            else if (typeBtnSetting == TypeBtnSetting.Shake)
            {
                var currValue = UserData.Instance.SettingData.shake;

                UserData.Instance.SettingData.shake = !currValue;

                UserData.Instance.Save();

                UpdateStatusSetting(UserData.Instance.SettingData.shake);
            }
            else if (typeBtnSetting == TypeBtnSetting.BatterySaver)
            {
                ShowComingSoon();
            }
            else if (typeBtnSetting == TypeBtnSetting.Notification)
            {
                ShowComingSoon();
            }
            else if (typeBtnSetting == TypeBtnSetting.Rate)
            {
                var link = string.Format(GameConfig.LinkGoogleStore, Application.identifier);
                Application.OpenURL(link);
            }
        }

        private void UpdateStatusSetting(bool active)
        {
            if (active)
            {
                button.image.sprite = ResourceUtils.GetSpriteAtlas("tab_image", NameBgrOn);

                icon.sprite = ResourceUtils.GetSpriteAtlas("setting_icon", $"icon_{(int) typeBtnSetting}_on");
            }
            else
            {
                button.image.sprite = ResourceUtils.GetSpriteAtlas("tab_image", NameBgrOff);

                icon.sprite = ResourceUtils.GetSpriteAtlas("setting_icon", $"icon_{(int) typeBtnSetting}_off");
            }
        }

        private void InitUi()
        {
            var settingData = UserData.Instance.SettingData;

            if (typeBtnSetting == TypeBtnSetting.Music)
            {
                UpdateStatusSetting(settingData.music);
            }
            else if (typeBtnSetting == TypeBtnSetting.Sound)
            {
                UpdateStatusSetting(settingData.sound);
            }
            else if (typeBtnSetting == TypeBtnSetting.Shake)
            {
                UpdateStatusSetting(settingData.shake);
            }
            else if (typeBtnSetting == TypeBtnSetting.BatterySaver)
            {
                UpdateStatusSetting(settingData.batterySave);
            }
            else if (typeBtnSetting == TypeBtnSetting.Notification)
            {
                UpdateStatusSetting(settingData.notification);
            }
        }

        private void ShowComingSoon()
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
        }
    }
}