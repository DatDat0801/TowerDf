using System;
using System.Collections.Generic;

namespace EW2
{
    public class DefensiveBuffUserData : IDisposable
    {
        //buffId, quantity
        public Dictionary<string, int> UserBuff { get; }

        public DefensiveBuffUserData()
        {
            UserBuff = new Dictionary<string, int>();
        }

        public void Add(string key)
        {
            if (UserBuff.ContainsKey(key))
            {
                UserBuff[key]++;
            }
            else
            {
                UserBuff.Add(key, 1);
            }
        }

        public void Dispose()
        {
            UserBuff.Clear();
        }
    }
}