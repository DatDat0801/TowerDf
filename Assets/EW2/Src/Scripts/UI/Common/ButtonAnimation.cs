using DG.Tweening;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EW2
{
    [RequireComponent(typeof(Button))]
    public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool muteSound;

        private readonly Vector3 scaleUp = new Vector3(1.15f, 1.15f, 1.15f);
        private readonly Vector3 scaleDown = new Vector3(0.96f, 0.96f, 0.96f);

        private Vector3 localScale;

        private void Awake()
        {
            localScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var scaleTarget = new Vector3(this.localScale.x * this.scaleDown.x, this.scaleDown.y, this.scaleDown.z);
            transform.DOScale(scaleTarget, 0.01f).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var scaleTarget = new Vector3(this.localScale.x * this.scaleUp.x, this.scaleUp.y, this.scaleUp.z);
            transform.DOScale(scaleTarget, 0.03f).OnComplete(() => { transform.localScale = localScale; })
                .SetUpdate(true);
            if (!muteSound)
            {
                var audioClip1 = ResourceUtils.LoadSound(SoundConstant.BUTTON_CLICK);
                EazySoundManager.PlaySound(audioClip1);
            }
        }

        public void SetMuteSound(bool value)
        {
            muteSound = value;
        }
    }
}