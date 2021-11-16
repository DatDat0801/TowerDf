using System;
using EW2;

namespace Zitga.CsvTools
{
    // ---------------
//  String => Int
// ---------------
    [Serializable]
    public class StringIntDictionary : SerializableDictionary<string, int> {}

// ---------------
//  String => String
// ---------------
    [Serializable]
    public class StringStringDictionary : SerializableDictionary<string, string> {}
    
    // ---------------
//  int => TowerRaiseCost
// ---------------
    [Serializable]
    public class IntTowerRaiseCostDictionary : SerializableDictionary<int, TowerRaiseCost> {}
    
    //  int => TowerRaiseCost
// ---------------
    [Serializable]
    public class IntRewardCampaignDictionary : SerializableDictionary<int, RewardCampaignInfo> {}
    
}



