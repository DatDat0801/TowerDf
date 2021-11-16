using System;
using UnityEngine;

namespace EW2
{
    public class TournamentNerfConfig : ScriptableObject
    {
        public TournamentStatData[] tournamentNerfDatas;
        public TournamentStatData GetNerfDataById(int statId)
        {
            return Array.Find(this.tournamentNerfDatas, data => data.statId == statId);
        }
    }
}