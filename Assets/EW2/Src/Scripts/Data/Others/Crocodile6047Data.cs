using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Crocodile6047Data : ScriptableObject
    {
        public DataCrocodile[] data;

        [System.Serializable]
        public class DataCrocodile
        {
            public int numberTarget;

            public float timeCooldown;

            public int[] listUnitIgnor;

        }

        public DataCrocodile GetDataCorcodile()
        {
            return data[0];
        }
    }

#if UNITY_EDITOR
    public class DataCrocodilePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/crocodile_6047.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Crocodile6047Data) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    Crocodile6047Data gm = AssetDatabase.LoadAssetAtPath<Crocodile6047Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Crocodile6047Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.data = CsvReader.Deserialize<Crocodile6047Data.DataCrocodile>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}