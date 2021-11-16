using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class InventoryDataBase
    {
        private ServiceContainer container;

        public InventoryDataBase()
        {
            container = new ServiceContainer();
        }

        public RuneDataBase GetRuneData(int runeIdConvert)
        {
            var runIdCompare = InventoryDataBase.GetRuneId(runeIdConvert);

            var listRuneDatas = GetRuneDatas(runIdCompare.Item1);

            if (listRuneDatas != null)
            {
                foreach (var runeData in listRuneDatas.runeDataBases)
                {
                    if (runeData.rarity == runIdCompare.Item2)
                        return runeData;
                }
            }

            return null;
        }

        private RuneDataBases GetRuneDatas(int runeId)
        {
            var obj = container.Resolve<RuneDataBases>(runeId.ToString());
            if (obj == null)
            {
                obj = LoadRuneFromResource<RuneDataBases>(runeId);

                container.Register(runeId.ToString(), obj);
            }

            return obj;
        }


        public T Get<T>() where T : ScriptableObject
        {
            var obj = container.Resolve<T>();
            if (obj == null)
            {
                obj = LoadFromResource<T>();

                container.Register(obj);
            }

            return obj;
        }

        private T LoadFromResource<T>() where T : ScriptableObject
        {
            return Resources.Load<T>($"CSV/Inventory/{typeof(T).Name}");
        }

        private T LoadRuneFromResource<T>(int runeId) where T : RuneDataBases
        {
            return Resources.Load<T>($"CSV/Inventory/Rune{runeId}DataBases");
        }

        public static int GetRuneIdConvert(int runeId, int rarity)
        {
            return runeId * 100 + rarity;
        }

        public static (int, int) GetRuneId(int runeIdConvert)
        {
            int rarity = runeIdConvert % 100;

            int runeId = (runeIdConvert % 10000) / 100;

            return (runeId, rarity);
        }
    }
}