using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class HeroCollection : ScriptableObject
    {
        public HeroCollectionInfo[] heroList;

        [Serializable]
        public class HeroCollectionInfo
        {
            public int heroId;

            public bool isFree;
        }

        public List<int> GetAllHeroes()
        {
            return heroList.Select(x => x.heroId).ToList();
        }

        public List<int> GetHeroes(bool isFree)
        {
            return heroList.Where(x => x.isFree == isFree).Select(x => x.heroId).ToList();
        }
    }

#if UNITY_EDITOR
    public class HeroCollectionPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Heroes/hero_collection.csv";

            int indexWorld = -1;

            int indexMode = -1;

            foreach (string str in importedAssets)
            {
                if (indexWorld >= GameConfig.NumberWord) return;

                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1)
                {
                    var assetFile = $"Assets/EW2/Resources/CSV/Units/{nameof(HeroCollection)}.asset";
                    var gm = AssetDatabase.LoadAssetAtPath<HeroCollection>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<HeroCollection>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.heroList = CsvReader.Deserialize<HeroCollection.HeroCollectionInfo>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}