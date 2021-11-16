using Lean.Pool;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class EffectStatChangeController : MonoBehaviour
    {
        private const string ColorGreen = "#8FFF14";

        private const string ColorRed = "#FF5556";

        [SerializeField] private Text txtNameStat;

        [SerializeField] private Text txtValueStat;

        [SerializeField] private Animator animator;

        [SerializeField] private Transform goContent;

        private GameObject _fx;

        public void ShowEffect(RPGStatType type, string value, bool increase = true)
        {
            if (increase)
            {
                ColorUtility.TryParseHtmlString(ColorGreen, out var colorGreen);

                txtValueStat.color = colorGreen;

                txtValueStat.text = "+" + value;
            }
            else
            {
                ColorUtility.TryParseHtmlString(ColorRed, out var colorRed);

                txtValueStat.color = colorRed;

                txtValueStat.text = "-" + value;
            }

            txtNameStat.text = Ultilities.GetNameStat(type);

            ShowFx(increase);

            if (animator)
                animator.StartPlayback();
        }

        private void ShowFx(bool increase)
        {
            var nameFx = "";

            if (increase)
            {
                nameFx = "fx_ui_stat_up";
            }
            else
            {
                nameFx = "fx_ui_stat_down";
            }

            this._fx = ResourceUtils.GetVfx("UI", nameFx, Vector3.zero, quaternion.identity, goContent);

            if (this._fx)
                this._fx.transform.SetAsFirstSibling();
        }

        public void TriggerEndAnim()
        {
            if (this._fx)
                LeanPool.Despawn(this._fx);
            LeanPool.Despawn(gameObject);
        }
    }
}