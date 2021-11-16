using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class TournamentTicketExchange
    {
        public int ticketMax;
        public int limitTicketExchange;
        public int costExchange;
        public int moneyTypeExchange;
        public int valueExchange;
    }

    public class TournamentTicketConfig : ScriptableObject
    {
        public TournamentTicketExchange[] tournamentTicketExchanges;
    }

#if UNITY_EDITOR

    public class TournamentTicketConfigPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Tournament/tournament_ticket.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(TournamentTicketConfig) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Tournament/" + nameAsset;
                    TournamentTicketConfig gm = AssetDatabase.LoadAssetAtPath<TournamentTicketConfig>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<TournamentTicketConfig>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);

                    gm.tournamentTicketExchanges = CsvReader.Deserialize<TournamentTicketExchange>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}