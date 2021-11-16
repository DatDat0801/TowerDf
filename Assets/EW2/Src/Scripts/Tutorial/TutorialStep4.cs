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
    public class TutorialStep4 : FocusType
    {
        private TowerOptionButton towerOptionBtn;
        
        public TutorialStep4()
        {
            tutorialId = AnyTutorialConstants.FOCUS_ARCHER_TOWER;
        }

        public override void Execute()
        {
            towerOptionBtn = FindingObjects.CalculateTowerOption().GetArcherTowerOptionBtn();
            towerOptionBtn.BuildingSuccess += CompleteBuilding;
            TutorialManager.Instance.StartCoroutine(CoExecute());
        }

        public  void CompleteBuilding()
        {
            TutorialManager.Instance.StartCoroutine(CoComplete());
        }

        protected override Vector3 CalculateFocusPosition() 
            => towerOptionBtn.transform.position;

        private IEnumerator CoComplete()
        {
            var secondDelayComplete = 0.5f;
            HidingFocus?.Invoke();
            towerOptionBtn.BuildingSuccess -= CompleteBuilding;
            yield return new WaitForSeconds(secondDelayComplete);
            base.Complete();
           
            DecreaseLayerFocusPoint(towerOptionBtn.gameObject);
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "Focus archer tower");
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