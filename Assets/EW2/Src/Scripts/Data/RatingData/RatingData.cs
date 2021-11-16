using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class RatingData : ScriptableObject
    {
        public MapIdTrigger[] mapIdTriggers;

        [Serializable]
        public class MapIdTrigger
        {
            public int mapId;
            public int difficulty;
        }
    }

#if UNITY_EDITOR
    public class RatingDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Rating/rating_trigger.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(RatingData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/RatingData/" + nameAsset;
                    RatingData gm = AssetDatabase.LoadAssetAtPath<RatingData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<RatingData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.mapIdTriggers = CsvReader.Deserialize<RatingData.MapIdTrigger>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}