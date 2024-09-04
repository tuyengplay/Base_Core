#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EranCore
{
    public static class ConfigEarn
    {
        //Android
        public static readonly string productName = "Hello : Hello :a";
        public static readonly string packageName = "Hello.Hello.Hello";
        public static readonly string keyStore = "eranofgame.keystore";
        public static readonly string keyAlias = "user";
        public static readonly string keyPassword = "erannoend123";
        public static readonly string keystorePassword = "erannoend";
        public static readonly string version = "1.0.1";
#if UNITY_EDITOR
        public static readonly AndroidSdkVersions minAPIVersion = AndroidSdkVersions.AndroidApiLevel22;
        public static readonly AndroidSdkVersions targetAPIVersion = AndroidSdkVersions.AndroidApiLevel30;
#endif
        public static readonly string scriptingDefineSymbols = "ODIN_INSPECTOR,ODIN_INSPECTOR_3,ODIN_INSPECTOR_3_1,ADS,IAP,FIREBASE";
        //IOS
    }
}
