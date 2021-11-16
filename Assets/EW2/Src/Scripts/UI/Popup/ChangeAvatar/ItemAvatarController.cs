using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemAvatarController : MonoBehaviour
    {
        [SerializeField] private Image iconAvatar;

        [SerializeField] private GameObject focus;

        private Button button;

        private Action<int> onClickCallback;

        private int avatarId;

        public int AvatarId => avatarId;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(AvatarOnClick);
        }

        public void InitAvatar(int id, bool isFocus, Action<int> onClick)
        {
            avatarId = id;

            onClickCallback = onClick;

            SetUi(isFocus);
        }

        private void SetUi(bool isFocus)
        {
            iconAvatar.sprite = ResourceUtils.GetSpriteAvatar(avatarId);

            focus.SetActive(isFocus);
        }

        public void UpdateStatus(bool isFocus)
        {
            focus.SetActive(isFocus);
        }

        private void AvatarOnClick()
        {
            onClickCallback?.Invoke(avatarId);
        }
    }
}