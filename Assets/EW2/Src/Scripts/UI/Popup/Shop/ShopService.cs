using System;
using System.Collections;
using Invoke;
using SocialTD2;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class ShopService : Singleton<ShopService>
    {
        /// <summary>
        /// product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="callback"></param>
        public void BuyPack(ShopItemData data, Action<bool, Reward[]> callback)
        {
            Buy(data.productId, (result, gifts) =>
            {
                if (callback != null)
                {
                    callback(result, gifts);
                }
            });
        }

        /// <summary>
        /// product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="callback"></param>
        public void BuyLimitedBundle(ShopLitmitedItemData data, Action<bool, Reward[]> callback)
        {
            Buy(data.productId, (result, gifts) =>
            {
                if (callback != null)
                {
                    callback(result, gifts);
                }
            });
        }

        /// <summary>
        /// product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="callback"></param>
        public void BuyGem(ShopItemData data, Action<bool, Reward[]> callback)
        {
            Buy(data.productId, (result, gifts) =>
            {
                if (callback != null)
                {
                    callback(result, gifts);
                }
            });
        }

        /// <summary>
        /// product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="callback"></param>
        public void BuyHero(ShopItemData data, Action<bool, Reward[]> callback)
        {
            Buy(data.productId, (result, gifts) =>
            {
                if (callback != null)
                {
                    callback(result, gifts);
                }
            });
        }

        /// <summary>
        /// buy product ID
        /// </summary>
        private void Buy(string productId, Action<bool, Reward[]> callback)
        {
            //tracking
            FirebaseLogic.Instance.PurchaseClick(productId);

            IapManager.Instance.Buy(productId, (success, rewards) =>
            {
                if (success)
                {
                    callback?.Invoke(success, rewards);
                    StartCoroutine(HandlePurchaseSuccess());

                    UserData.Instance.SetFirstPurchase(HandleOnFirstPurchase);
                }
                else
                {
                    HandlePurchaseFail();
                }
            });
        }

        private void HandleOnFirstPurchase()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.first_purchase_popup);
        }

        private void HandlePurchaseFail()
        {
            Ultilities.ShowToastNoti(L.popup.purchase_failed);
        }

        private IEnumerator HandlePurchaseSuccess()
        {
            EventManager.EmitEvent(GamePlayEvent.OnIAP);
            
            yield return new WaitForSecondsRealtime(1.2f);

            var title = L.popup.warning_txt;
            var content = L.popup.purchase_save_notice;
            var platform = "";

#if UNITY_ANDROID && !UNITY_EDITOR
            platform = L.common.google_title;
#elif UNITY_IOS && !UNITY_EDITOR
            platform = L.common.apple_title;
#endif

            content = string.Format(content, platform);

            if (!LoadSaveUtilities.IsAuthenticated())
            {
                PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(title,
                    content, PopupNoticeWindowProperties.PopupType.TwoOption_OkPriority,
                    L.button.btn_connect, CloudSaveClick, L.button.later_name);

                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
            }
            else
            {
                UserData.Instance.Save();
                LoadSaveUtilities.AutoSave(false);
            }
        }

        #region Login acc

        //login
        private void CloudSaveClick()
        {
            TDLoginGooglePlayGame loginGooglePlayGame = new TDLoginGooglePlayGame();
            if (LoadSaveUtilities.IsAuthenticated())
            {
                Ultilities.ShowToastNoti("Logged in");
            }
            else
            {
                OnLink();
            }


            void OnLink()
            {
#if UNITY_EDITOR
                OnLoginSuccess();
                return;
#endif
                loginGooglePlayGame.Login(b =>
                {
                    if (b)
                    {
                        OnLoginSuccess();
                    }
                    else
                    {
                        Ultilities.ShowToastNoti(L.popup.linking_failed);
                    }
                });
            }
        }

        void OnLoginSuccess()
        {
            Ultilities.ShowToastNoti(L.popup.login_successful_txt);
            var settingData = UserData.Instance.AccountData;
            //Save token generated by GPGS
            settingData.tokenId = LoadSaveUtilities.GetUserID();
            settingData.userId = LoadSaveUtilities.GetUserID();
            PlayerPrefs.SetString("token", settingData.userId);
            UserData.Instance.Save();
            LoadSaveUtilities.AutoSave(false);
            EventManager.EmitEvent(GamePlayEvent.OnLoginSuccess);
        }

        #endregion
    }
}