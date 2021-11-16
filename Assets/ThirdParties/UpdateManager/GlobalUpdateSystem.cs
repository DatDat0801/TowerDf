using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zitga.Update
{
    public class GlobalUpdateSystem : Singleton<GlobalUpdateSystem>
    {
        private readonly List<IUpdateSystem> updates;
        
        private readonly List<IUpdateSystem> removes;
        
        private readonly List<IUpdateSystem> adds;

        /// <summary>
        /// Current number update function
        /// </summary>
        public int Count
        {
            get { return updates.Count; }
        }

        public GlobalUpdateSystem()
        {
            updates = new List<IUpdateSystem>();
            
            removes = new List<IUpdateSystem>();
            
            adds = new List<IUpdateSystem>();
        }

        private void Update()
        {
            if (removes.Count > 0)
            {
                foreach (IUpdateSystem remove in removes)
                {
                    updates.Remove(remove);
                }
                
                removes.Clear();
            }
            
            if (adds.Count > 0)
            {
                foreach (IUpdateSystem add in adds)
                {
                    updates.Add(add);
                }
                
                adds.Clear();
            }

            var deltaTime = Time.deltaTime;
            
            foreach (IUpdateSystem update in updates)
            {
                update?.OnUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Add update function from someone object what need to call
        /// </summary>
        /// <param name="update"></param>
        /// <exception cref="Exception"></exception>
        public void Add(IUpdateSystem update)
        {
            // Debug.Log("Add: " + update);

            if (removes.Contains(update))
            {
                removes.Remove(update);
            }
            else
            {
                adds.Add(update);
                
                if (updates.Contains(update))
                {
                    throw new Exception($"Object is exist in updates: {nameof(update)}");
                }
            }
        }

        /// <summary>
        /// remove update from a object when it does not use
        /// </summary>
        /// <param name="update"></param>
        /// <exception cref="Exception"></exception>
        public void Remove(IUpdateSystem update)
        {
            // Debug.Log("Remove: " + update);
            
            if (removes.Contains(update) == false)
            {
                if (adds.Contains(update))
                {
                    adds.Remove(update);
                }
                else
                {
                    removes.Add(update);
                }
            }
            else
            {
                throw new Exception($"Object is not exist in updates: {nameof(update)}");
            }
        }
    }
}