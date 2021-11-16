using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class SalebundleButtonUi : MonoBehaviour
    {
        [SerializeField] protected Text title;

        [SerializeField] protected TimeRemainUi timeRemain;

        private Button btn;

        protected virtual void Awake()
        {
            if (btn == null)
            {
                btn = GetComponent<Button>();

                btn.onClick.AddListener(ButtonOnClick);
            }
        }

        public virtual void ButtonOnClick()
        {
        }
    }
}