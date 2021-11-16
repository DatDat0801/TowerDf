using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroClassesControl : MonoBehaviour
    {
        [SerializeField] private Image iconClasses;

        [SerializeField] private Text txtClasses;

        public void InitClasses(HeroClasses classes)
        {
            iconClasses.sprite = ResourceUtils.GetSpriteAtlas("hero_classes", $"icon_{classes.ToString().ToLower()}");

            txtClasses.text = Ultilities.GetHeroClasses(classes);

            txtClasses.color = Ultilities.GetTextColorHeroClasses(classes);
        }
    }
}