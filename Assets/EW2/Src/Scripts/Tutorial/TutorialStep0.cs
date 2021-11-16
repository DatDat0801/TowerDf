using System.Collections;
using System.Collections.Generic;
using EW2.Tutorial.General;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2.Tutorial.Step
{
    public class TutorialStep0 : BasicTutorial
    {
        public TutorialStep0()
        {
            tutorialId = AnyTutorialConstants.LOAD_MAP_0;
        }

        public override void Execute()
        {
            base.Execute();
            GamePlayController.CampaignId = 0;
            TutorialManager.Instance.StartCoroutine(CoLoadSceneGamePlay());
        }

        private IEnumerator CoLoadSceneGamePlay()
        {
            var asyncLoad = SceneManager.LoadSceneAsync(SceneName.GamePlay);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            CompletingCurrentTutorial?.Invoke();
        }

        public override void Complete()
        {
            base.Complete();
            ExecuteNextStepTutorial();
            FirebaseLogic.Instance.PassTutorial(tutorialId, "load map 0");
        }
    }
}