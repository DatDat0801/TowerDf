namespace EW2
{
    public interface IBuffItem
    {
        IBuffItem ConstructFromUserData(DefensiveBuffUserData userData);
        IBuffItem ConstructBuffStats(BuffBase dataBase);

        IBuffItem ConstructPrice(BuffExchangeDatabase exchangeDatabase);
    }
}