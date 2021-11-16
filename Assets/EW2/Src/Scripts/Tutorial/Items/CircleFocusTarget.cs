using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.Tutorial.Items
{
    public class CircleFocusTarget : MonoBehaviour
    {
        public Image[] circles;
        //public Image hand;

        private void OnEnable()
        {
            Play();
        }

        private void Play()
        {
            Sequence s = DOTween.Sequence();

            //s.Append(hand.rectTransform.DOLocalMove(new Vector3(49f, 35f), 0.4f).From(new Vector3(202f, 147.5f)));
            //s.AppendCallback(() => hand.gameObject.SetActive(false));
            s.AppendInterval(0.39f);
            s.Insert(0.15f,circles[0].rectTransform.DOScale(1f, 0.4f).From(0f)).SetLoops(-1);
            s.Join(circles[0].DOFade(0.15f, 0.4f).From(1f));
            s.Join(circles[1].rectTransform.DOScale(1f, 0.8f).From(0f)).SetLoops(-1);
            s.Join(circles[2].rectTransform.DOScale(1f, 1.2f).From(0f)).SetLoops(-1);
            s.SetLink(this.gameObject);
            s.SetUpdate(true);
            //s.SetRecyclable(true);
            //s.SetAutoKill(false);
            s.Play();
        }
    }
}