using System;
using System.Globalization;
using UnityEngine;

namespace Zitga.Localization
{
    public sealed class Localization
    {
        private static readonly object InstanceLock = new object();
        private static Localization _instance;

        private readonly object cultureInfoLock = new object();
        private CultureInfo cultureInfo;
        private EventHandler cultureInfoChanged;

        private readonly LocalizeProvider localizeProvider;

        public static Localization Current
        {
            get
            {
                if (_instance != null)
                    lock (InstanceLock)
                    {
                        return _instance;
                    }

                lock (InstanceLock)
                {
                    if (_instance == null)
                        _instance = new Localization();
                    return _instance;
                }
            }
        }

        private Localization() : this(null)
        {
        }

        private Localization(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo ?? Locale.GetCultureInfo();

            localizeProvider = new LocalizeProvider(this);
        }

        public event EventHandler CultureInfoChanged
        {
            add { lock (cultureInfoLock) { cultureInfoChanged += value; } }
            remove { lock (cultureInfoLock) { cultureInfoChanged -= value; } }
        }

        
        public CultureInfo localCultureInfo
        {
            get => cultureInfo;
            set
            {
                if (value == null || (cultureInfo != null && cultureInfo.Equals(value)))
                    return;

                localizeProvider.ClearData();
                
                cultureInfo = value;
                
                OnCultureInfoChanged();
            }
        }

        public string Get(string category, string key)
        {
            return localizeProvider.Get(category, key);
        }

        private void RaiseCultureInfoChanged()
        {
            try
            {
                Debug.Log("language: " + cultureInfo.Name);
                cultureInfoChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnCultureInfoChanged()
        {
            RaiseCultureInfoChanged();
        }

    }
}
