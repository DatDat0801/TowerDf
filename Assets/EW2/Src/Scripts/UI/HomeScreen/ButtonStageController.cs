using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;


namespace EW2
{
    public class ButtonStageController : MonoBehaviour
    {
        private const string HtmlColorStrokeNormal = "#14315D";

        private const string HtmlColorStrokeCurrent = "#450000";

        [SerializeField] private Image iconStage;

        [SerializeField] private Text lbLevel;

        [SerializeField] private GameObject starFlag;

        [SerializeField] private Image[] stars;

        private Button button;

        private int currLevel;

        private int mapId;

        private int numberStar = 0;

        private void Start()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(OnClick);
        }

        public void InitStage(int id)
        {
            currLevel = id + 1;

            mapId = id;

            numberStar = UserData.Instance.CampaignData.GetStar(0, mapId);

            ShowUi();
        }

        private void ShowUi()
        {
            lbLevel.text = currLevel.ToString();

            SetIcon();

            SetUiStar();
        }

        private void SetUiStar()
        {
            if (numberStar == 0)
            {
                starFlag.SetActive(false);
            }
            else
            {
                starFlag.SetActive(true);

                if (numberStar > GameConfig.MaxNumberStarNormal && numberStar <= GameConfig.MaxNumberStarHard)
                {
                    var starHard = numberStar - GameConfig.MaxNumberStarNormal;

                    for (int i = 0; i < stars.Length; i++)
                    {
                        if (i < starHard)
                        {
                            stars[i].sprite = ResourceUtils.GetIconMoney(MoneyType.GoldStar);

                            stars[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            stars[i].gameObject.SetActive(false);
                        }
                    }
                }
                else if (numberStar <= GameConfig.MaxNumberStarNormal)
                {
                    for (int i = 0; i < stars.Length; i++)
                    {
                        if (i < numberStar)
                        {
                            stars[i].sprite = ResourceUtils.GetIconMoney(MoneyType.SliverStar);

                            stars[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            stars[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        private void SetIcon()
        {
            var listStroke = lbLevel.GetComponents<Outline>();


            if (numberStar == 0)
            {
                foreach (var stroke in listStroke)
                {
                    stroke.effectColor = Ultilities.GetColorFromHtmlString(HtmlColorStrokeCurrent);
                }

                iconStage.sprite = ResourceUtils.GetSpriteAtlas("stage", "icon_stage_current");
            }
            else
            {
                foreach (var stroke in listStroke)
                {
                    stroke.effectColor = Ultilities.GetColorFromHtmlString(HtmlColorStrokeNormal);
                }

                if (numberStar > GameConfig.MaxNumberStarNormal && numberStar <= GameConfig.MaxNumberStarHard)
                {
                    if (currLevel % 5 != 0)
                    {
                        iconStage.sprite = ResourceUtils.GetSpriteAtlas("stage", "icon_stage_hard");
                    }
                    else
                    {
                        iconStage.sprite = ResourceUtils.GetSpriteAtlas("stage", "icon_stage_hard_boss");
                    }

                    //centering text level
                    lbLevel.rectTransform.anchoredPosition = new Vector2(2f, 3f);
                }
                else if (numberStar <= GameConfig.MaxNumberStarNormal)
                {
                    if (currLevel % 5 != 0)
                    {
                        iconStage.sprite = ResourceUtils.GetSpriteAtlas("stage", "icon_stage_normal");
                    }
                    else
                    {
                        iconStage.sprite = ResourceUtils.GetSpriteAtlas("stage", "icon_stage_normal_boss");
                    }

                    //centering text level
                    lbLevel.rectTransform.anchoredPosition = new Vector2(2f, 11.7f);
                }
            }

            iconStage.SetNativeSize();
        }

        private void OnClick()
        {
            if (mapId < 0) return;

            UIFrame.Instance.OpenWindow(ScreenIds.campaign_info,
                new CampaignInfoWindowProperties(MapCampaignInfo.GetCampaignId(0, mapId, 0)));
        }
    }
}