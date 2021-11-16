using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class Money : ResourceBase
    {
        private enum ActionType
        {
            Add,
            Sub,
        }

        public Money(ResourceType type) : base(type)
        {
            MoneyDict = new Dictionary<int, long>();
        }

        // MoneyType
        public Dictionary<int, long> MoneyDict { get; }

        /**
     * args[0] : MoneyType
     * args[1] : Value
     */
        public override void Add(params object[] args)
        {
            Execute(ActionType.Add, args);
        }

        /**
         * args[0] : MoneyType
         * args[1] : Value
         */
        public override void Sub(params object[] args)
        {
            Execute(ActionType.Sub, args);
        }

        /**
         * args[0] : MoneyType
         */
        public override object Get(params object[] args)
        {
            if (args.Length == 1)
            {
                int moneyType = (int) args[0];
                if (MoneyDict.ContainsKey(moneyType))
                {
                    return MoneyDict[moneyType];
                }

                return (long) 0;
            }

            throw new Exception("args is not valid: " + args.Length);
        }

        public override void Clear()
        {
            MoneyDict.Clear();
        }

        private void Execute(ActionType actionType, params object[] args)
        {
            if (args != null)
            {
                if (args.Length == 2)
                {
                    int moneyType = (int) args[0];
                    long value = (long) args[1];

                    if (value <= 0)
                    {
                        Debug.Log("value is negative " + value);

                        return;
                    }

                    if (MoneyDict.ContainsKey(moneyType))
                    {
                        switch (actionType)
                        {
                            case ActionType.Add:
                                MoneyDict[moneyType] += value;
                                EventManager.EmitEventData(GamePlayEvent.OnAddMoney(Type, moneyType), value);
                                break;
                            case ActionType.Sub:
                                if (MoneyDict[moneyType] > 0)
                                {
                                    MoneyDict[moneyType] -= value;
                                    if (MoneyDict[moneyType] < 0)
                                    {
                                        MoneyDict[moneyType] = 0;
                                    }

                                    EventManager.EmitEventData(GamePlayEvent.OnSubMoney(Type, moneyType), value);
                                }

                                break;
                        }
                    }
                    else
                    {
                        MoneyDict[moneyType] = value;
                    }

                    if (Type == ResourceType.Money)
                        FirebaseLogic.Instance.SetCurrency(moneyType, MoneyDict[moneyType]);

                    EventManager.EmitEventData(GamePlayEvent.OnMoneyChange(Type, moneyType), MoneyDict[moneyType]);
                }
                else
                {
                    throw new Exception("args is not valid: " + args.Length);
                }
            }
        }
    }
}