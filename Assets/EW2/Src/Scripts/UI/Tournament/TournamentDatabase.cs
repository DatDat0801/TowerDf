using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TournamentDatabase : ScriptableObject
    {
        //public TournamentStat[] statsDb;
        public TournamentHeroUnit[] heroUnitDb;
        public TournamentTowerUnit[] towerUnitDb;
        public TournamentSpellUnit[] spellUnitDb;
        public TournamentMap[] mapsPool;

        public TournamentMap GetTournamentMap(int currentMapId)
        {
            var index = Array.FindIndex(this.mapsPool, map => map.tournamentMapId == currentMapId);
            if (index == -1)
            {
                return this.mapsPool[0];
            }

            return this.mapsPool[index];
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            this.heroUnitDb = Array.FindAll(this.heroUnitDb, unit => unit.heroId != 0).ToArray();
            this.towerUnitDb = Array.FindAll(this.towerUnitDb, unit => unit.towerId != 0).ToArray();
            this.spellUnitDb = Array.FindAll(this.spellUnitDb, unit => unit.spellId != 0).ToArray();
        }
#endif

    }
    // [Serializable]
    // public struct TournamentStat
    // {
    //     public int statId;
    //     public float value;
    // }

    [Serializable]
    public struct TournamentHeroUnit
    {
        public int heroId;
    }
    [Serializable]
    public struct TournamentTowerUnit
    {
        public int towerId;
    }
    [Serializable]
    public struct TournamentSpellUnit
    {
        public int spellId;
    }

    [Serializable]
    public struct TournamentMap
    {
        public int tournamentMapId;
        public int worldId;
        public int mapId;
    }
#if UNITY_EDITOR
    public class TournamentStatDatabasePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TournamentDatabase) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tournament/" + nameAsset;
                    TournamentDatabase gm = AssetDatabase.LoadAssetAtPath<TournamentDatabase>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TournamentDatabase>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }
                    
                    string nameBaseCsv = $"{csvFormat}/tournament_stat.csv";
                    string mapPoolCsv = $"{csvFormat}/tournament_map_pool.csv";

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
                    //gm.statsDb = CsvReader.Deserialize<TournamentStat>(data.text);
                    gm.heroUnitDb = CsvReader.Deserialize<TournamentHeroUnit>(data.text);
                    gm.towerUnitDb = CsvReader.Deserialize<TournamentTowerUnit>(data.text);
                    gm.spellUnitDb = CsvReader.Deserialize<TournamentSpellUnit>(data.text);

                    TextAsset mapPoolData = AssetDatabase.LoadAssetAtPath<TextAsset>(mapPoolCsv);
                    gm.mapsPool = CsvReader.Deserialize<TournamentMap>(mapPoolData.text);
                    
                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}