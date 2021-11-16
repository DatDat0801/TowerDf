using UnityEngine;

namespace EW2
{
    public class SelectHeroUiController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer imgSelect;

        [SerializeField] private SpriteRenderer imgGlow;

        private Color colorHero1002 = new Color(1f, 0.58f, 0.15f);

        private Color colorHero1001 = new Color(1f, 0.96f, 0.15f);

        private Color colorHero1003 = new Color(0.7176471f, 0.01568628f, 0f, 1f);

        private Color colorHero1004 = new Color(0, 1f, 0f, 1f);

        public void InitSelectCircle(int heroId)
        {
            switch (heroId)
            {
                case 1001:
                    imgSelect.color = colorHero1001;
                    imgGlow.color = colorHero1001;
                    break;

                case 1002:
                    imgSelect.color = colorHero1002;
                    imgGlow.color = colorHero1002;
                    break;
                case 1003:
                    imgSelect.color = colorHero1003;
                    imgGlow.color = colorHero1003;
                    break;
                case 1004:
                    imgSelect.color = colorHero1004;
                    imgGlow.color = colorHero1004;
                    break;
            }
        }

        public void ShowSelectCircle()
        {
            gameObject.SetActive(true);
        }

        public void HideSelectCircle()
        {
            gameObject.SetActive(false);
        }
    }
}