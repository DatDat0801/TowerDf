using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class StaminaData : ScriptableObject
    {
        public Stamina[] dataConfig;

        [Serializable]
        public class Stamina
        {
            public long timeRecovery;

            public int maxStamina;

            public int numberStaminaBuy;

            public int cost;
        }

        public Stamina GetDataConfig()
        {
            return dataConfig[0];
        }
    }

#if UNITY_EDITOR
    public class StaminaDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/stamina_config.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(StaminaData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Stamina/" + nameAsset;
                    StaminaData gm = AssetDatabase.LoadAssetAtPath<StaminaData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<StaminaData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.dataConfig = CsvReader.Deserialize<StaminaData.Stamina>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}