using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class InfoSkillSpellController : MonoBehaviour
    {
        [SerializeField] private Image imgSkill;
        [SerializeField] private Image imgBorder;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtSubTitle;
        [SerializeField] private Text txtDesc;

        public void SetData(int idSpell,int rarity, string title, string content, bool isActive)
        {
            txtTitle.text = title;
            txtDesc.text = content;
            
            if (isActive)
                txtSubTitle.text = L.gameplay.active_skill;
            else
                txtSubTitle.text = L.gameplay.passive_skill;
            
            if (isActive)
                imgSkill.sprite = ResourceUtils.GetSpriteAtlas("spell", $"{idSpell}_0");
            else
                imgSkill.sprite = ResourceUtils.GetSpriteAtlas("spell", $"{idSpell}_1");
            
            imgBorder.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity}_circle");
        }
    }
}