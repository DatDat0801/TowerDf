using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ChangeFontByLanguage
    {
        public static void ChangeFont(GameObject root)
        {
            var currentLanguage = UserData.Instance.SettingData.userLanguage;

            var notosansFont = ResourceUtils.GetFont("NotoSans-Bold");
            foreach (var text in root.GetComponentsInChildren<Text>(true))
            {
                if (text.font != null)
                {
#if UNITY_IOS
                    if (currentLanguage == SystemLanguage.Thai)
                    {
                        var thaiFont = ResourceUtils.GetFont("IBMPlexSansThai-Medium");
                        text.font = thaiFont;
                        return;
                    }
#endif
                    if (currentLanguage == SystemLanguage.Vietnamese)
                    {
                        //Debug.LogAssertion(text.font.name);
                        if (text.font.name == "Overpass-ExtraBold")
                        {
                            text.font = notosansFont;
                        }
                        else if (text.font.name == "Seagram tfb")
                        {
                            var lobster = ResourceUtils.GetFont("Lobster-Regular");
                            text.font = lobster;
                        }
                    }
                    else
                    {
                        if (text.font.name == "Lobster-Regular")
                        {
                            var lobster = ResourceUtils.GetFont("Seagram tfb");
                            text.font = lobster;
                        }
                    }
                }
            }
        }
    }
}