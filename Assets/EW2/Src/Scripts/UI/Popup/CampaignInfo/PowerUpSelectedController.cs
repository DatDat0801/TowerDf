using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.PowerUpSelect
{
    public class PowerUpSelectedController : MonoBehaviour
    {
        private const int MaxSlot = 6;

        [SerializeField] private CampaignInfoWindowController campaignInfoWindowController;
        [SerializeField] private Transform root;
        [SerializeField] private Button buttonPowerUp;

        private List<PowerUpSelectedData> datas;
        
        private PowerUpSelectedView[] uiList;

        public void Start()
        {
            buttonPowerUp.onClick.AddListener(OnClickPowerUp);
            
            SetInfo();
        }

        private void OnClickPowerUp()
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
            
            campaignInfoWindowController.OnClickPowerUpSelected();
        }

        public void SetInfo()
        {
            datas = new List<PowerUpSelectedData>();
            for (int i = 0; i < MaxSlot; i++)
            {
                datas.Add(new PowerUpSelectedData(){id = -1, quantity = 0});
            }
            
            uiList = root.GetComponentsInChildren<PowerUpSelectedView>();

            if (uiList.Length != datas.Count)
            {
                throw new Exception($"data is not valid: {uiList.Length} != {datas.Count}");
            }

            for (int i = 0; i < MaxSlot; i++)
            {
                uiList[i].SetInfo(datas[i]);
            }
        }
    }
}