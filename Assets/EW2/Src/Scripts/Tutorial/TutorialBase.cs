using System;
using UnityEngine;

namespace EW2.Tutorial.Step
{
    public abstract class TutorialBase
    {

        public static Action ExecutingCurrentStepTutorial { get; set; }
        public static Action CompletingCurrentTutorial{ get; set; }
        public static Action ComputingNextStep{ get; set; }
        public int tutorialId { get; protected set; }

        public virtual void Execute()
        {
            
            Debug.Log("TUTORIAL EXECUTE " +tutorialId );
        }

        public virtual void Complete()
        {
            SaveData();
            Debug.Log("TUTORIAL COMPLETE " +tutorialId );
        }

        protected void ExecuteNextStepTutorial()
        {
            ComputingNextStep?.Invoke();
            ExecutingCurrentStepTutorial?.Invoke();
        }

        private void SaveData()
        {
            var accountData = UserData.Instance.AccountData;
            accountData.AddCompletedGroupId(tutorialId);
            //Debug.LogAssertion("SAVE STEP TUTORIAL " +tutorialId );
            UserData.Instance.Save();
        }
    }
}