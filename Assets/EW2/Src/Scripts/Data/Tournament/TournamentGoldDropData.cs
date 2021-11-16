using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class TournamentGoldDropData : ScriptableObject
    {
        public TournamentGoldDrop[] goldDrops;

        public int GetGoldDropByWaveId(int waveId)
        {
            var index = Array.FindIndex(this.goldDrops, drop => drop.waveId == waveId);
            if (index != -1)
            {
                return this.goldDrops[index].gold;
            }

            return 0;
        }

        public List<int> GetGoldDrops()
        {
            var list = new List<int>();
            for (var i = 0; i < this.goldDrops.Length; i++)
            {
                list.Add(this.goldDrops[i].gold);
            }

            return list;
        }
    }

    [Serializable]
    public struct TournamentGoldDrop
    {
        public int waveId;
        public int gold;
    }

#if UNITY_EDITOR

    public class TournamentGoldDropDataPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Maps/TournamentMaps/";

            //Editor map
            string csvWaveFormat = "Assets/EW2/CSV/Maps/TournamentMaps/";
            string nameBaseCsv = $"{csvWaveFormat}/tournament_editor_map.csv";
            TextAsset dataBase = AssetDatabase.LoadAssetAtPath<TextAsset>(nameBaseCsv);
            var editorMaps = CsvReader.Deserialize<TournamentEditorMap>(dataBase.text);

            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    foreach (var map in editorMaps)
                    {
                        var csvFileName = $"tournament_gold_drop_map_{map.tournamentMapId}.csv";
                        if (!str.Equals(csvFormat + csvFileName)) continue;
                        
                        // get asset file
                        string nameAsset = $"tournament_gold_drop_map_{map.tournamentMapId}" + ".asset";
                        string assetFile = "Assets/EW2/Resources/CSV/TournamentMaps/" + nameAsset;

                        // data orgin
                        TournamentGoldDropData gm = AssetDatabase.LoadAssetAtPath<TournamentGoldDropData>(assetFile);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance<TournamentGoldDropData>();
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }


                        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat + csvFileName);

                        gm.goldDrops = CsvReader.Deserialize<TournamentGoldDrop>(data.text);
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