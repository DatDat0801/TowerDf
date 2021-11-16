using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class TournamentStatData
    {
        public int statId;

        public float ratioBonus;

        public int valueBonus;

        public RPGStatType statBonus;
    }

    public class TournamentBuffConfig : ScriptableObject
    {
        public TournamentStatData[] tournamentBuffDatas;

        public TournamentStatData GetBuffDataById(int buffId)
        {
            return Array.Find(this.tournamentBuffDatas, data => data.statId == buffId);
        }
    }



#if UNITY_EDITOR

    public class TournamentBuffConfigPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament/";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAssetBuff = nameof(TournamentBuffConfig) + ".asset";
                    string assetFileBuff = "Assets/EW2/Resources/CSV/Tournament/" + nameAssetBuff;
                    //Buff
                    TournamentBuffConfig gm = AssetDatabase.LoadAssetAtPath<TournamentBuffConfig>(assetFileBuff);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TournamentBuffConfig>();
                        AssetDatabase.CreateAsset(gm, assetFileBuff);
                    }

                    var buffFile = csvFormat + "tournament_buff.csv";
                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(buffFile);
                    gm.tournamentBuffDatas = CsvReader.Deserialize<TournamentStatData>(data.text);
                    //Nerf
                    string nameAssetNerf = nameof(TournamentNerfConfig) + ".asset";
                    string assetFileNerf = "Assets/EW2/Resources/CSV/Tournament/" + nameAssetNerf;
                    TournamentNerfConfig nerfConfig = AssetDatabase.LoadAssetAtPath<TournamentNerfConfig>(assetFileNerf);
                    if (nerfConfig == null)
                    {
                        nerfConfig = ScriptableObject.CreateInstance<TournamentNerfConfig>();
                        AssetDatabase.CreateAsset(nerfConfig, assetFileNerf);
                    }

                    var nerfFile = csvFormat + "tournament_nerf.csv";
                    TextAsset nerfData = AssetDatabase.LoadAssetAtPath<TextAsset>(nerfFile);
                    nerfConfig.tournamentNerfDatas = CsvReader.Deserialize<TournamentStatData>(nerfData.text);
                    EditorUtility.SetDirty(nerfConfig);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}