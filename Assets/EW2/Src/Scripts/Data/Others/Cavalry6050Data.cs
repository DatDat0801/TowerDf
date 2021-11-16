using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Cavalry6050Data : ScriptableObject
    {
        public DataCavalry[] data;

        [System.Serializable]
        public class DataCavalry
        {
            public float damage;

            public DamageType damageType;

            public float timeCooldown;

            public float timeStun;
            
            public float moveSpeed;

        }

        public DataCavalry GetDataCavalry()
        {
            return data[0];
        }
    }

#if UNITY_EDITOR
    public class DataCavalryPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/cavalry_6050.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Cavalry6050Data) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    Cavalry6050Data gm = AssetDatabase.LoadAssetAtPath<Cavalry6050Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Cavalry6050Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.data = CsvReader.Deserialize<Cavalry6050Data.DataCavalry>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}
