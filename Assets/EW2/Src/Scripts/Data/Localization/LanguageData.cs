#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
#endif

using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class LanguageData : ScriptableObject
    {
        public StringStringDictionary data;
    }

#if UNITY_EDITOR
    public class LanguagePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.IndexOf("CSV/Localization", StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                    string assetFile = str.Replace(".csv", ".asset")
                        .Replace("CSV/Localization", "Resources/CSV/Localization");
                    LanguageData gm = AssetDatabase.LoadAssetAtPath<LanguageData>(assetFile);
                    if (gm == null)
                    {
                        if (File.Exists(assetFile) == false)
                            Directory.CreateDirectory(assetFile);

                        gm = ScriptableObject.CreateInstance<LanguageData>();

                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    var rows = CsvReader.Deserialize<RowData>(data.text, '~');

                    gm.data.Clear();
                    foreach (var row in rows)
                    {
                        gm.data.Add(row.key, row.value);
                    }

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }

        public class RowData
        {
            public string key;
            public string value;
        }
    }
#endif
}