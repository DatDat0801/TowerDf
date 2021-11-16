using System;
using EW2.Spell;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    [Serializable]
    public class FragmentSpellConvertData
    {
        public int id;
        public int ratioConvert;
    }

    public class FragmentSpellConvert : ScriptableObject
    {
        public FragmentSpellConvertData[] fragmentSpellConvertDatas;

        public int GetRatioConvertById(int rarity)
        {
            foreach (var fragmentSpellConvert in fragmentSpellConvertDatas)
            {
                if (fragmentSpellConvert.id == rarity)
                    return fragmentSpellConvert.ratioConvert;
            }

            return 0;
        }
    }

#if UNITY_EDITOR

    public class FragmentSpellConvertPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Spells/";
            string csvDataBase = $"{csvFormat}spell_fragment_convert.csv";
            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvDataBase, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(FragmentSpellConvert) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Inventory/" + nameAsset;
                    FragmentSpellConvert gm = AssetDatabase.LoadAssetAtPath<FragmentSpellConvert>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<FragmentSpellConvert>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvDataBase);

                    gm.fragmentSpellConvertDatas = CsvReader.Deserialize<FragmentSpellConvertData>(data.text);

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}