using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [System.Serializable]
    public class MapStatBase
    {
        public int stamina;
        public int gold;
        public int lifePoint;
        public int condition2Star;
        public int condition3Star;

        public int GetStar(int currentLifePoint)
        {
            if (currentLifePoint >= condition3Star)
                return 3;
            if (currentLifePoint >= condition2Star)
                return 2;
            return 1;
        }
    }

    [System.Serializable]
    public class WaveInfo
    {
        public int wave;
        public int gateSpawnId;
        public int line;
        public int enemyId;
        public int amount;
        public float spacing;
        public float delaySpawn;
    }

    public class MapCampaignInfo : ScriptableObject
    {
        public int stageId;

        public int worldId;

        public int modeId;

        public MapStatBase mapStatBase;

        public WaveInfo[] waveInfos;

        private Dictionary<int, List<WaveInfo>> dictMapData;

        private List<int> listEnemyUnlock;

        private void AnalyzeData()
        {
            dictMapData = new Dictionary<int, List<WaveInfo>>();

            listEnemyUnlock = new List<int>();

            foreach (var waveInfo in waveInfos)
            {
                if (!dictMapData.ContainsKey(waveInfo.wave))
                {
                    var listWave = new List<WaveInfo>();

                    listWave.Add(waveInfo);

                    dictMapData.Add(waveInfo.wave, listWave);
                }
                else
                {
                    dictMapData[waveInfo.wave].Add(waveInfo);
                }

                if (!listEnemyUnlock.Contains(waveInfo.enemyId))
                {
                    listEnemyUnlock.Add(waveInfo.enemyId);
                }
            }
        }

        public Dictionary<int, List<WaveInfo>> GetMapData()
        {
            if (dictMapData == null)
            {
                AnalyzeData();
            }

            return dictMapData;
        }

        public List<int> GetEnemyUnlock()
        {
            if (listEnemyUnlock == null)
            {
                AnalyzeData();
            }

            return listEnemyUnlock;
        }

        public MapStatBase GetMapStatBase()
        {
            return mapStatBase;
        }

        public Dictionary<int, int> GetTotalEnemyInMap()
        {
            var result = new Dictionary<int, int>();

            foreach (var waveInfo in waveInfos)
            {
                if (!result.ContainsKey(waveInfo.enemyId))
                {
                    result.Add(waveInfo.enemyId, 1);
                }
                else
                {
                    result[waveInfo.enemyId]++;
                }
            }

            return result;
        }

        public static int GetCampaignId(int worldId, int mapId, int modeId)
        {
            return worldId * 10000 + mapId * 100 + modeId;
        }

        public static (int, int, int) GetWorldMapModeId(int campaignId)
        {
            int modeId = campaignId % 100;

            int mapId = (campaignId % 10000) / 100;

            int worldId = campaignId / 10000;

            return (worldId, mapId, modeId);
        }
    }

#if UNITY_EDITOR

    [System.Serializable]
    public class MapStatBaseEditor
    {
        public int stageId;
        public MapStatBase mapStatBase;
    }

    public class MapCampaignInfoPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvWaveFormat = "Assets/EW2/CSV/Maps/Campaign/WorldMap/";
            string csvBaseFormat = "Assets/EW2/CSV/Maps/Campaign/WorldMapBase/";

            string csvWaveFormat1 = "Assets/EW2/CSV/Maps/Campaign_1/WorldMap/";
            string csvBaseFormat1 = "Assets/EW2/CSV/Maps/Campaign_1/WorldMapBase/";

            int indexWorld = -1;
            int indexMode = -1;

            foreach (string str in importedAssets)
            {
                if ((str.IndexOf(csvWaveFormat, StringComparison.Ordinal) != -1 ||
                     str.IndexOf(csvBaseFormat, StringComparison.Ordinal) != -1) &&
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

                            Debug.AssertFormat(arrStringFile.Length == 4, "Name file is not valid: " + nameFile);

                            if (arrStringFile[1].Equals("base"))
                            {
                                indexWorld = int.Parse(arrStringFile[2]);

                                indexMode = int.Parse(arrStringFile[3]);
                            }
                            else
                            {
                                indexWorld = int.Parse(arrStringFile[1]);

                                indexMode = int.Parse(arrStringFile[3]);
                            }

                            break;
                        }
                    }

                    if (indexWorld < 0 || indexMode < 0) continue;

                    string nameBaseCsv = $"{csvBaseFormat}/map_base_{indexWorld}_{indexMode}.csv";

                    TextAsset dataBase = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    var mapStatBases = CsvReader.Deserialize<MapStatBaseEditor>(dataBase.text);

                    foreach (var mapStatBaseEditor in mapStatBases)
                    {
                        var indexMap = mapStatBaseEditor.stageId;
                        // get asset file
                        string nameAsset = $"Map_{indexWorld}_{indexMap}_{indexMode}.asset";

                        string assetFile = "Assets/EW2/Resources/CSV/MapCampaigns/" + nameAsset;

                        MapCampaignInfo gm = AssetDatabase.LoadAssetAtPath<MapCampaignInfo>(assetFile);

                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<MapCampaignInfo>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        // get stat base
                        gm.stageId = indexMap;

                        gm.worldId = indexWorld;

                        gm.modeId = indexMode;

                        string namewaveCsv = $"{csvWaveFormat}/map_{indexWorld}_{indexMap}_{indexMode}.csv";

                        gm.mapStatBase = mapStatBaseEditor.mapStatBase;

                        TextAsset dataWave = AssetDatabase.LoadAssetAtPath<TextAsset>(namewaveCsv);

                        gm.waveInfos = CsvReader.Deserialize<WaveInfo>(dataWave.text);

                        EditorUtility.SetDirty(gm);
                    }

                    // data test 1

                    string nameBaseCsv1 = $"{csvBaseFormat1}/map_base_{indexWorld}_{indexMode}.csv";

                    TextAsset dataBase1 = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv1);

                    var mapStatBases1 = CsvReader.Deserialize<MapStatBaseEditor>(dataBase1.text);

                    foreach (var mapStatBaseEditor in mapStatBases1)
                    {
                        var indexMap = mapStatBaseEditor.stageId;
                        // get asset file
                        string nameAsset = $"Map_{indexWorld}_{indexMap}_{indexMode}.asset";

                        string assetFile1 = "Assets/EW2/Resources/CSV/MapCampaigns_1/" + nameAsset;

                        MapCampaignInfo gm = AssetDatabase.LoadAssetAtPath<MapCampaignInfo>(assetFile1);

                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<MapCampaignInfo>();
                            AssetDatabase.CreateAsset(gm, assetFile1);
                        }

                        // get stat base
                        gm.stageId = indexMap;

                        gm.worldId = indexWorld;

                        gm.modeId = indexMode;

                        string namewaveCsv = $"{csvWaveFormat1}/map_{indexWorld}_{indexMap}_{indexMode}.csv";

                        gm.mapStatBase = mapStatBaseEditor.mapStatBase;

                        TextAsset dataWave = AssetDatabase.LoadAssetAtPath<TextAsset>(namewaveCsv);

                        gm.waveInfos = CsvReader.Deserialize<WaveInfo>(dataWave.text);

                        EditorUtility.SetDirty(gm);
                    }

                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}