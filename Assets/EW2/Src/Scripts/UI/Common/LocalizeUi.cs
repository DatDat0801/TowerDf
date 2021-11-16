using Zitga.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class LocalizeUi : MonoBehaviour
    {
        [SerializeField] private string category;

        [SerializeField] private string keyLocalize;

        [SerializeField] private bool isUpper;

        private Text label;

        private void OnEnable()
        {
            label = GetComponent<Text>();

            if (label)
            {
                string content = Localization.Current.Get(category, keyLocalize);
                if (isUpper)
                    label.text = content.ToUpper();
                else
                    label.text = content;
            }
        }
    }
}