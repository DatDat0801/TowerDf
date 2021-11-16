using System;
using UnityEngine;

namespace EW2
{
    public class TowerUpgradeInfoData : ScriptableObject
    {
        public UpgradeTowerInfo[] UIInfo;
    }

    [Serializable]
    public class UpgradeTowerInfo
    {
        public int towerId;
        public int level;
        public int currencyType;
        public string localizationKey;
    }
}