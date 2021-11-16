using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using EW2;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

using UnityEditor.Build.Reporting;
using Application = UnityEngine.Application;
using Object = UnityEngine.Object;


namespace Assets.Editor
{
    public class EditorBuild
    {
        static string GetProjectName()
        {
            return Application.productName;
        }

        static string[] GetScenePaths()
        {
            return (
                from scene in EditorBuildSettings.scenes
                where scene.enabled
                select scene.path
            ).ToArray();
        }

      

        

      
        static void PerformBuildAndroid()
        {
//            PerformLoadIPLoadBalancer();

            var arguments = System.Environment.GetCommandLineArgs();
            var locationOut = "";
            var buildVersion = "";
            var bundleVersionCodeStr = "";
            var dataVersion = "";
            var buildType = "";
            var storeVersion = "";
            var appName = "";

            for (var i = 0; i < arguments.Length; i++)
            {
                var id = arguments[i];
                if (id.Equals("-out"))
                {
                    locationOut = arguments[i + 1];
                    Debug.Log("locationOut: " + locationOut);
                }
                else if (id.Equals("-finalBuildVersion"))
                {
                    buildVersion = arguments[i + 1];
                    Debug.Log("buildVersion: " + buildVersion);
                }
                else if (id.Equals("-buildType"))
                {
                    buildType = arguments[i + 1];
                }
                else if (id.Equals("-dataVersion"))
                {
                    dataVersion = arguments[i + 1];
                }
                else if (id.Equals("-bundleVersionCode"))
                {
                    bundleVersionCodeStr = arguments[i + 1];
                }
                else if (id.Equals("-storeVersion"))
                {
                    storeVersion = arguments[i + 1];
                }
                else if (id.Equals("-appName"))
                {
                    appName = arguments[i + 1];
                }
            }

            var locationPathName = locationOut + "\\" + appName + (buildType == BuildTypeRelease ? ".aab" : ".apk");
            Debug.Log("locationPathName: " + locationPathName);

            if (!string.IsNullOrEmpty(locationPathName))
            {
                var versionConfig = Resources.Load<BuildConfigData>($"CSV/BuildConfig/{typeof(BuildConfigData).Name}");
                PlayerSettings.bundleVersion = buildVersion;
                int.TryParse(bundleVersionCodeStr, out var bundleVersionCode);
                Debug.Log("bundleVersionCodeDebugXXX " + bundleVersionCode);
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
                Debug.Log(" PlayerSettings.Android.bundleVersionCodeDebugXXX " +  PlayerSettings.Android.bundleVersionCode);
                // PlayerSettings.applicationIdentifier = "com.fansipan.stickman.fight.shadow.knights";
                PlayerSettings.productName = appName.Replace("_", " ");
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, versionConfig.packageName);

                Debug.Log("REFRESH AFTER FIX PACKAGE NAME");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);


               
                AssetDatabase.SaveAssets();

                var projectPath = Regex.Replace(Application.dataPath, "/Assets$", string.Empty);
                PlayerSettings.Android.keystoreName = $"{projectPath}/Keystore/key.keystore";
                PlayerSettings.Android.keystorePass = "Stevenjob2015";
                PlayerSettings.Android.keyaliasName = "ew2";
                PlayerSettings.Android.keyaliasPass = "Stevenjob2015";

                var buildTarget = BuildTarget.Android;
                if (buildType == BuildTypeRelease)
                {
                    VerifySymbols(VerifyDefineSymbols, buildTarget);
                    RemoveDefineSymbols(NotReleaseSymbols, buildTarget);
                    AddDefineSymbols(ReleaseSymbols, buildTarget);
                    EditorUserBuildSettings.buildAppBundle = true;
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
                }
                else if (buildType == BuildTypeBeta)
                {
                    AddDefineSymbols(BetaSymbols, buildTarget);
                    EditorUserBuildSettings.buildAppBundle = false;
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                }
                else
                {
                    EditorUserBuildSettings.buildAppBundle = false;
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                }


                BuildResult buildResult = BuildResult.Succeeded;
                var error = BuildPipeline.BuildPlayer(GetScenePaths(), locationPathName, buildTarget,
                    BuildOptions.None);
                Debug.Log("UnityBuildPlayerResult:" + error.summary.result);
                buildResult = error.summary.result;

                if (buildType == BuildTypeRelease)
                {
                    ReverseDefineSymbolsToDefault(ReleaseSymbols, buildTarget, NotReleaseSymbols);
                }
                else if (buildType == BuildTypeBeta)
                {
                    ReverseDefineSymbolsToDefault(BetaSymbols, buildTarget);
                }

                EditorApplication.Exit(buildResult == BuildResult.Succeeded ? 0 : 1);
            }
            else
            {
                EditorApplication.Exit(1);
            }
        }

        static void PerformBuildIOS()
        {
//            PerformLoadIPLoadBalancer();

            var arguments = System.Environment.GetCommandLineArgs();
            var locationPathName = "";
            var buildVersion = "";
            var dataVersion = "";
            var buildType = "";
            var bundleVersionCodeStr = "";
            var storeVersion = "";
            var appName = "";
            for (var i = 0; i < arguments.Length; i++)
            {
                var id = arguments[i];
                if (id.Equals("-out"))
                {
                    locationPathName = arguments[i + 1];
                    Debug.Log("locationPathName: " + locationPathName);
                }
                else if (id.Equals("-finalBuildVersion"))
                {
                    buildVersion = arguments[i + 1];
                    Debug.Log("buildVersion: " + buildVersion);
                }
                else if (id.Equals("-buildType"))
                {
                    buildType = arguments[i + 1];
                }
                else if (id.Equals("-dataVersion"))
                {
                    dataVersion = arguments[i + 1];
                }
                else if (id.Equals("-bundleVersionCode"))
                {
                    bundleVersionCodeStr = arguments[i + 1];
                }
                else if (id.Equals("-storeVersion"))
                {
                    storeVersion = arguments[i + 1];
                }
                else if (id.Equals("-appName"))
                {
                    appName = arguments[i + 1];
                }
            }

            if (!string.IsNullOrEmpty(locationPathName))
            {
                var versionConfig = Resources.Load<BuildConfigData>($"CSV/BuildConfig/{typeof(BuildConfigData).Name}");
                PlayerSettings.bundleVersion = buildVersion;
                int.TryParse(bundleVersionCodeStr, out var bundleVersionCode);
                PlayerSettings.iOS.buildNumber = $"{bundleVersionCode}";

                // PlayerSettings.applicationIdentifier = "com.fansipan.stickman.fight.shadow.knight";
                PlayerSettings.productName = appName.Replace("_", " ");
                // PlayerSettings.applicationIdentifier = versionConfig.PackageName;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, versionConfig.packageName);
                // Debug.Log($"VERSION CONFIG PACKAGE NAME: {versionConfig.PackageName}");
                
               

                //Fix  crash iphone 6s ios 
//#if UNITY_IOS
                //     PBXProject project = new PBXProject();
                //  string targetGuid = project.TargetGuidByName("Unity-iPhone");
                //    var targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
                //    project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                //      project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
//#endif


                var buildTarget = BuildTarget.iOS;
                if (buildType == BuildTypeRelease)
                {
                    VerifySymbols(VerifyDefineSymbols, buildTarget);
                    RemoveDefineSymbols(NotReleaseSymbols, buildTarget);
                    AddDefineSymbols(ReleaseSymbols, buildTarget);
                }
                else if (buildType == BuildTypeBeta)
                {
                    AddDefineSymbols(BetaSymbols, buildTarget);
                }

               

                BuildResult buildResult = BuildResult.Succeeded;
                var error = BuildPipeline.BuildPlayer(GetScenePaths(), locationPathName, buildTarget,
                    BuildOptions.None);

                Debug.Log("UnityBuildPlayerResult:" + error.summary.result);
                buildResult = error.summary.result;

                if (buildType == BuildTypeRelease)
                {
                    ReverseDefineSymbolsToDefault(ReleaseSymbols, buildTarget, NotReleaseSymbols);
                }
                else if (buildType == BuildTypeBeta)
                {
                    ReverseDefineSymbolsToDefault(BetaSymbols, buildTarget);
                }

                EditorApplication.Exit(buildResult == BuildResult.Succeeded ? 0 : 1);
            }
            else
            {
                EditorApplication.Exit(1);
            }
        }

        private const string BuildTypeRelease = "release";
        private const string BuildTypeBeta = "beta";


        private static readonly string[] NotReleaseSymbols = new string[]
        {
            "UF_LOG_ENABLE",
            "DEBUG_LOG_INFO",
            "ENABLE_LOG",
            "ASSERT",
        };

        private static readonly string[] VerifyDefineSymbols = new string[]
        {
            "TRACKING_FIREBASE",
            "TRACKING_APPSFLYER",
        };

        private static readonly string[] ReleaseSymbols = new string[]
        {
            "USE_RELEASE"
        };

        private static readonly string[] BetaSymbols = new string[]
        {
            "USE_BETA"
        };

        private static void AddDefineSymbols(IEnumerable<string> symbols, BuildTarget buildTarget)
        {
            var buildTargetGroup = buildTarget == BuildTarget.Android
                ? BuildTargetGroup.Android
                : BuildTargetGroup.iOS;
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        private static void RemoveDefineSymbols(IEnumerable<string> symbols, BuildTarget buildTarget)
        {
            var buildTargetGroup = buildTarget == BuildTarget.Android
                ? BuildTargetGroup.Android
                : BuildTargetGroup.iOS;
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            foreach (var symbol in symbols)
            {
                allDefines.Remove(symbol);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        private static bool VerifySymbols(IEnumerable<string> symbols, BuildTarget buildTarget)
        {
            var buildTargetGroup = buildTarget == BuildTarget.Android
                ? BuildTargetGroup.Android
                : BuildTargetGroup.iOS;
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            foreach (var symbol in symbols)
            {
                if (!allDefines.Contains(symbol))
                {
                    throw new Exception("NOT HAVE DEFINE " + symbol);
                }
            }

            return true;
        }

        private static void ReverseDefineSymbolsToDefault(IEnumerable<string> symbols, BuildTarget buildTarget,
            IEnumerable<string> symbolsReAdd = null)
        {
            var buildTargetGroup = buildTarget == BuildTarget.Android
                ? BuildTargetGroup.Android
                : BuildTargetGroup.iOS;
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            foreach (var symbol in symbols)
            {
                allDefines.Remove(symbol);
            }

            if (symbolsReAdd != null)
            {
                foreach (var symbol in symbolsReAdd)
                {
                    if (!allDefines.Contains(symbol))
                        allDefines.Add(symbol);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

#if UNITY_IOS
       private static void SetIconIos(Texture2D[] textures)
        {
            var platform = BuildTargetGroup.iOS;

            var kind = UnityEditor.iOS.iOSPlatformIconKind.Application;

            var icons = PlayerSettings.GetPlatformIcons(platform, kind);

            //Assign textures to each available icon slot.
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].SetTexture(textures[0]);

            }

            PlayerSettings.SetPlatformIcons(platform, kind, icons);
        }
#endif

//#if UNITY_ANDROID
//        private static void SetIconAndroid(Texture2D[][] textures)
//        {
//            var platform = BuildTargetGroup.Android;
//
//            var kind = UnityEditor.Android.AndroidPlatformIconKind.Adaptive;
//
//            var icons = PlayerSettings.GetPlatformIcons(platform, kind);
//            //Assign textures to each available icon slot.
//            for (var i = 0; i < icons.Length; i++)
//            {
//                icons[i].SetTextures(textures[0]);
//            }
//
//            PlayerSettings.SetPlatformIcons(platform, kind, icons);
//        }
//
//#endif
    }
}