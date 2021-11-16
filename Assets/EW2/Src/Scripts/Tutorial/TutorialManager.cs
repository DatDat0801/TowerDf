using EW2.Tutorial.Step;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EW2.Tutorial.General
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        private int currentTutorialId = 0;

        public int CurrentTutorialId => this.currentTutorialId;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            TutorialFactory.InitialFactory();
        }

        private void Start()
        {
            RegisterEvents();
        }

        public void CompleteCurrentTutorialFollowId(int tutorialId)
        {
            if (IsCurrentTutorial(tutorialId))
            {
                TutorialFactory.GetTutorial(currentTutorialId).Complete();
            }
        }

        public void CompleteTutorial(int tutorialId)
        {
            TutorialFactory.GetTutorial(tutorialId).Complete();
        }

        public void CompleteCurrentTutorial()
        {
            TutorialFactory.GetTutorial(currentTutorialId).Complete();
        }


        public void ExecuteCurrentStepTutorialFollowId(int tutorialId)
        {
            if (IsCurrentTutorial(tutorialId))
            {
                TutorialFactory.GetTutorial(currentTutorialId).Execute();
            }
        }

        public void ExecuteCurrentStepTutorial()
        {
            TutorialFactory.GetTutorial(currentTutorialId)?.Execute();
        }

        public void ExecuteStepTutorial(int tutorialId)
        {
            TutorialFactory.GetTutorial(tutorialId)?.Execute();
            currentTutorialId = tutorialId;
        }

        public bool IsLockUpgradeTower
        {
            get
            {
                var lessThanId12 = currentTutorialId < AnyTutorialConstants.FOCUS_MAGE_BUILED_TOWER;
                var complete = CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0);
                if (lessThanId12 && !complete) return true;
                else return false;
            }
        }

        public bool IsTutorialCompleted()
        {
            return CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0);
        }

        public bool CheckCompleteGroupTutorial(int groupId)
        {
            var accountData = UserData.Instance.AccountData;
            return accountData.CheckCompleteGroupTutorial(groupId);
        }

        public void ExecuteSkipTutorial()
        {
            var accountData = UserData.Instance.AccountData;
            //Debug.LogAssertion("User AccountData hashcode: "+accountData.GetHashCode());
            accountData.AddFullGroupTutorial();
            
            UserData.Instance.Save();
            //FindingObjects.CalculateTutorialUI().ExecuteSkipTutorial();
        }

        public void ExecuteAutoGroupCompleteTutorial()
        {
            var accountData = UserData.Instance.AccountData;
            accountData.ExecuteAutoGroupCompleteTutorial();
        }

        private void ComputeNextStep()
        {
            var stepTutorialData = GameContainer.Instance.GetTutorialData().GetStepTutorialData(currentTutorialId);
            var customizeNextTutorialId = stepTutorialData.customizeNextTutorialId;
            if (customizeNextTutorialId != AnyTutorialConstants.DEFAULT_CUSTOMIZE_TUTORIAL_ID)
            {
                currentTutorialId = stepTutorialData.customizeNextTutorialId;
            }
            else
            {
                currentTutorialId++;
            }
        }

        private bool IsCurrentTutorial(int tutorialId) => currentTutorialId == tutorialId;


        private void RegisterEvents()
        {
            TutorialBase.ExecutingCurrentStepTutorial = ExecuteCurrentStepTutorial;
            TutorialBase.CompletingCurrentTutorial = CompleteCurrentTutorial;
            TutorialBase.ComputingNextStep = ComputeNextStep;
        }
    }
}