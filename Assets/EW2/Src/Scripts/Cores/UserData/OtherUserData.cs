using System.Collections.Generic;

namespace EW2
{
    public class OtherUserData
    {
        public long timeCountdownNormalSpell;
        public long timeCountdownPremiumSpell;
        public long timeCountdownNormalRune;
        public long timeCountdownPremiumRune;
        public bool isShowMoreInfoHero;
        public List<int> listSpellCanUpgrades = new List<int>();
        public List<int> listHeroEquipedSpell = new List<int>();
        public int totalRune;
        public long timeResetNewDay;
        public bool isSelectedNeetan;
        public OtherUserData()
        {
            UnlockSpellData = new UnlockSpellData();
            UnlockRuneData = new UnlockRuneData();
        }
        public UnlockRuneData UnlockRuneData { get; }
        public UnlockSpellData UnlockSpellData { get; }
        public long GetTimeRemainNormalSpell()
        {
            var timeRemain = timeCountdownNormalSpell - TimeManager.NowInSeconds;
            if (timeRemain <= 0)
                timeRemain = 0;

            return timeRemain;
        }
        public void SetFirstTimeSelectNeetan()
        {
            isSelectedNeetan = true;
            UserData.Instance.Save();
        }
        public void AddSpellCanUpgrade(int spellId)
        {
            if (!listSpellCanUpgrades.Contains(spellId))
                listSpellCanUpgrades.Add(spellId);
        }

        public bool IsSpellCanUpgrade(int spellId)
        {
            return listSpellCanUpgrades.Contains(spellId);
        }

        public void RemoveSpellCanUpgrade(int spellId)
        {
            if (listSpellCanUpgrades.Contains(spellId))
                listSpellCanUpgrades.Remove(spellId);
        }

        public long GetTimeRemainNormalRune()
        {
            var timeRemain = timeCountdownNormalRune - TimeManager.NowInSeconds;
            if (timeRemain <= 0)
                timeRemain = 0;

            return timeRemain;
        }

        public long GetTimeRemainPremiumRune()
        {
            var timeRemain = timeCountdownPremiumRune - TimeManager.NowInSeconds;
            if (timeRemain <= 0)
                timeRemain = 0;

            return timeRemain;
        }

        public long GetTimeRemainPremiumSpell()
        {
            var timeRemain = this.timeCountdownPremiumSpell - TimeManager.NowInSeconds;
            if (timeRemain <= 0)
                timeRemain = 0;

            return timeRemain;
        }

        public bool CheckCanResetNewDay()
        {
            return (timeResetNewDay - TimeManager.NowInSeconds) <= 0;
        }

        public void SetTimeResetNewDay()
        {
            timeResetNewDay = TimeManager.EndTimeOfDaySeconds;
        }
    }
}
