using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.HeroSelect
{
    public class HeroSelectedData
    {
        public int heroId;

        public int level;

        public int slot;

        public void SetData(HeroSelectedData data)
        {
            this.heroId = data.heroId;

            this.level = data.level;

            this.slot = data.slot;
        }
    }

    public class HeroSelectedView : MonoBehaviour
    {
        [SerializeField] private GameObject avatar;

        [SerializeField] private Image iconAvatar;

        [SerializeField] private Image notifyIcon;

        public HeroSelectedData Hero { get; private set; }

        public void SetInfo(HeroSelectedData data)
        {
            this.Hero = data;

            avatar.SetActive(this.Hero.heroId > 0);

            if (this.Hero.heroId >= 0)
            {
                var avatarId = (Hero.heroId % 1000) - 1;
                iconAvatar.sprite = ResourceUtils.GetSpriteAvatar(avatarId);
            }
        }

        public void Notify(bool enable)
        {
            notifyIcon.gameObject.SetActive(enable);
        }
    }
}