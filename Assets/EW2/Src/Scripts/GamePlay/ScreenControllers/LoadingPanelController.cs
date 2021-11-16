using Cysharp.Threading.Tasks;
using TigerForge;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    public class LoadingPanelController : APanelController
    {
        private const int MaxValue = 100;

        //[SerializeField] private ProgressBar progressBar;

        private int valueTrigger;

        private bool isTrigger;

        private bool isProcessing;

        protected override void Awake()
        {
            InTransitionFinished += Loading;
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.CloseLoading, OnCloseLoading);
        }

        protected override void OnPropertiesSet()
        {
            isTrigger = false;

            isProcessing = false;

            //progressBar.SetData(0);

            Debug.Log("close loading");


        }

        private void OnCloseLoading()
        {
            isTrigger = true;

            EventManager.StopListening(GamePlayEvent.CloseLoading, OnCloseLoading);

            if (isProcessing == false)
            {
                ILoading(valueTrigger, MaxValue);
                //StartCoroutine(ILoading(valueTrigger, MaxValue));
            }
        }

        async void ILoading(int from, int to)
        {
            isProcessing = true;

            for (int i = from; i < to; i++)
            {
                //progressBar.SetData(i / 100.0f);
                await UniTask.Yield();
                // return null;
            }

            if (to == MaxValue)
            {
                UIFrame.Instance.HidePanel(ScreenIds.loading);
            }
            else if (isTrigger)
            {
                ILoading(to, MaxValue);
                //StartCoroutine(ILoading(to, MaxValue));
            }

            isProcessing = false;
        }


        private void Loading(IUIScreenController controller)
        {
            valueTrigger = MaxValue;//Random.Range(50, 70);
            ILoading(0, valueTrigger);
            //StartCoroutine(ILoading(0, valueTrigger));
        }
    }
}