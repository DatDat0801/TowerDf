using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace  EW2
{
    public class PoisonDump6051Data : ScriptableObject
    {
        public DataPoisonDump[] data;

        [Serializable]
        public class DataPoisonDump
        {
            public float damage;
            public float interval;
            public DamageType damageType;
            public float duration;
            public EffectOnType[] effectOnType;
        }

        public DataPoisonDump GetDataPoisonDump()
        {
            return data[0];
        }
    }
    
    #if UNITY_EDITOR
    public class DataPoinsonDumpPostprocessor: AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Others/poison_dump_6051.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(PoisonDump6051Data) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    PoisonDump6051Data gm = AssetDatabase.LoadAssetAtPath<PoisonDump6051Data>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<PoisonDump6051Data>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.data = CsvReader.Deserialize<PoisonDump6051Data.DataPoisonDump>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
    #endif
}