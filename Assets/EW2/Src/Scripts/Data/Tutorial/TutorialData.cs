using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif


namespace EW2
{
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private StepTutorialData[] anyStepTutorialData;

        public StepTutorialData[] AnyStepTutorialData
        {
            get => anyStepTutorialData;
            set => anyStepTutorialData = value;
        }

        public StepTutorialData GetStepTutorialData(int tutorialId)
        {
            foreach (var stepTutorialData in AnyStepTutorialData)
            {
                if (stepTutorialData.tutorialId ==tutorialId)
                {
                    return stepTutorialData;
                }
            }
            return null;
        }

        public int CalculateCountTutorialInGroup(int groupId)
        {
            var calculatedCountTutorial = 0;
            foreach (var stepTutorialData in AnyStepTutorialData)
            {
                if (stepTutorialData.groupId==groupId)
                {
                    calculatedCountTutorial++;
                }
            }

            return calculatedCountTutorial;
        }

        public List<int> CalculateAnyGroups()
        {
            var anyGroups = new List<int>();
            foreach (var stepTutorialData in AnyStepTutorialData)
            {
                if (!anyGroups.Contains(stepTutorialData.groupId))
                {
                    anyGroups.Add(stepTutorialData.groupId);
                }
            }
            return anyGroups;
        }

       
    
        [System.Serializable]
        public class StepTutorialData
        {
            public int tutorialId;
            public int groupId;
            public string dialog;
            public int speakerId;
            public string trackingId;
            public string tooltip;
            public bool isAutoCompleteGroup;
            public int customizeNextTutorialId;

        }
    }
    
    #if UNITY_EDITOR
    public class TutorialDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tutorials";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                  

                    // get asset file
                    string nameAsset = "tutorial_data" + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tutorials/" + nameAsset;
                    TutorialData gm = AssetDatabase.LoadAssetAtPath<TutorialData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TutorialData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    string nameBaseCsv = $"{csvFormat}/config.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.AnyStepTutorialData = CsvReader.Deserialize<TutorialData.StepTutorialData>(data.text);
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}


