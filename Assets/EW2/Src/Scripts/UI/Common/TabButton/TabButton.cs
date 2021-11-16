using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EW2
{
    public abstract class TabButton : MonoBehaviour
    {
        [SerializeField] protected int index;

        [SerializeField] protected Button button;

        [SerializeField] protected Text lbButton;
        
        [SerializeField] protected bool hasImgFocus;

        [ShowIf("hasImgFocus")] [SerializeField]
        protected Image focus;

        public Action<int> OnClickButton { get; set; }

        protected virtual void Awake()
        {
            button.onClick.AddListener(TabOnClick);
        }
        
        public virtual void TabOnClick()
        {
            OnClickButton?.Invoke(index);
        }

        public int GetIndex()
        {
            return index;
        }

        public virtual void SetTabActiveChangeColor(bool isActive, Color colorActive, Color colorInactive,
            Color colorActiveText, Color colorInactiveText)
        {
            if (button.image)
            {
                button.image.color = isActive ? colorActive : colorInactive;
            }

            if (lbButton != null)
            {
                lbButton.color = isActive ? colorActiveText : colorInactiveText;
            }

            HandleUiCommon(isActive);
        }

        public virtual void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText, Color colorInactiveText)
        {
            HandleUiCommon(isActive);
        }
        
        public virtual void SetTabActiveChangeImg(bool isActive)
        {
            HandleUiCommon(isActive);
        }

        private void HandleUiCommon(bool isActive)
        {
            gameObject.SetActive(true);
            
            if (focus)
                focus.gameObject.SetActive(isActive);

            // Canvas.ForceUpdateCanvases();
        }
    }
}