using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class PreRegisterData : ScriptableObject
    {
        public Reward[] rewards;
        
    }

#if UNITY_EDITOR
    public class PreRegisterDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/PreRegister/reward.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(PreRegisterData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/PreRegister/" + nameAsset;
                    PreRegisterData gm = AssetDatabase.LoadAssetAtPath<PreRegisterData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<PreRegisterData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.rewards = CsvReader.Deserialize<Reward>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}