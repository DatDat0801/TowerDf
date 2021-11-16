using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroAcademyCheckIn : ScriptableObject
    {
        public HeroAcademyCheckInData[] heroAcademyCheckInDatas;

        [Serializable]
        public class HeroAcademyCheckInData : QuestBase
        {
        }
    }

#if UNITY_EDITOR

    public class HeroAcademyCheckInPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/GameOpsEvent/HeroAcademy/hero_academy_checkin.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(HeroAcademyCheckIn) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/GameOpsEvent/" + nameAsset;
                    HeroAcademyCheckIn gm = AssetDatabase.LoadAssetAtPath<HeroAcademyCheckIn>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroAcademyCheckIn>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.heroAcademyCheckInDatas =
                        CsvReader.Deserialize<HeroAcademyCheckIn.HeroAcademyCheckInData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}