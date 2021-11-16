using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

//using UnityEngine.UIElements;


namespace EW2
{
    public class TournamentTestTool : MonoBehaviour
    {
        [SerializeField] private InputField buffIdTextField;
        [SerializeField] private Button changeBuffId;

        [SerializeField] private InputField nerfIdTextField;
        [SerializeField] private Button changeNerfId;
        
        [SerializeField] private InputField heroIdBuffField;
        [SerializeField] private Button changeBuffHeroes;
        [SerializeField] private InputField heroIdNerfField;
        [SerializeField] private Button changeNerfHero;

        [SerializeField] private Button changeMap;
        [SerializeField] private InputField mapIdField;
        

        [SerializeField] private Button toggleButton;
        [SerializeField] private GameObject testPanel;
        private void Start()
        {
            this.changeBuffId.onClick.AddListener(ChangeBuffIdClick);
            this.changeNerfId.onClick.AddListener(ChangeNerfIdClick);
            this.toggleButton.onClick.AddListener(ToggleCloseButton);
            this.changeBuffHeroes.onClick.AddListener(ChangeBuffHero);
            this.changeNerfHero.onClick.AddListener(ChangeNerfHero);
            this.changeMap.onClick.AddListener(ChangMapClick);
        }

        private void ChangMapClick()
        {
            var value = this.mapIdField.text;
            if (string.IsNullOrEmpty(value)) return;
            try
            {
                UserData.Instance.TournamentData.currentMapId = int.Parse(value);
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Success");
            }
            finally
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Failed");
            }
        }

        void ChangeBuffHero()
        {
            var value = this.heroIdBuffField.text;
            if (string.IsNullOrEmpty(value)) return;
            try
            {
                var ids = value.Split(',');
                var listId = new List<int>();
                foreach (var id in ids)
                {
                    listId.Add(int.Parse(id));
                }
                
                UserData.Instance.TournamentData.SetListHeroBuff(listId);
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Success");
            }
            finally
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Failed");
            }
        }

        void ChangeNerfHero()
        {
            var value = this.heroIdNerfField.text;
            if (string.IsNullOrEmpty(value)) return;
            try
            {
                UserData.Instance.TournamentData.heroNerfId = int.Parse(value);
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Success");
            }
            finally
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Failed");
            }
            
        }
        void ToggleCloseButton()
        {
            this.testPanel.SetActive(!this.testPanel.activeSelf);
        }
        private void ChangeNerfIdClick()
        {
            var value = this.nerfIdTextField.text;
            if (string.IsNullOrEmpty(value)) return;
            try
            {
                UserData.Instance.TournamentData.nerfStatId = int.Parse(value);
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Success");
            }
            finally
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Failed");
            }
        }

        private void ChangeBuffIdClick()
        {
            var value = this.buffIdTextField.text;
            if (string.IsNullOrEmpty(value)) return;
            try
            {
                UserData.Instance.TournamentData.buffStatId = int.Parse(value);
                UserData.Instance.Save();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Success");
            }
            finally
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, "Failed");
            }
        }
    }

}
