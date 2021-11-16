using EW2.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.Tutorial.Step
{
    public class LayerFocusPoint: MonoBehaviour
    {

        public void IncreaseLayer()
        {
            var  canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = SortingLayerConstants.TUTORIAL;
            canvas.sortingOrder = 2;
            gameObject.AddComponent<GraphicRaycaster>();
        }
        
        public void DecreaseLayer()
        {
            var canvas=  gameObject.GetComponent<Canvas>();
            var graphicRaycaster=  gameObject.GetComponent<GraphicRaycaster>();
            if (canvas && graphicRaycaster)
            {
                Destroy(graphicRaycaster);
                Destroy(canvas);
            }
           
        }
        
    }
}