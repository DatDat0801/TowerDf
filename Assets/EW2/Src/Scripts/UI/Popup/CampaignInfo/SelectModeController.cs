using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2.CampaignInfo.ModeSelect
{
    public class SelectModeController : MonoBehaviour
    {
        [SerializeField] private CampaignInfoWindowController campaignInfoWindowController;

        [SerializeField] private Button buttonNormal;

        [SerializeField] private Button buttonHard;

        [SerializeField] private GameObject hardLock;

        [SerializeField] private GameObject hardUnlock;

        [SerializeField] private Text txtNormal;

        [SerializeField] private Text txtHard;

        [SerializeField] private Image[] stars;

        [SerializeField] private GameObject flagSelected;

        [SerializeField] private Image miniMap;

        private int worldId = -1;

        private int stageId = -1;

        private int modeId = -1;

        private void Start()
        {
            buttonNormal.onClick.AddListener(OnClickNormal);
            buttonHard.onClick.AddListener(OnClickHard);
        }

        private void OnEnable()
        {
            SetLocalization();
        }

        public void InitializeMode()
        {
            //Init
            var isUnlockStage =
                UserData.Instance.CampaignData.IsUnlockedHardStage(worldId, stageId);

            if (isUnlockStage)
            {
                OnSelectMode(1);
            }
            else
            {
                OnSelectMode(0);
            }
        }

        private void OnClickNormal()
        {
            OnSelectMode(0);
        }

        private void OnClickHard()
        {
            var isUnlockStage =
                UserData.Instance.CampaignData.IsUnlockedHardStage(worldId, stageId);

            if (isUnlockStage)
            {
                OnSelectMode(1);
            }
            else
            {
                print("hard stage is not unlock");
                var content = string.Format(L.popup.locked_mode_notice, stageId + 1);
                var dataInfo = new PopupInfoWindowProperties(L.popup.notice_txt, content);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_text, dataInfo);
            }
        }

        public void SetInfo(int worldId, int stageId, int modeId)
        {
            if (this.worldId == worldId && this.stageId == stageId && this.modeId == modeId)
                return;

            this.worldId = worldId;

            this.stageId = stageId;

            this.modeId = modeId;

            SetHardButton();

            OnSelectMode(this.modeId);

            SetStar();



            SetMiniMap();
        }

        private void SetMiniMap()
        {
            miniMap.sprite = ResourceUtils.GetSprite("MiniMaps", $"map_{this.worldId}_{this.stageId}");
        }

        private void SetLocalization()
        {
            txtNormal.text = L.stages.difficult_normal;

            txtHard.text = L.stages.difficult_nightmare;
        }

        private void SetHardButton()
        {
            var isUnlockStage =
                UserData.Instance.CampaignData.IsUnlockedHardStage(worldId, stageId);

            hardLock.SetActive(!isUnlockStage);
            hardUnlock.SetActive(isUnlockStage);
        }

        private void SetStar()
        {
            int star = UserData.Instance.CampaignData.GetStar(worldId, stageId, modeId);

            var fillImage = ResourceUtils.GetSpriteAtlas("stars", modeId == 0 ? "1" : "2");
            var emptyImage = ResourceUtils.GetSpriteAtlas("stars", "3");
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = i < star ? fillImage : emptyImage;
            }
        }

        public void OnSelectMode(int modeId)
        {
            this.modeId = modeId;

            if (modeId == 0)
            {
                flagSelected.transform.localPosition = buttonNormal.transform.localPosition;
            }
            else
            {
                flagSelected.transform.localPosition = buttonHard.transform.localPosition;
            }

            campaignInfoWindowController.SetMode(modeId);

            SetStar();
        }
    }
}
