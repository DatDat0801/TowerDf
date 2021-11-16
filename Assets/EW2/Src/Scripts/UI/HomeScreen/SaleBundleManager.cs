using TigerForge;
using UnityEngine;

namespace EW2
{
    public class SaleBundleManager : MonoBehaviour
    {
        [SerializeField] private SalebundleButtonUi starterPack;

        [SerializeField] private SalebundleButtonUi buyNowPack;

        [SerializeField] private SalebundleButtonUi heroAcademy;

        [SerializeField] private SalebundleButtonUi runePackage;

        [SerializeField] private SalebundleButtonUi hero4Bundle;

        [SerializeField] private SalebundleButtonUi hero4ResourceFlashSale;

        [SerializeField] private SalebundleButtonUi runeFlashSale;
        
        [SerializeField] private SalebundleButtonUi spellFlashSale;
        
        [SerializeField] private SalebundleButtonUi spellPackageSale;
        
        [SerializeField] private SalebundleButtonUi newHeroEvent;
        
        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnUpdateSaleBundle, CheckStatusBundle);
        }

        private void OnEnable()
        {
            CheckStatusBundle();
        }

        private void CheckStatusBundle()
        {
            CheckStatusStarterPack();

            CheckStatusBuyNowPack();

            CheckStatusHeroAcademy();

            CheckStatusRunePackage();

            CheckStatusHero4Bundle();

            UpdateHero4ResourceFlashSaleState();

            UpdateRuneFlashSaleState();
            
            UpdateSpellFlashSaleState();
            
            UpdateSpellPackageSaleState();

            UpdateNewHeroState();
        }
        private void UpdateSpellFlashSaleState()
        {
            if (this.spellFlashSale)
            {
                if (UserData.Instance.UserEventData.SpellFlashSaleUserData.CheckCanShow())
                {
                    spellFlashSale.gameObject.SetActive(true);
                }
                else
                {
                    spellFlashSale.gameObject.SetActive(false);
                }
            }
        }
        private void UpdateRuneFlashSaleState()
        {
            if (this.runeFlashSale)
            {
                if (UserData.Instance.UserEventData.RuneFlashSaleUserData.CheckCanShow())
                {
                    runeFlashSale.gameObject.SetActive(true);
                }
                else
                {
                    runeFlashSale.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateSpellPackageSaleState()
        {
            if (this.spellPackageSale)
            {
                if (UserData.Instance.UserEventData.SpellPackageUserData.CheckCanShow())
                {
                    spellPackageSale.gameObject.SetActive(true);
                }
                else
                {
                    spellPackageSale.gameObject.SetActive(false);
                }
            }
        }
        private void UpdateHero4ResourceFlashSaleState()
        {
            if (hero4ResourceFlashSale)
            {
                //For both hero5 and hero4
                if (UserData.Instance.UserEventData.CrystalFlashSaleUserData.CheckCanShow() || UserData.Instance.UserEventData.CrystalFlashSaleUserData.CheckCanShowFor1005())
                {
                    hero4ResourceFlashSale.gameObject.SetActive(true);
                }
                else
                {
                    hero4ResourceFlashSale.gameObject.SetActive(false);
                }
            }
        }
        private void CheckStatusHero4Bundle()
        {
            if (hero4Bundle)
            {
                if (UserData.Instance.UserEventData.Hero4BundleUserData.CheckCanShow())
                {
                    hero4Bundle.gameObject.SetActive(true);
                }
                else
                {
                    hero4Bundle.gameObject.SetActive(false);
                }
            }
        }
        private void CheckStatusRunePackage()
        {
            if (runePackage)
            {
                runePackage.gameObject.SetActive(false);
                if (UserData.Instance.UserEventData.RunePackageUserData.CheckCanShow())
                {
                    runePackage.gameObject.SetActive(true);
                }
                else
                {
                    runePackage.gameObject.SetActive(false);
                }
            }
        }
        private void CheckStatusStarterPack()
        {
            if (starterPack)
            {
                starterPack.gameObject.SetActive(false);
                // if (UserData.Instance.UserEventData.StarterPackUserData.CheckCanShow())
                // {
                //     starterPack.gameObject.SetActive(true);
                // }
                // else
                // {
                //     starterPack.gameObject.SetActive(false);
                // }
            }
        }

        private void CheckStatusBuyNowPack()
        {
            if (buyNowPack)
            {
                if (UserData.Instance.UserEventData.BuyNowUserData.CheckCanShow())
                {
                    buyNowPack.gameObject.SetActive(true);
                }
                else
                {
                    buyNowPack.gameObject.SetActive(false);
                }
            }
        }

        private void CheckStatusHeroAcademy()
        {
            if (heroAcademy)
            {
                if (UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow())
                {
                    heroAcademy.gameObject.SetActive(true);
                }
                else
                {
                    heroAcademy.gameObject.SetActive(false);
                }
            }
        }
        
        private void UpdateNewHeroState()
        {
            if (this.newHeroEvent)
            {
                if (UserData.Instance.UserEventData.NewHeroEventUserData.CheckCanShow())
                {
                    newHeroEvent.gameObject.SetActive(true);
                }
                else
                {
                    newHeroEvent.gameObject.SetActive(false);
                }
            }
        }
    }
}
