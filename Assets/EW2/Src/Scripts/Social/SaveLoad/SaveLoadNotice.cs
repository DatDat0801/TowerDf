using SocialTD2;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    /// <summary>
    /// Enable and disable notice icon on avatar home screen
    /// </summary>
    public class SaveLoadNotice : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private Image icon;

        private void OnEnable()
        {
            Notice();
        }

        public  void Notice()
        {
            if (LoadSaveUtilities.IsSavedToday())
            {
                icon.enabled = false;
                Status = false;
            }
            else
            {
                icon.enabled = true;
                Status = true;
            }
        }

        public bool Status { get; set; }
    }
}