using System.Collections.Generic;
using EW2.CampaignInfo.HeroSelect;
using TigerForge;

namespace EW2
{
    public class UserHeroData
    {
        public Dictionary<int, HeroItem> DictHeroes { get; }

        //public Dictionary<int, HeroItem> LockedHeroes { get; }
        public List<int> FirstNotifyHeroIds { get; }

        public List<HeroSelectedData> SelectedHeroes { get; private set; }

        /// <summary>
        /// If user interact with select hero and save, it's true
        /// </summary>
        public bool UserDidSelectHeroAction { get; set; }

        public UserHeroData()
        {
            DictHeroes = new Dictionary<int, HeroItem>();
            FirstNotifyHeroIds = new List<int>();
            SelectedHeroes = new List<HeroSelectedData>();
            if (DictHeroes.Count == 0)
            {
                var freeHeroes = GameContainer.Instance.GetHeroCollection().GetHeroes(true);
                var lockedHeroes = GameContainer.Instance.GetHeroCollection().GetHeroes(false);

                foreach (var hero in lockedHeroes)
                {
                    var heroItem = new HeroItem(hero);

                    if (heroItem.unlock)
                    {
                        DictHeroes.Add(hero, heroItem);
                    }
                }

                foreach (var hero in freeHeroes)
                {
                    var heroItem = new HeroItem(hero);

                    FirstNotifyHeroIds.Add(hero);
                    DictHeroes.Add(hero, heroItem);
                }
            }
        }

        public HeroItem GetHeroById(int heroId)
        {
            if (DictHeroes.ContainsKey(heroId))
            {
                return DictHeroes[heroId];
            }

            return null;
        }


        public bool CheckDisplayFirstNotify(int heroId)
        {
            return FirstNotifyHeroIds.Contains(heroId) || DictHeroes.ContainsKey(heroId);
        }

        public void SeeFirstNotify(int heroId)
        {
            if (!FirstNotifyHeroIds.Contains(heroId))
            {
                FirstNotifyHeroIds.Add(heroId);
            }
        }

        public void AddHero(int heroId, int level)
        {
            if (DictHeroes.ContainsKey(heroId)) return;

            var heroItem = new HeroItem(heroId);

            DictHeroes.Add(heroId, heroItem);

            if (level > 1)
            {
                heroItem.SetSkillPoint(level - 1);
                heroItem.SetLevel(level);
            }

            EventManager.EmitEventData(GamePlayEvent.OnHeroUnlocked, heroId);
            if (level > 1)
            {
                EventManager.EmitEventData(GamePlayEvent.OnHeroLevelUp, heroId);
            }
        }

        public int NumberHeroUnlocked()
        {
            return DictHeroes.Count;
        }

        public bool CheckHeroUnlocked(int heroId)
        {
            return DictHeroes.ContainsKey(heroId);
        }

        /// <summary>
        /// Set default hero data if there is no data is selected, otherwise set as custom
        /// </summary>
        public void SetDefaultSelectedHeroes(List<HeroSelectedData> heroes = null)
        {
            if (SelectedHeroes == null)
            {
                SelectedHeroes = new List<HeroSelectedData>();
            }

            if (heroes != null)
            {
                SelectedHeroes = heroes;
                UserDidSelectHeroAction = true;
            }

            //Auto select if user did not make the action that selecting heroes before
            if (UserDidSelectHeroAction == false)
            {
                if (SelectedHeroes.Count <= 0)
                {
                    int slot = 0;
                    foreach (var heroItem in DictHeroes)
                    {
                        SelectedHeroes.Add(new HeroSelectedData() {
                            level = heroItem.Value.level, slot = slot, heroId = heroItem.Value.heroId
                        });
                        slot++;
                    }
                }
            }

            //Refresh data for Selected hero
            for (var i = 0; i < SelectedHeroes.Count; i++)
            {
                DictHeroes.TryGetValue(SelectedHeroes[i].heroId, out var hero);
                if (hero != null)
                    SelectedHeroes[i].level = hero.level;
            }
        }

        /// <summary>
        /// Selected heroes using in map
        /// </summary>
        /// <returns></returns>
        public List<HeroItem> GetSelectedHeroItems()
        {
            List<HeroItem> heroItems = new List<HeroItem>();
            for (var i = 0; i < SelectedHeroes.Count; i++)
            {
                DictHeroes.TryGetValue(SelectedHeroes[i].heroId, out HeroItem heroItem);
                heroItems.Add(heroItem);
            }

            return heroItems;
        }

        public List<HeroItem> GetListHeroes()
        {
            List<HeroItem> heroItems = new List<HeroItem>();
            foreach (var hero in DictHeroes)
            {
                heroItems.Add(hero.Value);
            }

            return heroItems;
        }
    }
}