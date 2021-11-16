using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class DefensiveModeMapData : ScriptableObject
    {
        public int defensiveMapId;
        public int modeId;

        public WaveInfo[] waveInfos;
        public WaveInfo[] loopWaveInfos;
        public DefensiveEnemyConfig[] enemyConfig;

        private Dictionary<int, List<WaveInfo>> _dictMapData;
        private Dictionary<int, List<WaveInfo>> _dictMapLoopData;
        
        private void AnalyzeData()
        {
            this._dictMapData = new Dictionary<int, List<WaveInfo>>();

            foreach (var waveInfo in waveInfos)
            {
                if (!this._dictMapData.ContainsKey(waveInfo.wave))
                {
                    var listWave = new List<WaveInfo>();

                    listWave.Add(waveInfo);

                    this._dictMapData.Add(waveInfo.wave, listWave);
                }
                else
                {
                    this._dictMapData[waveInfo.wave].Add(waveInfo);
                }
            }
        }

        public DefensiveEnemyConfig GetDefensiveEnemyConfig()
        {
            //get the first enemy config
            return enemyConfig[0];
        }
        private void AnalyzeLoopData()
        {
            this._dictMapLoopData = new Dictionary<int, List<WaveInfo>>();

            foreach (var waveInfo in this.loopWaveInfos)
            {
                if (!this._dictMapLoopData.ContainsKey(waveInfo.wave))
                {
                    var listWave = new List<WaveInfo>();

                    listWave.Add(waveInfo);

                    this._dictMapLoopData.Add(waveInfo.wave, listWave);
                }
                else
                {
                    this._dictMapLoopData[waveInfo.wave].Add(waveInfo);
                }
            }
        }
        
        public Dictionary<int, List<WaveInfo>> GetMapData()
        {
            if (this._dictMapData == null)
            {
                AnalyzeData();
            }

            return this._dictMapData;
        }
        
        public Dictionary<int, List<WaveInfo>> GetMapLoopData()
        {
            if (this._dictMapLoopData == null)
            {
                AnalyzeLoopData();
            }

            return this._dictMapLoopData;
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
    }

    [Serializable]
    public struct DefensiveEnemyConfig
    {
        public float hpRatioIncrease;
        public float damageRatioIncrease;
        public float armorRatioIncrease;
        public float resistanceRatioIncrease;
        public float critDamageRatioIncrease;
        public float tricksterDamage;
    }
    
#if UNITY_EDITOR
    public class DefensiveEditorMap : ScriptableObject
    {
        public int defensiveMapId;
    }

    public class DefensiveModeWaveDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvWaveFormat = "Assets/EW2/CSV/Maps/DefensiveMaps/";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvWaveFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    //Editor map
                    string nameBaseCsv = $"{csvWaveFormat}/defensive_editor_map.csv";

                    TextAsset dataBase = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    var editorMaps = CsvReader.Deserialize<DefensiveEditorMap>(dataBase.text);

                    foreach (DefensiveEditorMap defensiveEditorMap in editorMaps)
                    {
                        var indexMap = defensiveEditorMap.defensiveMapId;
                        // get asset file
                        string nameAsset = $"defensive_map_{indexMap}.asset";

                        string assetFile = "Assets/EW2/Resources/CSV/DefensiveMaps/" + nameAsset;

                        DefensiveModeMapData gm = AssetDatabase.LoadAssetAtPath<DefensiveModeMapData>(assetFile);

                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<DefensiveModeMapData>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        gm.defensiveMapId = defensiveEditorMap.defensiveMapId;
                        gm.modeId = 3;
                        //normal wave
                        string namewaveCsv = $"{csvWaveFormat}/defensive_waves_{indexMap}.csv";
                        TextAsset dataWave = AssetDatabase.LoadAssetAtPath<TextAsset>(namewaveCsv);
                        gm.waveInfos = CsvReader.Deserialize<WaveInfo>(dataWave.text);

                        //loop waves
                        string loopWaveName = $"{csvWaveFormat}/defensive_waves_loop_{indexMap}.csv";
                        TextAsset dataWaveLoop = AssetDatabase.LoadAssetAtPath<TextAsset>(loopWaveName);
                        gm.loopWaveInfos = CsvReader.Deserialize<WaveInfo>(dataWaveLoop.text);

                        //enemy stat
                        string enemyStatConfig = $"{csvWaveFormat}/enemy_stat_config_{indexMap}.csv";
                        TextAsset configAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(enemyStatConfig);
                        gm.enemyConfig = CsvReader.Deserialize<DefensiveEnemyConfig>(configAsset.text);

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