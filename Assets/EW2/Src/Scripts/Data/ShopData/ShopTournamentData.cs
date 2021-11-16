using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    public class ShopTournamentData : ScriptableObject
    {
        public ShopTournamentItem[] items;
    }
    [Serializable]
    public struct ShopTournamentItem
    {
        public int shopItemId;
        public Reward item;
        public int purchaseAvailable;
        public int price;

        public Reward[] Generate()
        {
            if (this.item.id == -1)
            {
                return Reward.GenRewards(new[] { this.item });
            }

            return new []{this.item};
        }
    }
#if UNITY_EDITOR

    public class ShopTournamentDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/tournament_shop_items.csv";


            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(ShopTournamentData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;

                    // data orgin
                    ShopTournamentData gm = AssetDatabase.LoadAssetAtPath<ShopTournamentData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<ShopTournamentData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    gm.items = CsvReader.Deserialize<ShopTournamentItem>(data.text);

                    EditorUtility.SetDirty(gm);

                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif
}