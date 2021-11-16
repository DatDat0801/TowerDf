using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EW2.Tutorial.Step;
using UnityEngine;
using FocusType= EW2.Tutorial.Step.FocusType;

namespace EW2.Tutorial
{
    public class TutorialFactory
    {
        private static Dictionary<int, Type> anyInstanceTutorials;
        private static Dictionary<int, TutorialBase> anyTutorials;
        
        private static bool IsInitialized => anyInstanceTutorials != null;

        public static void InitialFactory()
        {
            if (IsInitialized)
            {
                return;
            }
            
            var tutorialBase = Assembly.GetAssembly(typeof(TutorialBase)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && 
                    (    myType.IsSubclassOf(typeof(DialogType)) ||  myType.IsSubclassOf(typeof(FocusType))
                                                                 ||  myType.IsSubclassOf(typeof(BasicTutorial))));
            anyInstanceTutorials = new Dictionary<int, Type>();
            anyTutorials = new Dictionary<int, TutorialBase>();
            foreach (var type in tutorialBase)
            {
                var tutorialBaseInstance = Activator.CreateInstance(type) as TutorialBase;
                if (tutorialBaseInstance != null)
                {
                    anyInstanceTutorials.Add(tutorialBaseInstance.tutorialId, type);
                    anyTutorials.Add(tutorialBaseInstance.tutorialId,tutorialBaseInstance);
                }
            }
        }

        public static TutorialBase GetTutorial(int tutorialId)
        {
            if (anyTutorials.ContainsKey(tutorialId))
            {
                return anyTutorials[tutorialId];
            }

            return null;
        }

        public static TutorialBase CreateInstanceTutorial(int tutorialId)
        {
            if (anyInstanceTutorials.ContainsKey(tutorialId))
            {
                Type type = anyInstanceTutorials[tutorialId];
                var tutorialBaseInstance = Activator.CreateInstance(type) as TutorialBase;
                return tutorialBaseInstance;
            }

            return null;
        }
    }
}