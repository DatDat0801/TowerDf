using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class GraphicPanel : MonoBehaviour, IUpdateTabBarChanged
    {
        [SerializeField] private TabsManager tabsManager;

        private int currQuality;

        private void OnEnable()
        {
            currQuality = UserData.Instance.SettingData.graphicQuality;

            tabsManager.InitTabManager(this, currQuality);
        }

        public void OnTabBarChanged(int indexActive)
        {
            currQuality = indexActive;

            UserData.Instance.SettingData.graphicQuality = currQuality;
            
            UserData.Instance.Save();
            
            DeviceUtils.SetUpQualityLevel(UserData.Instance.SettingData.graphicQuality);
        }
    }
}