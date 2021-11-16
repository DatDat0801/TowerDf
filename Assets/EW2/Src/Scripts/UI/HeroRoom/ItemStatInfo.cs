using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemStatInfo : MonoBehaviour
    {
        [SerializeField] private RPGStatType type;

        public RPGStatType Type => type;

        [SerializeField] private Image iconStat;

        [SerializeField] private Text statValue;

        [SerializeField] private Text statDesc;

        [SerializeField] private Text statLvlUp;

        public void ShowInfoStat(float valueStat, float valueStatLvlUp, bool isPercent = false)
        {
            if (iconStat)
                iconStat.sprite = ResourceUtils.GetSpriteAtlas("stat_icons", $"icon_stat_{(int) Type}");

            if (statValue)
                statValue.text = isPercent ? $"{valueStat * 100f}%" : "" + valueStat;

            if (statLvlUp)
                statLvlUp.text = isPercent ? $"+{valueStatLvlUp * 100f}%" : $"+{valueStatLvlUp}";

            if (statDesc)
                statDesc.text = Ultilities.GetNameStat(type);
        }
    }
}