using System.Collections;
using System.Collections.Generic;
using EW2.Constants;
using EW2.Tutorial.Map;
using UnityEngine;
using EW2.Tutorial.General;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2.Tutorial.Step
{
    public class TutorialStep8 : FocusType
    {
        
        private TowerOptionButton towerOptionBtn;
        
        public TutorialStep8()
        {
            tutorialId = AnyTutorialConstants.FOCUS_BARRACK_TOWER;
        }

        public override void Execute()
        {
            towerOptionBtn = FindingObjects.CalculateTowerOption().GetBarrackTowerOptionBtn();
            towerOptionBtn.BuildingSuccess += CompleteBuilding;
            TutorialManager.Instance.StartCoroutine(CoExecute());
        }

        public  void CompleteBuilding()
        {
            TutorialManager.Instance.StartCoroutine(CoComplete());
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus build barrack tower");
        }

        protected override Vector3 CalculateFocusPosition() 
            => towerOptionBtn.transform.position;

        private IEnumerator CoComplete()
        {
            var secondDelayComplete = 0.5f;
            towerOptionBtn.BuildingSuccess -= CompleteBuilding;
            HidingFocus?.Invoke();
            yield return new WaitForSeconds(secondDelayComplete);
            base.Complete();
           
            DecreaseLayerFocusPoint(towerOptionBtn.gameObject);
            ExecuteNextStepTutorial();
          
           

        }

        private IEnumerator CoExecute()
        {
            var secondDelayExecute = 0.3f;
            yield return new WaitForSeconds(secondDelayExecute);
            base.Execute();
            IncreaseLayerFocusPoint(towerOptionBtn.gameObject);
            
        }


       
    }
}