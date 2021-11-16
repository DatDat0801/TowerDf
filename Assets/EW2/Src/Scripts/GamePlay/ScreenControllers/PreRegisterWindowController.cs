using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class PreRegisterWindowController : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtContent;
        [SerializeField] private Text txtButton;
        [SerializeField] private Transform grid;
        [SerializeField] private Button btnConfirm;

        private GridReward gridReward;

        protected override void Awake()
        {
            base.Awake();
            
            gridReward = new GridReward(grid);
            
            btnConfirm.onClick.AddListener(OnConfirm);

            OutTransitionFinished += controller =>
            {
                gridReward.ReturnPool();
                
                var rewards = GameContainer.Instance.GetPreRegister();
                
                PopupUtils.ShowReward(rewards);
            };
        }
        
        public override void SetLocalization()
        {
            txtTitle.text = L.popup.pre_register_reward_txt;
            txtContent.text = L.popup.pre_register_thank_txt;
            txtButton.text = L.button.btn_confirm;
        }
        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            
            var rewards = GameContainer.Instance.GetPreRegister();
            
            gridReward.SetData(rewards);
        }

        private void OnConfirm()
        {
            UserData.Instance.BackUpData.isTriggeredPreregister = true;
            
            var rewards = GameContainer.Instance.GetPreRegister();
            Reward.AddToUserData(rewards, AnalyticsConstants.SourcePreRegister);
            
            UIFrame.Instance.CloseWindow(ScreenIds.pre_register);
        }
    }
}