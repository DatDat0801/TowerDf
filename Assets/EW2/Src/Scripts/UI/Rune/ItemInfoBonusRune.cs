using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;

namespace EW2
{
    public class ItemInfoBonusRune : MonoBehaviour
    {
        [SerializeField] private GameObject dotEnable;
        [SerializeField] private GameObject dotDisable;
        [SerializeField] private Text txtDesc;
        [SerializeField] private Text txtSuggest;

        private Color colorDesc;
        private RuneItem runeUser;
        private RuneSetBonusStat runeSetBonusStat;
        private int runeId;
        private int setNumber;

        public void SetInfo(int setType, RuneItem runeUserData, int currSet)
        {
            runeUser = runeUserData;
            this.setNumber = setType;
            var dataCompare = InventoryDataBase.GetRuneId(runeUserData.RuneIdConvert);

            runeId = dataCompare.Item1;
            runeSetBonusStat = GameContainer.Instance.Get<InventoryDataBase>().Get<RuneSetBonusDatabase>()
                .GetDataRuneSet(dataCompare.Item1, setType);

            dotEnable.SetActive(currSet == setType);
            dotDisable.SetActive(currSet != setType);

            if (setType < currSet)
            {
                ColorUtility.TryParseHtmlString(GameConfig.TextColorBrown, out colorDesc);
                txtSuggest.gameObject.SetActive(false);
            }
            else if (setType == currSet)
            {
                ColorUtility.TryParseHtmlString(GameConfig.TextColorOrange, out colorDesc);
                txtSuggest.gameObject.SetActive(false);
            }
            else if (setType > currSet)
            {
                ColorUtility.TryParseHtmlString(GameConfig.TextColorGray, out colorDesc);
                var nameRune = Localization.Current.Get("rune", $"rune_name_{dataCompare.Item1.ToString()}");
                txtSuggest.text = string.Format(L.popup.set_required_txt, setType.ToString(), nameRune.ToLower());
                txtSuggest.gameObject.SetActive(true);
            }

            txtDesc.text = GetBonusDesc();
            txtDesc.color = colorDesc;
        }

        public string GetBonusDesc()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{string.Format(L.popup.set_of_pieces_txt, setNumber)} ");

            if (runeSetBonusStat != null)
            {
                if (runeId == (int) RuneId.ImmortalRune)
                {
                    var nameStatHpRegen = Ultilities.GetNameStat(RPGStatType.HpRegeneration);
                    var nameStatRespawn = Ultilities.GetNameStat(RPGStatType.TimeRevive);
                    var desc = L.rune.rune_set_basic_des_6;
                    var descPercent = L.rune.rune_set_basic_des_percent;
                    stringBuilder
                        .Append(string.Format(descPercent, nameStatHpRegen, $"{runeSetBonusStat.statValue[0] * 100}"))
                        .Append("\n");
                    stringBuilder.Append(string.Format(desc, nameStatRespawn, $"{runeSetBonusStat.statValue[1]}"));
                }
                else if (runeId == (int) RuneId.WisdomRune)
                {
                    var desc = L.rune.rune_set_basic_des_9;
                    stringBuilder.Append(string.Format(desc, $"{runeSetBonusStat.statValue[0] * 100}"));
                }
                else if (runeId == (int) RuneId.ArgonyRune || runeId == (int) RuneId.MiseryRune)
                {
                    var nameStat = Ultilities.GetNameStat(runeSetBonusStat.statType[0]);
                    var desc = L.rune.bonus_damage_rune_set_des;
                    stringBuilder.Append(string.Format(desc, $"{runeSetBonusStat.statValue[0] * 100}", nameStat));
                }
                else
                {
                    var nameStat = Ultilities.GetNameStat(runeSetBonusStat.statType[0]);
                    var desc = L.rune.rune_set_basic_des_percent;
                    stringBuilder.Append(string.Format(desc, nameStat, $"{runeSetBonusStat.statValue[0] * 100}"));
                }
            }

            return stringBuilder.ToString();
        }
    }
}