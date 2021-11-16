using SecurityHelper;
using System;
using UnityEngine;

namespace ZitgaSaveLoad
{
    public class SaveOutbound
    {
        public AuthProvider Provider { get; private set; }

        /// <summary>
        /// id which uniquely identify an user
        /// it's can be Google Play Service user id, Apple id...
        /// </summary>
        public string Token { get; private set; }

        public string Data { get; set; }

        public SnapshotType SnapshotType { get; set; }

        public string Description { get; set; }

        public string DeviceModel { get; private set; }

        public string DeviceName { get; private set; }

        public string DeviceId { get; private set; }

        public long CreatedTime { get; private set; }

        public SaveOutbound(AuthProvider provider, string token)
        {
            Provider = provider;
            Token = token;

            DeviceModel = SystemInfo.deviceModel;
            DeviceName = SystemInfo.deviceName;
            DeviceId = SystemInfo.deviceUniqueIdentifier;

            CreatedTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public void SetData(string data, SnapshotType snapshotType, string description)
        {
            Data = StringCompressor.CompressString(data);
            SnapshotType = snapshotType;
            Description = description;
        }

        public string GetSignature(string gameVersion)
        {
            return (int)SnapshotType + Data + Description + gameVersion + DeviceModel + DeviceName + DeviceId + CreatedTime;
        }

        public string CreateHash(string secret, string gameVersion)
        {
            string signature = GetSignature(gameVersion);
            return HashHelper.HashSHA256((int)Provider + Token + signature + secret);
        }

        public SaveMetadata CreateSaveMetadata(string secret, string gameVersion)
        {
            SaveMetadata result = new SaveMetadata();

            result.SnapshotType = (int)SnapshotType;
            result.Description = Description;
            result.GameVersion = gameVersion;

            result.DeviceModel = DeviceModel;
            result.DeviceName = DeviceName;
            result.DeviceId = DeviceId;

            result.CreatedTime = CreatedTime;
            result.Hash = CreateHash(secret, gameVersion);

            return result;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Data))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Description))
            {
                return false;
            }

            return true;
        }
    }
}