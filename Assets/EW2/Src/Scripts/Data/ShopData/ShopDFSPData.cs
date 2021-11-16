using System;
using UnityEditor;
using UnityEngine;
using Zitga.CsvTools;

namespace EW2
{
    /// <summary>
    /// Defensive point data
    /// </summary>
    [Serializable]
    public class ShopDFSPItemData
    {
        public int defensivePointId;
        public int price;
        public int moneyType;

    }
    public class ShopDFSPData : ScriptableObject
    {
        public ShopDFSPItemData[] exchangePrices;

        public ShopDFSPItemData GetShopDFSPItemData(int dfspId)
        {
            var item = Array.Find(this.exchangePrices, data => data.defensivePointId == dfspId);
            if (item != null)
            {
                return item;
            }

            throw new ArgumentNullException(nameof(item));
        }
    }
    #if UNITY_EDITOR

    public class ShopBuffDataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            string csvFormat = "Assets/EW2/CSV/Shops/shop_dfsp_exchange_price.csv";


            foreach (string str in importedAssets)
            {
                if (str.IndexOf(csvFormat, StringComparison.Ordinal) != -1 &&
                    str.IndexOf(".csv", StringComparison.Ordinal) != -1)
                {
                    // get asset file
                    string nameAsset = nameof(ShopDFSPData) + ".asset";
                    string assetFile = "Assets/EW2/Resources/CSV/Shops/" + nameAsset;

                    // data orgin
                    ShopDFSPData gm = AssetDatabase.LoadAssetAtPath<ShopDFSPData>(assetFile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<ShopDFSPData>();
                        AssetDatabase.CreateAsset(gm, assetFile);
                    }

                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(csvFormat);
                    gm.exchangePrices = CsvReader.Deserialize<ShopDFSPItemData>(data.text);

                    EditorUtility.SetDirty(gm);

                    Debug.Log("Reimport Asset: " + str);
                }
            }
        }
    }
#endif

}