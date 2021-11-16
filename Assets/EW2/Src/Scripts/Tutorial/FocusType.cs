using System;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class FocusType: TutorialBase
    {
        public static Action<Vector3, string> ShowingFocus { get; set; }
        public static Action HidingFocus { get; set; }
        
        public override void Execute()
        {
            base.Execute();
            var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(tutorialId);
            ShowingFocus?.Invoke(CalculateFocusPosition(),stepTutorialData.tooltip);
            UIFrame.Instance.HideAllPanels(true);
            UIFrame.Instance.CloseAllPopup(true);

        }
        
        protected void IncreaseLayerFocusPoint(GameObject focusPoint)
        {
            var  layerFocusPoint = focusPoint.AddComponent<LayerFocusPoint>();
            layerFocusPoint.IncreaseLayer();
        }
        
        protected void DecreaseLayerFocusPoint(GameObject focusPoint)
        {
           
            var  layerFocusPoint = focusPoint.GetComponent<LayerFocusPoint>();
            if (layerFocusPoint)
            {
                layerFocusPoint.DecreaseLayer();
            }
        }

        protected virtual Vector3 CalculateFocusPosition() => Vector3.zero;
    }
}