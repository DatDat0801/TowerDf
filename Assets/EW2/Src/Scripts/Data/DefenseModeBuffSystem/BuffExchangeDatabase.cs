using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class BuffExchangeDatabase : ScriptableObject
    {
        public BuffExchangeRate[] exchangeRates;
    }
    [Serializable]
    public class BuffExchangeRate
    {
        public string buffId;
        public int price;
    }
#if UNITY_EDITOR
    public class BuffExchangeDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/DefenseModeBuff";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(BuffExchangeDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/DefenseModeBuff/" + nameAsset;
                    BuffExchangeDatabase gm = AssetDatabase.LoadAssetAtPath<BuffExchangeDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<BuffExchangeDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }
                    
                    string nameBaseCsv = $"{csvFormat}/buff_exchange_database.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.exchangeRates = CsvReader.Deserialize<BuffExchangeRate>(data.text);


                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}