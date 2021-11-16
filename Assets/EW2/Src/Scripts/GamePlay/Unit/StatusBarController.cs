using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class StatusBarController : MonoBehaviour
    {
        [SerializeField] private GameObject statusPrefab;
        [SerializeField] private Transform listStatus;
        [SerializeField] private Transform iconStatus;

        private Dictionary<StatusType, GameObject> _listIconStatus = new Dictionary<StatusType, GameObject>();

        private GameObject _virtualHpBar;

        public void AddIconStatus(StatusType statusType)
        {
            var go = Instantiate(this.statusPrefab, this.iconStatus);

            if (go == null) return;

            if (statusType == StatusType.ShieldPoint)
            {
                go.GetComponent<Image>().sprite = ResourceUtils.GetSpriteAtlas("icon_status", "icon_amor_buff");
            }
            else if (statusType == StatusType.Terrify)
            {
                go.GetComponent<Image>().sprite = ResourceUtils.GetSpriteAtlas("icon_status", "icon_terrify");
            }

            go.SetActive(true);

            this._listIconStatus.Add(statusType, go);
        }

        public void RemoveIconStatus(StatusType statusType)
        {
            if (this._listIconStatus.ContainsKey(statusType))
            {
                Destroy(this._listIconStatus[statusType]);
                _listIconStatus.Remove(statusType);
            }
        }

        public GameObject AddShieldBar()
        {
            var shielddBar = ResourceUtils.GetUnitOther("shield_bar_frame", listStatus);
            AddIconStatus(StatusType.ShieldPoint);
            return shielddBar;
        }

        public GameObject AddVirtualHpBar()
        {
            if (this._virtualHpBar == null)
            {
                this._virtualHpBar = ResourceUtils.GetUnitOther("virtual_hp_bar", listStatus);
                return this._virtualHpBar;
            }

            this._virtualHpBar.SetActive(true);
            return this._virtualHpBar;
        }
    }
}