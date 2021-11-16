namespace EW2
{
    public class UserEventData
    {
        public UserEventData()
        {
            FirstPurchase = new FirstPurchaseUserData();
            GloryRoadUser = new GloryRoadUserData();
            StarterPackUserData = new StarterPackUserData();
            BuyNowUserData = new BuyNowUserData();
            HeroChallengeUserData = new HeroChallengeUserData();
            HeroAcademyUserData = new HeroAcademyUserData();
            RunePackageUserData = new RunePackageUserData();
            Hero4BundleUserData = new Hero4BundleUserData();
            CrystalFlashSaleUserData = new Hero4ResourceFlashSaleUserData();
            RuneFlashSaleUserData = new RuneFlashSaleUserData();
            SpellFlashSaleUserData = new SpellFlashSaleUserData();
            SpellPackageUserData = new SpellPackageUserData();
            ShopStaminaUserData = new ShopStaminaUserData();
            NewHeroEventUserData = new NewHeroEventUserData();
            CommunityEventUserData = new CommunityEventUserData();
        }

        public CommunityEventUserData CommunityEventUserData { get; }
        public FirstPurchaseUserData FirstPurchase { get; }
        public GloryRoadUserData GloryRoadUser { get; }
        public StarterPackUserData StarterPackUserData { get; }
        public BuyNowUserData BuyNowUserData { get; }
        public HeroChallengeUserData HeroChallengeUserData { get; }

        public HeroAcademyUserData HeroAcademyUserData { get; }
        public RunePackageUserData RunePackageUserData { get; }

        public Hero4BundleUserData Hero4BundleUserData { get; }
        public Hero4ResourceFlashSaleUserData CrystalFlashSaleUserData { get; }
        public RuneFlashSaleUserData RuneFlashSaleUserData { get; }
        public SpellFlashSaleUserData SpellFlashSaleUserData { get; }

        public SpellPackageUserData SpellPackageUserData { get; }

        public ShopStaminaUserData ShopStaminaUserData { get; }
        public bool IsAnyFlashSaleOpen()
        {
            var hero4Resource = CrystalFlashSaleUserData.isOpen;
            var rune = RuneFlashSaleUserData.isOpen;
            var spell = SpellFlashSaleUserData.isOpen;
            return hero4Resource || rune || spell;
        }
        
        public NewHeroEventUserData NewHeroEventUserData { get; }
    }
}
