using System;
using UnityEngine;

namespace EW2
{
    public class SettingData
    {
        public bool music = true;

        public bool sound = true;

        public bool shake = true;

        public bool batterySave = true;

        public bool notification = true;

        public int graphicQuality = 2;
        
        public int countOnlineTime = 0;

        public int maxCampaignId = -1;
        //saved data log
        public int savedToday;
        public DateTime lastSaved;

        public SystemLanguage userLanguage = SystemLanguage.Unknown;

        public bool CheckCampaignFirstUnlock(int campId)
        {
            return campId == maxCampaignId;
        }
    }
}