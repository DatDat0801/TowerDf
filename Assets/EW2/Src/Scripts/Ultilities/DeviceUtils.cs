using UnityEngine;

namespace EW2
{
    public enum GraphicsLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    public class DeviceUtils
    {
        public enum Platform
        {
            Android = 0,
            iOS = 1,
            Other = 2,
        }

        private static bool s_isFirstSetup;
        private static int s_nativeScreenWidth;
        private static int s_nativeScreenHeight;

        public static void AutoSetupDevice()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_IOS
            QualitySettings.vSyncCount = 1;
#elif UNITY_ANDROID
            QualitySettings.vSyncCount = 0;
#endif
        }

        public static void SetUpBatterySave(bool isSaveBattery)
        {
            if (isSaveBattery)
            {
                Application.targetFrameRate = 30;
            }
            else
            {
                Application.targetFrameRate = 50;
            }

            Log.Info("Set target frame rate to " + Application.targetFrameRate);
        }

        public static void SetUpQualityLevel(int quality)
        {
            if (!s_isFirstSetup)
            {
                s_isFirstSetup = true;
                s_nativeScreenWidth = Screen.width;
                s_nativeScreenHeight = Screen.height;
            }

            switch ((GraphicsLevel)quality)
            {
                case GraphicsLevel.Low:
                    Application.targetFrameRate = 30;
                    QualitySettings.SetQualityLevel(0, true);
                    Screen.SetResolution((int)(s_nativeScreenWidth * 0.75f), (int)(s_nativeScreenHeight * 0.75f), true);
                    break;

                case GraphicsLevel.Medium:
                    Application.targetFrameRate = 50;
                    QualitySettings.SetQualityLevel(1, true);
                    Screen.SetResolution((int)(s_nativeScreenWidth * 0.75f), (int)(s_nativeScreenHeight * 0.75f), true);
                    break;

                case GraphicsLevel.High:
                    Application.targetFrameRate = 60;
                    QualitySettings.SetQualityLevel(1, true);
                    if (Screen.width != s_nativeScreenWidth || Screen.height != s_nativeScreenHeight)
                    {
                        Screen.SetResolution(s_nativeScreenWidth, s_nativeScreenHeight, true);
                    }

                    break;
            }

            ClearGarbage();
        }

        public static void ClearGarbage()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();

            Log.Info("Manual run GC");
        }

        public static int GetDevicePlatform()
        {
            int platformCode = 0;

#if UNITY_ANDROID
            platformCode = (int)Platform.Android;
#elif UNITY_IOS
        platformCode = (int)Platform.iOS;
#else
        platformCode = (int)Platform.Other;
#endif

            return platformCode;
        }

        public static bool IsLowEndDevice()
        {
            bool isLowGraphics = true;

#if UNITY_IOS
        isLowGraphics = SystemInfo.systemMemorySize < 600;
#elif UNITY_ANDROID
            isLowGraphics = SystemInfo.graphicsMemorySize < 750;
#endif

            Debug.Log("RAM: " + SystemInfo.systemMemorySize);
            Debug.Log("GPU: " + SystemInfo.graphicsMemorySize);

            return isLowGraphics;
        }
    }
}