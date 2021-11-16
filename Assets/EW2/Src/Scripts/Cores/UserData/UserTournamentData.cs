using System.Collections.Generic;
using System.Linq;
using EW2;
using EW2.CampaignInfo.HeroSelect;
using MoreMountains.Tools;
using UnityEngine;
using Zitga.TrackingFirebase;

public class UserTournamentData
{
    public int buffStatId;

    public int buffStatIdPrev;

    public int spellBanId;

    public List<HeroSelectedData> listHeroSelected { get; }

    public List<HeroSelectedData> listHeroBuff { get; }

    public List<int> SelectedTower { get; }

    public int heroNerfId;

    public int nerfStatId;

    public long timeResetNewDay;

    public int remainNumberExchange;

    public long startTime;

    public long endTime;

    public bool showUnlockPopup;

    //public TournamentBuff BuffSeason { get; private set; }
    public TournamentShopUserData ShopUserData { get; private set; }
    public int currRank;
    public int tournamentScore;
    public int currentMapId;
    public bool isClaimedReward;
    public int numberRerollHero;

    public UserTournamentData()
    {
        if (this.listHeroSelected == null)
        {
            this.listHeroSelected = new List<HeroSelectedData>();
        }

        if (this.listHeroBuff == null)
        {
            this.listHeroBuff = new List<HeroSelectedData>();
        }

        ShopUserData = new TournamentShopUserData();

        this.currRank = -1;
    }

    public bool SetHighScore(int newScore)
    {
        if (newScore > this.tournamentScore)
        {
            this.tournamentScore = newScore;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetTournamentSeasonId(long seasonId)
    {
        ShopUserData.seasonId = seasonId;
    }

    public void SetTournamentMapId()
    {
        var mapPool = GameContainer.Instance.GetTournamentData().mapsPool;
        for (var i = 0; i < mapPool.Length; i++)
        {
            if (this.currentMapId == mapPool[i].tournamentMapId)
            {
                if (i == mapPool.Length - 1)
                {
                    this.currentMapId = mapPool[0].tournamentMapId;
                }
                else
                {
                    this.currentMapId = mapPool[i + 1].tournamentMapId;
                }
            }
        }
    }

    public long GetSeasonId()
    {
        return ShopUserData.seasonId;
    }

    public void SetSelectedHeroes(List<HeroSelectedData> list)
    {
        this.listHeroSelected.Clear();
        this.listHeroSelected.AddRange(list);
    }

    public List<int> GetListHeroes()
    {
        var listHeroIds = new List<int>();
        foreach (var heroSelected in this.listHeroSelected)
        {
            listHeroIds.Add(heroSelected.heroId);
        }

        return listHeroIds;
    }

    public void SetListHeroBuff(List<int> heroIds)
    {
        this.listHeroBuff.Clear();
        for (int i = 0; i < heroIds.Count; i++)
        {
            HeroSelectedData item = new HeroSelectedData() {heroId = heroIds[i], level = 1, slot = i};
            this.listHeroBuff.Add(item);
        }
    }

    public int[] GetHeroIdBuff()
    {
        int[] results = new int[2];
        for (int i = 0; i < listHeroBuff.Count; i++)
        {
            results[i] = this.listHeroBuff[i].heroId;
        }

        return results;
    }

    /// <summary>
    /// If selected heroes is not contains all of the buff heroes, then can not buff
    /// </summary>
    /// <returns></returns>
    public bool CanBuffHeroes()
    {
        foreach (HeroSelectedData heroSelectedData in this.listHeroBuff)
        {
            if (!listHeroSelected.Exists(data => data.heroId == heroSelectedData.heroId)) { return false; }
        }

        return true;
    }

    public List<HeroItem> GetSelectedHeroItems()
    {
        List<HeroItem> heroItems = new List<HeroItem>();
        for (var i = 0; i < listHeroSelected.Count; i++)
        {
            UserData.Instance.UserHeroData.DictHeroes.TryGetValue(listHeroSelected[i].heroId,
                out HeroItem heroItem);
            heroItems.Add(heroItem);
        }

        return heroItems;
    }

    public long GetTimeRemainResetNewDay()
    {
        return timeResetNewDay - TimeManager.NowInSeconds;
    }

    private bool CheckCanResetNewDay()
    {
        return (timeResetNewDay - TimeManager.NowInSeconds) <= 0;
    }

    public void SetTimeResetNewDay()
    {
        timeResetNewDay = TimeManager.EndTimeOfDaySeconds;
    }

    public void CheckNewDay()
    {
        if (CheckCanResetNewDay())
        {
            SetTimeResetNewDay();
            HandleResetNewDay();
        }
    }

    private void HandleResetNewDay()
    {
        var dataConfig = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentTicketConfig>();
        if (dataConfig != null)
        {
            var data = dataConfig.tournamentTicketExchanges[0];
            var numberRemain = UserData.Instance.GetMoney(MoneyType.TournamentTicket);
            UserData.Instance.SubMoney(MoneyType.TournamentTicket, numberRemain, AnalyticsConstants.SourceTournament,
                "reset_ticket");
            UserData.Instance.AddMoney(MoneyType.TournamentTicket, data.ticketMax, AnalyticsConstants.SourceTournament,
                "reset_ticket");
            this.remainNumberExchange = data.limitTicketExchange;
        }

        this.numberRerollHero = 0;
    }

    public long GetTimeRemainSeason()
    {
        Debug.Log($"time: {this.endTime - TimeManager.NowInSeconds}");
        return this.endTime - TimeManager.NowInSeconds;
    }
}


public class TournamentShopUserData
{
    public long seasonId;
    public long previousSeasonId;
    public List<ShopTournamentItem> items { get; }
    public List<ShopTournamentItem> previousSeasonItems { get; set; }

    public TournamentShopUserData()
    {
        if (this.items == null)
            items = new List<ShopTournamentItem>();
        if (this.items == null)
            previousSeasonItems = new List<ShopTournamentItem>();
    }

    public bool Purchase(int shopItemId, out ShopTournamentItem remain)
    {
        //this.items.RemoveAll(data => data.shopItemId == shopItemId);
        var index = this.items.FindIndex(tournamentItem => tournamentItem.shopItemId == shopItemId);
        var item = items[index];
        if (item.purchaseAvailable <= 0)
        {
            remain = item;
            return false;
        }
        else
        {
            item.purchaseAvailable--;
            remain = item;
            items[index] = item;
            Debug.Log(items);
            return true;
        }
    }

    public void ResetSeasonData()
    {
        this.items.Clear();
    }

    public ShopTournamentItem GetItemWithId(int shopItemId)
    {
        return this.items.Find(data => data.shopItemId == shopItemId);
    }

    public void CreateNewSeasonItems(ShopTournamentData data, long newSeasonId)
    {
        this.previousSeasonItems = this.items;
        this.previousSeasonId = this.seasonId;
        this.items.Clear();
        for (var i = 0; i < data.items.Length; i++)
        {
            //if own hero?
            if (data.items[i].item.type == ResourceType.Hero)
            {
                var heroItem = GetHeroItem();
                if (heroItem != null)
                {
                    data.items[i].item = heroItem;
                    this.items.Add(data.items[i]);
                }
            }
            else
            {
                this.items.Add(data.items[i]);
            }
        }

        this.seasonId = newSeasonId;
    }

    private Reward GetHeroItem()
    {
        var index = this.previousSeasonItems.FindIndex(item => item.item.type == ResourceType.Hero);
        var heroCollection = GameContainer.Instance.GetHeroCollection();
        var heroes = heroCollection.GetHeroes(false);
        if (index != -1)
        {
            heroes.RemoveAll(i => i == this.previousSeasonItems[index].item.id);
        }

        // for (var i = 0; i < heroes.Count; i++)
        // {
        //     if (UserData.Instance.UserHeroData.CheckHeroUnlocked(heroes[i]))
        //     {
        //         heroes.Remove(heroes[i]);
        //     }
        // }

        heroes.MMShuffle();
        if (heroes.Count > 0)
        {
            return new Reward() {id = heroes[0], number = 1, itemType = -1, type = ResourceType.Hero};
        }
        else
        {
            return null;
        }
    }

    public bool CanResetSeason()
    {
        if (this.previousSeasonId != this.seasonId)
        {
            return true;
        }

        return false;
    }
}
// [Serializable]
// public class TournamentShopItemData
// {
//     public int shopItemId;
//     public int remainingPurchase;
// }