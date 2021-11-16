using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class InfoCheatDailyQuestController : MonoBehaviour
    {
        [SerializeField] private GameObject bgr;
        [SerializeField] private GameObject itemRewardPrefab;
        [SerializeField] private Transform rewardPanel;

        public void SetReward(Reward[] rewards, Vector3 point)
        {
            foreach (Transform child in rewardPanel)
            {
                LeanPool.Despawn(child.gameObject);
            }

            foreach (var reward in rewards)
            {
                var go = Instantiate(itemRewardPrefab, rewardPanel);

                var rewardUi = ResourceUtils.GetRewardUi(reward.type);

                rewardUi.SetData(reward);

                rewardUi.SetParent(go.transform);
            }

            GetComponent<RectTransform>().anchoredPosition3D = point;
        }

        private void Update()
        {
            HideIfClickedOutside(bgr);
        }

        private void HideIfClickedOutside(GameObject panel)
        {
            if (Input.GetMouseButton(0) && gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    panel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    Camera.main))
            {
                gameObject.SetActive(false);
            }
        }
    }
}