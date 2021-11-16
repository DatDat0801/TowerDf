using UnityEngine;
using UnityEngine.EventSystems;

namespace EW2
{
    public class ToggleElements : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject[] onItems;

        [SerializeField] private GameObject[] offItems;

        public bool IsOn { get; private set; }

        private void OnEnable()
        {
            Toggle();
        }

        public void Toggle()
        {
            if (IsOn)
            {
                for (var i = 0; i < onItems.Length; i++)
                {
                    onItems[i].SetActive(false);
                }
                for (var i = 0; i < offItems.Length; i++)
                {
                    offItems[i].SetActive(true);
                }

                IsOn = false;
            }
            else
            {
                for (var i = 0; i < onItems.Length; i++)
                {
                    onItems[i].SetActive(true);
                }
                for (var i = 0; i < offItems.Length; i++)
                {
                    offItems[i].SetActive(false);
                }

                IsOn = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }
    }
}