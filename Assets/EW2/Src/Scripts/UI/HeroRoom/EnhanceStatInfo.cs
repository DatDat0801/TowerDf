using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class EnhanceStatInfo : MonoBehaviour
    {
        [SerializeField] private Text txtTitleStat;
        [SerializeField] private Text txtStatCurr;
        [SerializeField] private Text txtStatNext;

        public void SetData(string title, string statCurr, string statNext)
        {
            txtTitleStat.text = title;
            txtStatCurr.text = statCurr;
            txtStatNext.text = statNext;
        }
    }
}