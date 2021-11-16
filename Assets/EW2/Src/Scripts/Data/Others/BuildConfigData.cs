using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Zitga.CsvTools;
#endif

namespace EW2
{
    [System.Serializable]
    public class BuildConfigData : ScriptableObject
    {
        public string packageName;
    
       
        
   
    }

#if UNITY_EDITOR
    public class BuildConfigDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/BuildConfig";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 && str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {

                    // get asset file
                    string nameAsset = nameof(BuildConfigData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/BuildConfig/" + nameAsset;
                    BuildConfigData gm = AssetDatabase.LoadAssetAtPath<BuildConfigData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<BuildConfigData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get hero base file
                    string nameBaseCsv = $"{csvFormat}/build_config.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                   var  deserializeData = CsvReader.Deserialize<BuildConfigData>(data.text)[0];
                   gm.packageName = deserializeData.packageName;
                    
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}


