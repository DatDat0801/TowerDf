using UnityEngine;
using UnityEngine.Events;
using ZitgaGiftCode;

namespace EW2.Social.GiftCode
{
    public class ZitgaGiftCode
    {
        public event UnityAction<int, string> onCheckGiftCode;
        public event UnityAction<int, string> onClaimGiftCode;
        
        private const string HOST = "34.87.170.169";

        private const int PORT = 8800;

        private const int API_VERSION = 2;

        private const int GAME_ID = 6;

        private const string SECRET_KEY = "o7oj69w09v2r1bewpa0x5gzqjtfsvf2c";

        private readonly GiftCodeService giftCodeService;
        
        public ZitgaGiftCode()
        {
            GiftCodeOption option = new GiftCodeOption(HOST, PORT);

            option.ApiVersion = API_VERSION;
            option.GameId = GAME_ID;
            option.SecretKey = SECRET_KEY;

            option.CheckGiftCodeCallback = OnCheckGiftCode;
            option.ClaimGiftCodeCallback = OnClaimGiftCode;

            giftCodeService = new GiftCodeService(option);
        }

        #region Check

        public void CheckGiftCode(AuthProvider provider, string uniqueIdentifier, string giftcode)
        {
            GiftCodeOutbound outbound =
                new GiftCodeOutbound(provider, uniqueIdentifier);
            outbound.Code = giftcode;

            Debug.Log("Check gift code sent");
            giftCodeService.CheckGiftCode(outbound);
        }

        public void OnCheckGiftCode(int logicCode, string giftCodeData)
        {
            Debug.Log("OnCheckGiftCode: logicCode=" + logicCode);
            Debug.Log("OnCheckGiftCode: data=" + giftCodeData);
            onCheckGiftCode?.Invoke(logicCode, giftCodeData);
        }

        #endregion

        #region DO

        public void ClaimGiftCode(AuthProvider provider, string uniqueIdentifier, string giftcode)
        {
            GiftCodeOutbound outbound = new GiftCodeOutbound(provider, uniqueIdentifier);
            outbound.Code = giftcode;

            Debug.Log("Claim gift code sent");
            giftCodeService.ClaimGiftCode(outbound);
        }

        public void OnClaimGiftCode(int logicCode, string giftCodeData)
        {
            Debug.Log("OnClaimGiftCode: logicCode=" + logicCode);
            Debug.Log("OnClaimGiftCode: data=" + giftCodeData);
            onClaimGiftCode?.Invoke(logicCode, giftCodeData);
        }

        #endregion

        public static AuthProvider GetProvider()
        {
            var currentPlatform = Application.platform;

            switch (currentPlatform)
            {
                case RuntimePlatform.Android:
                    return AuthProvider.ANDROID_DEVICE;
                case RuntimePlatform.IPhonePlayer:
                    return AuthProvider.IOS_DEVICE;
                default:
                    return AuthProvider.WINDOWS_DEVICE;
            }
        }
    }
}