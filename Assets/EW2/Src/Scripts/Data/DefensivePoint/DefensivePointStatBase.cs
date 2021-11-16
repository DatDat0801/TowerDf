using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class DefensivePointStatBase
    {
        public int hp;
    }
    public class DefensivePointData : ScriptableObject
    {
        /// <summary>
        /// Each level has a stat base
        /// </summary>
        public DefensivePointStatBase[] stats;

        public virtual List<string> GetDescStatSkillActive(int level)
        {
            return new List<string>();
        }
        public virtual List<string> GetDescStatSkillPassive(int level)
        {
            return new List<string>();
        }

#if UNITY_EDITOR
        public static T[] LoadAsset<T>(string csvFormat, string defensivePointId, string skillId)
        {
            string skillCsv = $"{csvFormat}/defensive_{defensivePointId}_passive_{skillId}.csv";

            TextAsset dataSkill = AssetDatabase.LoadAssetAtPath<TextAsset>(skillCsv);

            return CsvReader.Deserialize<T>(dataSkill.text);
        }
#endif
    }
}