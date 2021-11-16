using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroAcademyCheckinPrompt : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private Image icon;
        public bool Status { get; set; }

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnUpdateBadge, Notice);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.OnUpdateBadge, Notice);
        }

        private void OnEnable()
        {
            Notice();
        }

        public void Notice()
        {
            var on = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroAcademyCheckinEvent>().CheckCanReceive();
            if (on)
            {
                icon.enabled = true;
                Status = true;
            }
            else
            {
                icon.enabled = false;
                Status = false;
            }
        }
    }
}