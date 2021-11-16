using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class PreviewRewardDefendModeWindow : AWindowController<MultiRewardWindowProperties>
    {
        [SerializeField] private Transform grid;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Button btnClose;

        private GridReward _gridReward;

        protected override void Awake()
        {
            base.Awake();

            this._gridReward = new GridReward(grid);

            OutTransitionFinished += controller => {
                this._gridReward.ReturnPool();
                Properties.CloseCallBack?.Invoke();
            };

            this.btnClose.onClick.AddListener(CloseClick);
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_preview_reward_defense_mode);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            this._gridReward.SetData(Properties.rewards);
        }
    }
}