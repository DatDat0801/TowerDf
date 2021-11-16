using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EW2
{
#if UNITY_EDITOR
    public class UnityIapEditor
    {
        const string DEFINE_UNITY_IAP = "UNITY_IAP";

        [MenuItem("Tools/Unity IAP Config/Enable")]
        public static void ConfigDefineForUnityIAP()
        {
            BuildTargetGroup buildTarget = BuildTargetGroup.Standalone;
#if UNITY_ANDROID
            buildTarget = BuildTargetGroup.Android;
#elif UNITY_IOS
                buildTarget = BuildTargetGroup.iOS;
#endif

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);

            if (!defines.Contains(DEFINE_UNITY_IAP))
            {
                if (defines.Length <= 0)
                {
                    defines = DEFINE_UNITY_IAP;
                }
                else
                {
                    defines = defines + ";" + DEFINE_UNITY_IAP;
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, defines);
            }
        }

        [MenuItem("Tools/Unity IAP Config/Disable")]
        public static void DisableUnityIAP()
        {
            BuildTargetGroup buildTarget = BuildTargetGroup.Standalone;
#if UNITY_ANDROID
            buildTarget = BuildTargetGroup.Android;
#elif UNITY_IOS
            buildTarget = BuildTargetGroup.iOS;
#endif

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
            StringBuilder newDefines = new StringBuilder();

            if (defines.Contains(DEFINE_UNITY_IAP))
            {
                string[] arrDefine = defines.Split(';');
                if (arrDefine != null)
                {
                    if (arrDefine.Length == 1)
                    {
                        defines = string.Empty;
                    }
                    else
                    {
                        foreach (var item in arrDefine)
                        {
                            if (item.Equals(DEFINE_UNITY_IAP))
                            {
                                continue;
                            }

                            newDefines.Append(item).Append(";");
                        }

                        defines = newDefines.ToString();
                    }
                }


                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, defines);
            }
        }
    }
#endif
}