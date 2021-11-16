using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Text textPercent;

    public void SetData(float percent)
    {
        if (rect)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal , percent * width);
        }

        if (textPercent)
        {
            textPercent.text = $"{Mathf.FloorToInt(percent * 100)}%";
        }
    }
}