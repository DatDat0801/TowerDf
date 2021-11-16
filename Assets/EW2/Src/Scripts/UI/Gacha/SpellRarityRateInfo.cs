using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class SpellRarityRateInfo : MonoBehaviour
    {
        [SerializeField] private Text nameRarity;
        [SerializeField] private Text rate;

        public void SetInfo(int rarityId, float ratio)
        {
            Color colorText;
            ColorUtility.TryParseHtmlString(GetColorRarity(rarityId), out colorText);
            nameRarity.color = colorText;
            nameRarity.text = Ultilities.GetRarity(rarityId);
            rate.text = $"{(ratio * 100).ToString("n2")}%";
        }

        private string GetColorRarity(int rarity)
        {
            if (rarity == 0)
            {
                return "#62a521";
            }
            else if (rarity == 1)
            {
                return "#0094ac";
            }
            else if (rarity == 2)
            {
                return "#a65af4";
            }
            else if (rarity == 3)
            {
                return "#b97900";
            }
            else if (rarity == 4)
            {
                return "#ea3e3e";
            }
            else
            {
                return "#62a521";
            }
        }
    }
}