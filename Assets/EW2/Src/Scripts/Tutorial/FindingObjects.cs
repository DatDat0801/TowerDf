using EW2.Tutorial.Map;
using EW2.Tutorial.UI;
using UnityEngine;
 
namespace EW2.Tutorial.General
{
    public  class FindingObjects: MonoBehaviour
    {
        public static GamePlayWindowController CalculateGamePlayWindowController() => FindObjectOfType<GamePlayWindowController>();
        public static GamePlayController CalculateGamePlayController() => FindObjectOfType<GamePlayController>();
        public static Map0Tutorial CalculateMap0TutorialInspector() => FindObjectOfType<Map0Tutorial>();
        
        public static TowerOption CalculateTowerOption() => FindObjectOfType<TowerOption>();
        
        public static CameraController CalculateCameraController()=>  FindObjectOfType<CameraController>();

        public static  TutorialUI CalculateTutorialUI() => FindObjectOfType<TutorialUI>();
        
        public static  HeroSkillContentController CalculateHeroSkillContentController() => FindObjectOfType<HeroSkillContentController>();
        
        
        
      
      
    }
}