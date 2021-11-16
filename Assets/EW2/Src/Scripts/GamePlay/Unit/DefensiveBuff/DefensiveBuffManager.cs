using System;
using TigerForge;

namespace EW2
{
    public class DefensiveBuffManager : IDisposable
    {
        //private readonly IBuffItem _buffBuilder;

        private readonly DefensiveBuffUserData _userData;
        public BuffHandler buffHandler;

        public DefensiveBuffManager()
        {
            //this._buffBuilder = buffBuilder;
            buffHandler = SetupChain();
            this._userData = new DefensiveBuffUserData();
            EventManager.StartListening(GamePlayEvent.onHeroRevive, RestoreBuff);
        }

        ~DefensiveBuffManager()
        {
            EventManager.StopListening(GamePlayEvent.onHeroRevive, RestoreBuff);
        }

        public void Dispose()
        {
            this._userData.Dispose();
        }

        /// <summary>
        /// Add one buff each time
        /// </summary>
        /// <param name="key"></param>
        public void AddBuffToUserData(string key)
        {
            this._userData.Add(key);
        }

        public BuffItem ConstructBuff(IBuffItem buffBuilder, int buffId)
        {
            var buffBase = GameContainer.Instance.Get<UnitDataBase>().GetDefenseModeBuff(buffId);
            var exchange = GameContainer.Instance.Get<UnitDataBase>().LoadBuffExchange();
            var buffItem = buffBuilder.ConstructBuffStats(buffBase).ConstructFromUserData(this._userData)
                .ConstructPrice(exchange);
            return buffItem as BuffItem;
        }


        public BuffHandler SetupChain()
        {
            var buff7001 = new Buff7001Handler();
            var buff7002 = new Buff7002Handler();
            var buff7003 = new Buff7003Handler();
            var buff7004 = new Buff7004Handler();
            var buff7005 = new Buff7005Handler();
            var buff7006 = new Buff7006Handler();
            buff7001
                .SetNext(buff7002).SetNext(buff7003).SetNext(buff7004)
                .SetNext(buff7005).SetNext(buff7006);
            return buff7001;
        }

        private void RestoreBuff()
        {
            HeroBase hero = EventManager.GetData<HeroBase>(GamePlayEvent.onHeroRevive);
            BuffItem buffitem;
            foreach (var item in _userData.UserBuff)
            {
                if (item.Key == "7003")
                {
                    continue;
                }

                buffitem = ConstructBuff(new BuffItem(), int.Parse(item.Key));
                buffHandler.Buff(buffitem, hero.Id, item.Value);
            }
        }
    }
}