using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zitga.Localization
{
    public class LocalizeProvider
    {
        private readonly Localization localization;
        
        private readonly Dictionary<string, LanguageData> data;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localization"></param>
        public LocalizeProvider(Localization localization)
        {
            this.localization = localization;
            
            data = new Dictionary<string, LanguageData>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void ClearData()
        {
            data.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private LanguageData Load(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new Exception("category is empty");
            }
            
            if (IsContain(category))
            {
                throw new Exception("category is exist: " + category);
            }

            var path = $"Localization/{localization.localCultureInfo.Name}/{category}";
            
            return Resources.Load<LanguageData>(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string category, string key)
        {
            if (IsContain(category) == false)
            {
                var categoryData = Load(category);
                data.Add(category, categoryData);
            }
            
            try
            {
                return data[category].data[key];
            }
            catch (Exception e)
            {
                Debug.LogAssertion($"Key is not exist: {category}-{key} {e}");
                return $"#{category}-{key}";
                //  return string.Empty;
            }
        }

        /// <summary>
        /// Check need load or not
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool IsContain(string category)
        {
            return data.ContainsKey(category);
        }
    }
}