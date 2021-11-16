using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class MultiRewardWindowProperties : WindowProperties
    {
        public Reward[] rewards;
        public UnityAction CloseCallBack;
        public MultiRewardWindowProperties(Reward[] rewards, UnityAction CloseCallBack = null)
        {
            this.CloseCallBack = CloseCallBack;
            this.rewards = rewards;
        }
    }

    public class MultiRewardWindowController : AWindowController<MultiRewardWindowProperties>
    {
        [SerializeField] private Transform grid;
        [SerializeField] private RectTransform bgTransform;
        [SerializeField] private Text txtCongratulation;

        private const int MaxRewardPerColumn = 6;

        private readonly int[] bgHeightSize = {441, 551};

        private GridReward gridReward;

        protected override void Awake()
        {
            base.Awake();

            gridReward = new GridReward(grid);

            OutTransitionFinished += controller => {
                gridReward.ReturnPool();
                Properties.CloseCallBack?.Invoke();
            };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SetBgHeightSize();

            gridReward.SetData(Properties.rewards);
        }

        private void SetBgHeightSize()
        {
            //int index = MaxRewardPerColumn < Properties.rewards.Length ? 1 : 0;
            bgTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bgHeightSize[1]);//bgHeightSize[index]
        }
    }
}
