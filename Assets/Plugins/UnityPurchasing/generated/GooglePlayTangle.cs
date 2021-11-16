#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("uYeUjnQa4sDYJa2g/aTrfOCWm3d30jTh/H9eumYfj8Pnlzw1EZNTCFqAZFA7BHVU/KBPv6+7GCG99mcw07ATzndN0PnrNMVVEiGjCSxIqt5P5CXRbjKaDGvbuCqcCqyKfqjUOij83ztaXODnRq3zIp5mhGYlP9BZnh0THCyeHRYenh0dHLuynFIqFWY5f/I5/qgVPXcmOnNuvYB7dswOrsEvcKwKJmxh0foOYYtTEa22qRTXKaBgrOfzcGSEF7cENbveuAiDHe0snh0+LBEaFTaaVJrrER0dHRkcH+VpF9xsNphYnESXv+o/9Y0AqGkPZVqZSg1Bf7sV94qJRmDG3nKTLCLfGW0JGxXUs0/hDFbHm3oCPXmfdblMqVtcQUCteR4fHRwd");
        private static int[] order = new int[] { 2,13,12,8,6,7,13,7,8,13,12,13,12,13,14 };
        private static int key = 28;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
