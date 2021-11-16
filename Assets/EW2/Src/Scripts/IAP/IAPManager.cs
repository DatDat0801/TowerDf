#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
#define RECEIPT_VALIDATION
#endif

using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using TigerForge;
using UnityEngine;
using Zitga.UIFramework;
using Zitga.TrackingFirebase;
#if UNITY_IAP
using UnityEngine.Purchasing;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

#endif

namespace EW2
{
    public class IapManager : Singleton<IapManager>
#if UNITY_IAP
        , IStoreListener
#endif
    {
#if UNITY_IAP
        private static IStoreController s_storeController; // The Unity Purchasing system.
        private static IExtensionProvider s_storeExtensionProvider; // The store-specific Purchasing subsystems.
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googlePlayStoreExtensions;
        private ITransactionHistoryExtensions _transactionHistoryExtensions;
#endif

        private readonly List<string> _nonConsumableProducts = new List<string>();
        private readonly List<string> _consumableProducts = new List<string>();

        private Action<bool, Reward[]> _callbackPay;
        private Action<bool> _callbackInit;

        private string _productId;

        // private Dictionary<string, string> localPricesString = new Dictionary<string, string>();
        // private Dictionary<string, decimal> localPrice = new Dictionary<string, decimal>();

        private bool _isWaitForInit = false;
        private bool _mPurchaseInProgress;
        private bool _mIsGooglePlayStoreSelected;
        private bool _mIsAppleStoreSelected;
        private bool _isTestIap;
        private bool _isTester;

        #region Init

        private void AddAllProductId()
        {
            foreach (var productIdKey in ProductsManager.ProductsConsumable.Keys)
            {
                this._consumableProducts.Add(productIdKey);
            }

            foreach (var productIdKey in ProductsManager.ProductsNonConsumable.Keys)
            {
                this._nonConsumableProducts.Add(productIdKey);
            }
        }

        public void Init(Action<bool> callback)
        {
            SetIsTestIAP(GameLaunch.isCheat);

            AddAllProductId();

            this._callbackInit = callback;
#if UNITY_IAP
            var module = StandardPurchasingModule.Instance();
            this._mIsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android &&
                                               module.appStore == AppStore.GooglePlay;
            this._mIsAppleStoreSelected = Application.platform == RuntimePlatform.IPhonePlayer &&
                                          module.appStore == AppStore.AppleAppStore;
#endif
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                InvokeCallbackInitIAP(true);
                return;
            }

#if UNITY_IAP
            // If we haven't set up the Unity Purchasing reference
            if (s_storeController == null && !this._isWaitForInit)
            {
                this._isWaitForInit = true;
                // Begin to configure our connection to Purchasing
                try
                {
                    // Create a builder, first passing in a suite of Unity provided stores.
                    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                    //Consumable
                    foreach (var item in this._consumableProducts)
                    {
                        var consumableProductId = item.ToString();
                        builder.AddProduct(consumableProductId, ProductType.Consumable,
                            new IDs() {
                                {consumableProductId, GooglePlay.Name}, {consumableProductId, AppleAppStore.Name},
                            });
                    }

                    foreach (var item in this._nonConsumableProducts)
                    {
                        var nonConsumableProductId = item.ToString();
                        builder.AddProduct(nonConsumableProductId, ProductType.NonConsumable,
                            new IDs() {
                                {nonConsumableProductId, GooglePlay.Name}, {nonConsumableProductId, AppleAppStore.Name},
                            });
                    }

                    // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
                    // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
                    Debug.Log("[IAP] Initialize IAP");
                    UnityPurchasing.Initialize(this, builder);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            else
            {
                InvokeCallbackInitIAP(false);
            }
#else
            InvokeCallbackInitIAP(false);
#endif
        }


        public void SetIsTestIAP(bool value)
        {
            this._isTestIap = value;
        }

        public void SetIsTester(bool value)
        {
            this._isTester = value;
        }

        public bool IsInitialized()
        {
#if UNITY_IAP
            // Only say we are initialized if both the Purchasing references are set.
            return s_storeController != null && s_storeExtensionProvider != null;
#else
            return false;
#endif
        }

        #endregion

        #region Payment actions

        public void Buy(string productID, Action<bool, Reward[]> callback)
        {
            try
            {
                if (this._mPurchaseInProgress == true)
                {
                    Debug.Log("Please wait, purchase in progress");
                    return;
                }

#if UNITY_IAP
                if (s_storeController == null)
                {
                    Debug.LogError("Purchasing is not initialized");
                    return;
                }

                if (s_storeController.products.WithID(productID) == null)
                {
                    Debug.LogError("No product has id " + productID);
                    return;
                }
#endif

                this._callbackPay = callback;
                this._productId = productID;
                this._mPurchaseInProgress = true;
                Debug.Log("[IAP] Purchasing product: " + this._productId);

                if (this._isTestIap)
                {
                    Debug.LogWarning("Cheat IAP");
                    FakeProcessPurchase(productID);
                }
                else
                {
                    if (IsInitialized())
                    {
#if UNITY_IAP
                        Product product = s_storeController.products.WithID(this._productId);

                        if (product != null && product.availableToPurchase)
                        {
                            Debug.Log(string.Format("[IAP] Purchasing product asychronously: '{0}'",
                                product.definition.id));

                            s_storeController.InitiatePurchase(product);
                        }
                        else
                        {
                            Debug.Log(
                                "[IAP] BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

                            if (this._callbackPay != null)
                            {
                                this._callbackPay(false, null);
                            }

                            this._callbackPay = null;
                        }
#else
                        Debug.Log(
                            "[IAP] BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

                        if (this.callbackPay != null)
                        {
                            this.callbackPay(false, null);
                        }

                        this.callbackPay = null;
#endif
                    }
                    else
                    {
                        // Show warrning iap not initialized
                        HandleNotConnectStore();

                        Debug.Log("[IAP] BuyProductID FAIL. Not initialized.");
                        if (this._callbackPay != null)
                        {
                            this._callbackPay(false, null);
                        }

                        this._callbackPay = null;

                        this._mPurchaseInProgress = false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void RestorePurchases()
        {
#if UNITY_IAP
            try
            {
                // If Purchasing has not yet been set up ...
                if (!IsInitialized())
                {
                    // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                    Debug.Log("[IAP] RestorePurchases FAIL. Not initialized.");
                    return;
                }

                if (this._mIsGooglePlayStoreSelected)
                {
                    this._googlePlayStoreExtensions.RestoreTransactions(OnTransactionsRestored);
                }
                else if (this._mIsAppleStoreSelected)
                {
                    this._appleExtensions.RestoreTransactions(OnTransactionsRestored);
                }
                else
                {
                    Debug.LogError("[IAP] RestorePurchases FAIL. Not supported on this platform. Current = " +
                                   Application.platform);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        private void OnTransactionsRestored(bool success)
        {
            Debug.Log("Transactions restored." + success);
        }

        private void InvokeCallbackInitIAP(bool result)
        {
            this._callbackInit?.Invoke(result);
            this._callbackInit = null;
        }

        #endregion

#if UNITY_IAP

        #region Payment event listener

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("[IAP] OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            s_storeController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            s_storeExtensionProvider = extensions;

#if UNITY_IAP
            if (s_storeExtensionProvider != null)
            {
                this._appleExtensions = s_storeExtensionProvider.GetExtension<IAppleExtensions>();
                this._transactionHistoryExtensions =
                    s_storeExtensionProvider.GetExtension<ITransactionHistoryExtensions>();
                this._googlePlayStoreExtensions = s_storeExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();
            }
#endif

            foreach (var item in controller.products.all)
            {
                try
                {
                    if (item.availableToPurchase)
                    {
                        //Log.Info (item.metadata.localizedPriceString);
                        ProductsManager.AddPriceString(item.definition.id, item.metadata.localizedPriceString);
                        ProductsManager.AddPrice(item.definition.id, item.metadata.localizedPrice);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            InvokeCallbackInitIAP(true);
            LogProductDefinitions();
            this._isWaitForInit = false;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("[IAP] Billing failed to initialize!");
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    Debug.LogError("[Buy] Is your App correctly uploaded on the relevant publisher console?");
                    break;

                case InitializationFailureReason.PurchasingUnavailable:
                    // Ask the user if billing is disabled in device settings.
                    Debug.Log("[Buy] Billing disabled!");
                    break;

                case InitializationFailureReason.NoProductsAvailable:
                    // Developer configuration error; check product metadata.
                    Debug.Log("[Buy] No products available for purchase!");
                    break;
            }

            InvokeCallbackInitIAP(false);
            this._isWaitForInit = false;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // Presume valid for platforms with no R.V.
            var isSandboxPurchase = false;
            var validPurchase = true;
            this._mPurchaseInProgress = false;

#if (UNITY_ANDROID || UNITY_IOS) && RECEIPT_VALIDATION
            try
            {
                PurchasingLog log = new PurchasingLog();
                log.LogPurchase(args);
                // Prepare the validator with the secrets we prepared in the Editor obfuscation window.
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                    AppleTangle.Data(), Application.identifier);

                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(args.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("[IAP] Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate.ToShortDateString());
                    Debug.Log(productReceipt.transactionID);

                    if (_mIsGooglePlayStoreSelected)
                    {
                        GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                        if (null != google)
                        {
                            // This is Google's Order ID.
                            // Note that it is null when testing in the sandbox
                            // because Google's sandbox does not provide Order IDs.
                            Debug.Log(google.transactionID);
                            Debug.Log(google.purchaseState);
                            Debug.Log(google.purchaseToken);
                            isSandboxPurchase = false;
                        }
                        else
                        {
                            isSandboxPurchase = true;
                        }
                    }

                    if (_mIsAppleStoreSelected)
                    {
                        AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                        if (null != apple)
                        {
                            Debug.Log(apple.originalTransactionIdentifier);
                            Debug.Log(apple.subscriptionExpirationDate);
                            Debug.Log(apple.cancellationDate);
                            Debug.Log(apple.quantity);
                            isSandboxPurchase = false;
                        }
                        else
                        {
                            isSandboxPurchase = true;
                        }
                    }
                }
            }
            catch (IAPSecurityException e)
            {
                Debug.LogError("[IAP] Invalid receipt, not unlocking content " + e);
                validPurchase = false;
            }
#endif

            try
            {
                if (validPurchase)
                {
                    // Unlock the appropriate content here.
                    this._productId = args.purchasedProduct.definition.id;
                    var title = args.purchasedProduct.metadata.localizedTitle;
                    var transactionId = args.purchasedProduct.transactionID;

                    var shopItemData = ProductsManager.GetItemShopById(this._productId);
                    Reward[] rewards = null;

                    if (!ProductsManager.CheckReceived(shopItemData))
                    {
                        rewards = ProductsManager.GetItemShopById(this._productId).GenRewards();
                        if (shopItemData.consumable <= 0)
                            UserData.Instance.UserShopData.AddProductIdNonconsumePurchased(this._productId);
                        Reward.AddToUserData(rewards, AnalyticsConstants.SourceShop, "", true);

                        try
                        {
                            //tracking
                            UserData.Instance.UserShopData.totalPackageBuyed++;
                            UserData.Instance.UserShopData.totalRevenue += shopItemData.price;
                            UserData.Instance.Save();

                            if (!this._isTester)
                            {
                                Debug.LogWarning("Tracking IAP");
#if !UNITY_EDITOR
                            FirebaseLogic.Instance.SetIapCount(
                                UserData.Instance.UserShopData.totalPackageBuyed.ToString());
                            FirebaseLogic.Instance.SetRevenue(UserData.Instance.UserShopData.totalRevenue.ToString());

                            Product product = args.purchasedProduct;
                            if (Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                AppsflyerUtils.Instance.ValidateAndSendInAppPurchaseIos(product.definition.id,
                                    ((float) product.metadata.localizedPrice).ToString(),
                                    product.metadata.isoCurrencyCode, product.transactionID, null, this);
                            }
                            else
                            {
                                GooglePurchaseData data = new GooglePurchaseData(args.purchasedProduct.receipt);
#if UNITY_ANDROID && TRACKING_APPSFLYER
                                AppsFlyerAndroid.initInAppPurchaseValidatorListener(this);
#endif
                                AppsflyerUtils.Instance.ValidateAndSendInAppPurchaseAndroid(data.inAppDataSignature,
                                    data.inAppPurchaseData, ((float) product.metadata.localizedPrice).ToString(),
                                    product.metadata.isoCurrencyCode, null, this);
                            }
#endif
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                        }
                    }

                    if (this._callbackPay != null)
                    {
                        this._callbackPay(true, rewards);
                    }
                    else
                    {
                        Debug.Log("[IAP] Restore purchase id: " + this._productId);
                    }

                    Debug.Log(string.Format("[IAP] ProcessPurchase: PASS. Product: '{0}'", this._productId));
                    this._callbackPay = null;
                }
                else
                {
                    Debug.Log("[IAP] IAP cheat detected");
                    if (this._callbackPay != null)
                    {
                        this._callbackPay(false, null);
                    }

                    this._callbackPay = null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return PurchaseProcessingResult.Pending;
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            try
            {
                // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
                // this reason with the user to guide their troubleshooting actions.
                Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                    product.definition.storeSpecificId, failureReason));

                // Detailed debugging information
                Debug.Log("Store specific error code: " +
                          this._transactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
                if (this._transactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
                {
                    Debug.Log("Purchase failure description message: " +
                              this._transactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
                }

                if (this._callbackPay != null)
                {
                    this._callbackPay(false, null);
                }

                this._callbackPay = null;
                this._mPurchaseInProgress = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion

#endif
        internal void FakeProcessPurchase(string productId)
        {
            // Unlock the appropriate content here.            
            var shopItemData = ProductsManager.GetItemShopById(productId);
            Reward[] rewards = null;

            if (!ProductsManager.CheckReceived(shopItemData))
            {
                rewards = ProductsManager.GetItemShopById(productId).GenRewards();
                var vipPoint = ProductsManager.GetItemShopById(productId).vipPoint;
                UserData.Instance.AddMoney(MoneyType.VipPoint, vipPoint, AnalyticsConstants.SourceShop, "cheat", true,
                    false);
                if (shopItemData.consumable <= 0)
                    UserData.Instance.UserShopData.AddProductIdNonconsumePurchased(productId);
                Reward.AddToUserData(rewards, AnalyticsConstants.SourceShop, "", false);
            }

            if (this._callbackPay != null)
            {
                this._callbackPay(true, rewards);
            }
            else
            {
                Debug.Log("[IAP] Restore purchase id: " + productId);
            }

            Debug.Log(string.Format("[IAP] ProcessPurchase: PASS. Product: '{0}'", productId));
            this._callbackPay = null;

            this._mPurchaseInProgress = false;
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            //return PurchaseProcessingResult.Complete;
        }

#if UNITY_IAP
        private void LogProductDefinitions()
        {
            var products = s_storeController.products.all;
            foreach (var product in products)
            {
#if UNITY_5_6_OR_NEWER
                Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\nenabled: {3}\n",
                    product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString(),
                    product.definition.enabled ? "enabled" : "disabled"));
#else
            Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\n", product.definition.id,
                product.definition.storeSpecificId, product.definition.type.ToString()));
#endif
            }
        }
#endif

        // private void HandleRestoreSuccess(Reward[] gifts)
        // {
        //     Reward.AddToUserData(gifts,AnalyticsConstants.SourceShop,true);
        //     Ultilities.ShowToastNoti("Restore purchase success!");
        // }

        public void HandleNotConnectStore()
        {
            var title = L.popup.warning_txt;
            var content = L.popup.warning_iap;
            PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(title,
                content, PopupNoticeWindowProperties.PopupType.TwoOption,
                L.button.btn_yes, ReInitIap, L.button.btn_no);

            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
        }

        private void ReInitIap()
        {
            Init((result => {
                if (result)
                {
                    EventManager.EmitEvent(GamePlayEvent.OnReInitIap);
                }
                else
                {
                    HandleNotConnectStore();
                }
            }));
        }
    }

    class GooglePurchaseData
    {
        // INAPP_PURCHASE_DATA
        public string inAppPurchaseData;

        // INAPP_DATA_SIGNATURE
        public string inAppDataSignature;

        [System.Serializable]
        private struct GooglePurchaseReceipt
        {
            public string Payload;
        }

        [System.Serializable]
        private struct GooglePurchasePayload
        {
            public string json;
            public string signature;
        }

        public GooglePurchaseData(string receipt)
        {
            try
            {
                var purchaseReceipt = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
                var purchasePayload = JsonUtility.FromJson<GooglePurchasePayload>(purchaseReceipt.Payload);
                inAppPurchaseData = purchasePayload.json;
                inAppDataSignature = purchasePayload.signature;
            }
            catch
            {
                Debug.Log("Could not parse receipt: " + receipt);
                inAppPurchaseData = "";
                inAppDataSignature = "";
            }
        }
    }
}