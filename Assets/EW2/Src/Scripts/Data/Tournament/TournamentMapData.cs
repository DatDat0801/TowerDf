using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TournamentMapData : ScriptableObject
    {
        public int tournamentMapId;
        public int modeId;

        public WaveInfo[] waveInfos;
        public WaveInfo[] loopWaveInfos;
        public TournamentEnemyConfig[] enemyConfig;
        public TournamentMapConfig mapConfigs;
        

        private Dictionary<int, List<WaveInfo>> _dictMapData;
        private Dictionary<int, List<WaveInfo>> _dictMapLoopData;

        public TournamentMapConfig GetTournamentMapConfig()
        {
            return this.mapConfigs;
        }

        public TournamentEnemyConfig GetTournamentEnemyConfig()
        {
            return this.enemyConfig[0];
        }
        
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
    public class TournamentEnemyConfig
    {
        //public int startWaveLoop;
        public float hpRatioIncrease;
        public float damageRatioIncrease;
        public float armorRatioIncrease;
        public float resistanceRatioIncrease;
        public float critDamageRatioIncrease;
        
    }
    [Serializable]
    public struct TournamentMapConfig
    {
        public int campaignMapId;
        public int lifePoint;
        public int goldBase;
        // public int startWaveLoop;
        // public int endWaveLoop;

    }
    #if UNITY_EDITOR
    [Serializable]
    public class TournamentEditorMap
    {
        public int tournamentMapId;
    }

    public class TournamentWaveDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvWaveFormat = "Assets/EW2/CSV/Maps/TournamentMaps/";

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvWaveFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    //Editor map
                    string nameBaseCsv = $"{csvWaveFormat}/tournament_editor_map.csv";

                    TextAsset dataBase = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);

                    var editorMaps = CsvReader.Deserialize<TournamentEditorMap>(dataBase.text);

                    foreach (TournamentEditorMap tournamentEditorMap in editorMaps)
                    {
                        var indexMap = tournamentEditorMap.tournamentMapId;
                        // get asset file
                        string nameAsset = $"tournament_map_{indexMap}.asset";

                        string assetFile = "Assets/EW2/Resources/CSV/TournamentMaps/" + nameAsset;

                        TournamentMapData gm = AssetDatabase.LoadAssetAtPath<TournamentMapData>(assetFile);

                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<TournamentMapData>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        gm.tournamentMapId = tournamentEditorMap.tournamentMapId;
                        gm.modeId = 3;
                        //normal wave
                        string namewaveCsv = $"{csvWaveFormat}/tournament_waves_{indexMap}.csv";
                        TextAsset dataWave = AssetDatabase.LoadAssetAtPath<TextAsset>(namewaveCsv);
                        gm.waveInfos = CsvReader.Deserialize<WaveInfo>(dataWave.text);

                        //loop waves
                        string loopWaveName = $"{csvWaveFormat}/tournament_waves_loop_{indexMap}.csv";
                        TextAsset dataWaveLoop = AssetDatabase.LoadAssetAtPath<TextAsset>(loopWaveName);
                        gm.loopWaveInfos = CsvReader.Deserialize<WaveInfo>(dataWaveLoop.text);

                        //enemy stat
                        string enemyStatConfig = $"{csvWaveFormat}/enemy_stat_config_{indexMap}.csv";
                        TextAsset configAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(enemyStatConfig);
                        gm.enemyConfig = CsvReader.Deserialize<TournamentEnemyConfig>(configAsset.text);
                        
                        //map config
                        string mapConfigCsv = $"{csvWaveFormat}/tournament_map_config.csv";
                        TextAsset mapConfig = AssetDatabase.LoadAssetAtPath<TextAsset>(mapConfigCsv);
                        var configs = CsvReader.Deserialize<TournamentMapConfig>(mapConfig.text);
                        gm.mapConfigs = configs[tournamentEditorMap.tournamentMapId];
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