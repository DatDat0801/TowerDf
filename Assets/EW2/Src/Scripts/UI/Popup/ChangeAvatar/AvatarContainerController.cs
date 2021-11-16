using System.Collections.Generic;
using Lean.Pool;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class AvatarContainerController : TabContainer
    {
        [SerializeField] private GameObject itemPrefab;

        [SerializeField] private Transform container;

        private List<ItemAvatarController> listAvatar = new List<ItemAvatarController>();

        private bool isInit;

        public override void ShowContainer()
        {
            gameObject.SetActive(true);

            if (!isInit)
            {
                isInit = true;

                for (int i = 0; i < GameConfig.MaxAvatar; i++)
                {
                    var go = LeanPool.Spawn(itemPrefab, container, false);

                    if (go)
                    {
                        var control = go.GetComponent<ItemAvatarController>();

                        control.InitAvatar(i, UserData.Instance.AccountData.avatarId == i, AvatarClick);

                        listAvatar.Add(control);
                    }
                }
            }
            else
            {
                UpdateUi();
            }
        }

        private void AvatarClick(int avatarId)
        {
            EventManager.EmitEventData(nameof(AvatarContainerController), avatarId);

            foreach (var avatarItem in listAvatar)
            {
                avatarItem.UpdateStatus(avatarItem.AvatarId == avatarId);
            }
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void UpdateUi()
        {
            foreach (var avatarItem in listAvatar)
            {
                avatarItem.UpdateStatus(avatarItem.AvatarId == UserData.Instance.AccountData.avatarId);
            }
        }
    }
}