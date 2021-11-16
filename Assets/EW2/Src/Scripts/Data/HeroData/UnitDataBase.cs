using System;
using EW2.Spell;
using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class UnitDataBase
    {
        private ServiceContainer container;

        public UnitDataBase()
        {
            container = new ServiceContainer();
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

        public EnemyData GetEnemyById(int enemyID)
        {
            switch (enemyID)
            {
                case 3001:
                {
                    var obj = container.Resolve<EnemyData3001>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3001>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3002:
                {
                    var obj = container.Resolve<EnemyData3002>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3002>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3003:
                {
                    var obj = container.Resolve<EnemyData3003>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3003>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3004:
                {
                    var obj = container.Resolve<EnemyData3004>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3004>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3005:
                {
                    var obj = container.Resolve<EnemyData3005>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3005>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3006:
                {
                    var obj = container.Resolve<EnemyData3006>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3006>();

                        container.Register(obj);
                    }

                    return obj;
                }
                case 3007:
                {
                    var obj = container.Resolve<EnemyData3007>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3007>();

                        container.Register(obj);
                    }

                    return obj;
                }
                case 3008:
                {
                    var obj = container.Resolve<EnemyData3008>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3008>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3009:
                {
                    var obj = container.Resolve<EnemyData3009>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3009>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3010:
                {
                    var obj = container.Resolve<EnemyData3010>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3010>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3011:
                {
                    var obj = container.Resolve<EnemyData3011>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3011>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3012:
                {
                    var obj = container.Resolve<EnemyData3012>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3012>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3013:
                {
                    var obj = container.Resolve<EnemyData3013>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3013>();

                        container.Register(obj);
                    }

                    return obj;
                }
                case 3014:
                {
                    var obj = container.Resolve<EnemyData3014>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3014>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3015:
                {
                    var obj = container.Resolve<EnemyData3015>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3015>();

                        container.Register(obj);
                    }

                    return obj;
                }
                case 3016:
                {
                    var obj = container.Resolve<EnemyData3016>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3016>();

                        container.Register(obj);
                    }

                    return obj;
                }
                case 3017:
                {
                    var obj = container.Resolve<EnemyData3017>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3017>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3018:
                {
                    var obj = container.Resolve<EnemyData3018>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3018>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3019:
                {
                    var obj = container.Resolve<EnemyData3019>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3019>();

                        container.Register(obj);
                    }

                    return obj;
                }

                case 3020:
                {
                    var obj = container.Resolve<EnemyData3020>();

                    if (obj == null)
                    {
                        obj = LoadFromResource<EnemyData3020>();

                        container.Register(obj);
                    }

                    return obj;
                }
                default:
                    return null;
            }
        }

        public SpellData GetSpellDataById(int idSpell)
        {
            var fileName = $"Spell{idSpell.ToString()}Data";

            var obj = container.Resolve(fileName);

            if (obj == null)
            {
                obj = LoadSpellFromResource(idSpell);

                container.Register(fileName, obj);
            }

            return (SpellData)obj;
        }

        public BuffBase GetDefenseModeBuff(int buffId)
        {
            var fileName = $"Buff{buffId.ToString()}";

            var obj = container.Resolve(fileName);

            if (obj == null)
            {
                obj = LoadBuffFromResource(buffId);

                container.Register(fileName, obj);
            }

            return (BuffBase)obj;
        }

        private T LoadFromResource<T>() where T : ScriptableObject
        {
            return Resources.Load<T>($"CSV/Units/{typeof(T).Name}");
        }

        private SpellData LoadSpellFromResource(int idSpell)
        {
            var fileName = $"Spell{idSpell.ToString()}Data";

            return Resources.Load<SpellData>($"CSV/Units/{fileName}");
        }

        private BuffBase LoadBuffFromResource(int buffId)
        {
            var fileName = $"Buff{buffId.ToString()}";

            return Resources.Load<BuffBase>($"CSV/DefenseModeBuff/{fileName}");
        }

        public BuffExchangeDatabase LoadBuffExchange()
        {
            string fileName = $"CSV/DefenseModeBuff/{nameof(BuffExchangeDatabase)}";
            var obj = container.Resolve(fileName);

            if (obj == null)
            {
                obj = Resources.Load<BuffExchangeDatabase>(fileName);

                container.Register(fileName, obj);
            }

            return (BuffExchangeDatabase)obj;
        }

        public DefensivePointData GetDefensivePointData(int dfpId)
        {
            var dfpNameAsset = $"DefensivePoint{dfpId}Data";

            var obj = container.Resolve<DefensivePointData>(dfpNameAsset);

            if (obj == null)
            {
                obj = Resources.Load<DefensivePointData>($"CSV/Units/{dfpNameAsset}");

                container.Register(dfpNameAsset, obj);
            }

            return obj;
        }
    }
}