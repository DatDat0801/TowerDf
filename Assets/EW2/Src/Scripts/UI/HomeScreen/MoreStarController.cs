using UnityEngine;

namespace EW2
{
    public class MoreStarController : MonoBehaviour
    {
        [SerializeField] private GameObject bgr;

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