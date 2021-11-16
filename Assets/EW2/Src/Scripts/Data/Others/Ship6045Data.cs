using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class Ship6045Data : ScriptableObject
    {
        public DataCanon[] data;

        [System.Serializable]
        public class DataCanon
        {
            public int damage;

            public float timeCooldown;

            public DamageType damageType;
        }

        public DataCanon GetDataCanon()
        {
            return data[0];
        }
    }

#if UNITY_EDITOR
    public class Ship6045DataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/ship_6045.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(Ship6045Data) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    Ship6045Data gm = AssetDatabase.LoadAssetAtPath<Ship6045Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<Ship6045Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.data = CsvReader.Deserialize<Ship6045Data.DataCanon>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}