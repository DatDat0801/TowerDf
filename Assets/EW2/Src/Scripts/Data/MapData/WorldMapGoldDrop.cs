using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class WorldMapGoldDrop : ScriptableObject
    {
        public GoldDropInfo[] goldDropInfos;

        [System.Serializable]
        public class GoldDropInfo
        {
            public int stageId;
            public int goldWave0;
            public int goldWave1;
            public int goldWave2;
            public int goldWave3;
            public int goldWave4;
            public int goldWave5;
            public int goldWave6;
            public int goldWave7;
            public int goldWave8;
            public int goldWave9;
            public int goldWave10;
            public int goldWave11;
            public int goldWave12;
            public int goldWave13;
            public int goldWave14;
            public int goldWave15;
            public int goldWave16;
        }

        public List<int> GetGoldDropByMapId(int mapId)
        {
            foreach (var info in goldDropInfos)
            {
                if (info.stageId == mapId)
                {
                    return GenGoldDrop(info);
                }
            }

            return new List<int>();
        }

        private List<int> GenGoldDrop(GoldDropInfo goldDropInfo)
        {
            var results = new List<int>();

            results.Add(goldDropInfo.goldWave0);
            results.Add(goldDropInfo.goldWave1);
            results.Add(goldDropInfo.goldWave2);
            results.Add(goldDropInfo.goldWave3);
            results.Add(goldDropInfo.goldWave4);
            results.Add(goldDropInfo.goldWave5);
            results.Add(goldDropInfo.goldWave6);
            results.Add(goldDropInfo.goldWave7);
            results.Add(goldDropInfo.goldWave8);
            results.Add(goldDropInfo.goldWave9);
            results.Add(goldDropInfo.goldWave10);
            results.Add(goldDropInfo.goldWave11);
            results.Add(goldDropInfo.goldWave12);
            results.Add(goldDropInfo.goldWave13);
            results.Add(goldDropInfo.goldWave14);
            results.Add(goldDropInfo.goldWave15);
            results.Add(goldDropInfo.goldWave16);

            return results;
        }
    }

#if UNITY_EDITOR
    public class WorldGoldDropPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Maps/Campaign/WorldMapGoldDrops";
            string csvFormat1 = "Assets/EW2/CSV/Maps/Campaign_1/WorldMapGoldDrops";

            int indexWorld = -1;

            int indexMode = -1;

            foreach (string str in importedAssets)
            {
                if (indexWorld >= GameConfig.NumberWord) return;

                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // handle name file
                    var arrString = str.Split('/');
                    foreach (var element in arrString)
                    {
                        if (element.EndsWith(".csv"))
                        {
                            var nameFile = element.Remove(element.Length - 4);

                            var arrStringFile = nameFile.Split('_');
                            for (int i = 0; i < arrStringFile.Length; i++)
                            {
                                int worldId;

                                if (int.TryParse(arrStringFile[i], out worldId))
                                {
                                    indexWorld = worldId;

                                    int.TryParse(arrStringFile[i + 1], out indexMode);

                                    break;
                                }
                            }


                            break;
                        }
                    }

                    if (indexWorld < 0 || indexMode < 0) continue;

                    // get asset file
                    string nameAsset = $"Map_Gold_Drop_{indexWorld}_{indexMode}.asset";
                    string assetFile = "Assets/EW2/Resources/CSV/MapCampaigns/" + nameAsset;
                    WorldMapGoldDrop gm = AssetDatabase.LoadAssetAtPath<WorldMapGoldDrop>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<WorldMapGoldDrop>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    // get stat base
                    string nameBaseCsv = $"{csvFormat}/world_{indexWorld}_{indexMode}_map_gold_drop.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    gm.goldDropInfos = CsvReader.Deserialize<WorldMapGoldDrop.GoldDropInfo>(data.text);

                    EditorUtility.SetDirty(gm);
                    
                    // data test 1
                    string nameAsset1 = $"Map_Gold_Drop_{indexWorld}_{indexMode}.asset";
                    string assetFile1 = "Assets/EW2/Resources/CSV/MapCampaigns_1/" + nameAsset;
                    WorldMapGoldDrop gm1 = AssetDatabase.LoadAssetAtPath<WorldMapGoldDrop>(assetFile1);
                    if (gm1 == null)
                    {
                        gm1 = ScriptableObject.CreateInstance<WorldMapGoldDrop>();
                        AssetDatabase.CreateAsset(gm1, assetFile1);
                    }

                    // get stat base
                    string nameBaseCsv1 = $"{csvFormat1}/world_{indexWorld}_{indexMode}_map_gold_drop.csv";

                    TextAsset data1 = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv1);

                    gm1.goldDropInfos = CsvReader.Deserialize<WorldMapGoldDrop.GoldDropInfo>(data1.text);

                    EditorUtility.SetDirty(gm1);
                    
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);

                    indexWorld++;
                }
            }
        }
    }
#endif
}