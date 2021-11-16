using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZitgaSaveLoad
{
    public class ZitgaSaveLoadExampleUIController : MonoBehaviour
    {
        public Dropdown authProviderDropdown;
        public InputField authTokenText;

        public InputField dataText;
        public InputField descriptionText;
        public Dropdown snapshotTypeDropdown;

        public void Start()
        {
            InitAuthProviderDropdown();
            InitSnapshotTypeDropdown();
        }

        public AuthProvider GetAuthProvider()
        {
            foreach (AuthProvider provider in Enum.GetValues(typeof(AuthProvider)))
            {
                if (provider.ToString().Equals(authProviderDropdown.options[authProviderDropdown.value].text))
                {
                    return provider;
                }
            }

            return AuthProvider.FACEBOOK;
        }

        public string GetAuthToken()
        {
            return authTokenText.text;
        }

        public string GetData()
        {
            return dataText.text;
        }

        public string GetDescription()
        {
            return descriptionText.text;
        }

        public SnapshotType GetSnapshotType()
        {
            foreach (SnapshotType snapshotType in Enum.GetValues(typeof(SnapshotType)))
            {
                if (snapshotType.ToString().Equals(snapshotTypeDropdown.options[snapshotTypeDropdown.value].text))
                {
                    return snapshotType;
                }
            }

            return SnapshotType.AUTO;
        }

        private void InitAuthProviderDropdown()
        {
            List<string> options = new List<string>();
            foreach (AuthProvider provider in Enum.GetValues(typeof(AuthProvider)))
            {
                options.Add(provider.ToString());
            }
            authProviderDropdown.ClearOptions();
            authProviderDropdown.AddOptions(options);
        }

        private void InitSnapshotTypeDropdown()
        {
            List<string> options = new List<string>();
            foreach (SnapshotType snapshotType in Enum.GetValues(typeof(SnapshotType)))
            {
                options.Add(snapshotType.ToString());
            }
            snapshotTypeDropdown.ClearOptions();
            snapshotTypeDropdown.AddOptions(options);
        }
    }
}