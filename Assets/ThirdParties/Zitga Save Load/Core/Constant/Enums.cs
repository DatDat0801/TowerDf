namespace ZitgaSaveLoad
{
    public enum AuthProvider
    {
        FACEBOOK = 0,
        APPLE = 1,
        GOOGLE = 2,

        WINDOWS_DEVICE = 10,
        MAC_DEVICE = 11,
        LINUX_DEVICE = 12,

        ANDROID_DEVICE = 20,
        IOS_DEVICE = 30,

        WEB = 40,
    }

    public enum SnapshotType
    {
        AUTO = 0,
        MANUAL = 1,
    }
}