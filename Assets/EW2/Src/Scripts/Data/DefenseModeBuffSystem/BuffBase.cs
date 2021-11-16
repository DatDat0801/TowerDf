using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class BuffBase : ScriptableObject
    {
        public string buffId;
        public virtual List<string> GetDescStatSkillActive()
        {
            return new List<string>();
        }

        public virtual List<string> GetDescStatSkillPassive()
        {
            return new List<string>();
        }
    }
}
