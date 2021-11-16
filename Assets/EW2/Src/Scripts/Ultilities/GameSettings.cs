using Hellmade.Sound;

namespace EW2
{
    public sealed class GameSettings
    {
        public static void InitSoundSettings()
        {
            //init user settings sound
            var setting = UserData.Instance.SettingData;
            EazySoundManager.GlobalMusicVolume = setting.music ? 1 : 0.001f;
            EazySoundManager.GlobalSoundsVolume = setting.sound ? 1 : 0.001f;
        }
    }
}