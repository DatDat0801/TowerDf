using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class TowerWorld
    {
        public int world;
        public int[] towers;
    }
    
    public class TowerWorldData : ScriptableObject
    {
        private Dictionary<int, TowerWorld> towerWorldDict = new Dictionary<int, TowerWorld>();
        
        public TowerWorld[] towerWorlds;

        public TowerWorld GetCurrentTowerWorld()
        {
            return GetTowerWorld(GamePlayController.Instance.WorldId);
        }

        public TowerWorld GetTowerWorld(int worldId)
        {
            if (towerWorldDict.TryGetValue(worldId, out var towerWorld))
            {
                return towerWorld;
            }

            foreach (var world in towerWorlds)
            {
                if (world.world.Equals(worldId))
                {
                    towerWorldDict.Add(worldId, world);

                    return world;
                }
            }
            
            throw new Exception("Can't find world: " + worldId);
        }
    }

#if UNITY_EDITOR
    public class TowerWorldPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Towers/tower_world";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TowerWorldData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Units/" + nameAsset;
                    TowerWorldData gm = AssetDatabase.LoadAssetAtPath<TowerWorldData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TowerWorldData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    gm.towerWorlds = CsvReader.Deserialize<TowerWorld>(data.text);
                    
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}