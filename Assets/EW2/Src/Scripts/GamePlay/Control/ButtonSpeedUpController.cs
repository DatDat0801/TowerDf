using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ButtonSpeedUpController : MonoBehaviour
    {
        [SerializeField] private Button btnSpeedUp;

        [SerializeField] private GameObject border;
        private const float SPEED_UP = 16f;
        private GameObject effect;

        private void Awake()
        {
            btnSpeedUp.onClick.AddListener(SpeedUpButton);
        }

        private void OnEnable()
        {
            border.SetActive(false);
        }

        private void OnDisable()
        {
            if (effect)
            {
                effect.SetActive(false);
            }
        }

        private void SpeedUpButton()
        {
            if (GamePlayController.Instance.Speed <= 0 || GamePlayController.Instance.Speed >= SPEED_UP)
            {
                GamePlayController.Instance.Speed = 1;

                border.SetActive(false);

                if (effect)
                {
                    effect.SetActive(false);
                }
            }
            else
            {
                GamePlayController.Instance.Speed = SPEED_UP;

                border.SetActive(true);

                if (effect == null)
                {
                    effect = ResourceUtils.GetVfx("UI", "fx_ui_skip_btn", border.transform.localPosition,
                        Quaternion.identity, transform);
                    effect.transform.SetParent(transform);

                    effect.transform.localPosition = border.transform.localPosition;
                }
                
                effect.SetActive(true);
               
            }
        }
    }
}