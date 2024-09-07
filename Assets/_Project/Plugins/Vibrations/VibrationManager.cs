using UnityEngine;
using System;
using EranCore.Audio;


#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace GameCore.Vibrations
{
    public enum HapticTypes { Selection, Success, Warning, Failure, LightImpact, MediumImpact, HeavyImpact, None }

    public static class VibrationManager
    {
        public static long LightDuration = 20;
        public static long MediumDuration = 40;
        public static long HeavyDuration = 80;
        public static int LightAmplitude = 40;
        public static int MediumAmplitude = 120;
        public static int HeavyAmplitude = 255;
        private static int _sdkVersion = -1;
        private static long[] _lightimpactPattern = { 0, LightDuration };
        private static int[] _lightimpactPatternAmplitude = { 0, LightAmplitude };
        private static long[] _mediumimpactPattern = { 0, MediumDuration };
        private static int[] _mediumimpactPatternAmplitude = { 0, MediumAmplitude };
        private static long[] _HeavyimpactPattern = { 0, HeavyDuration };
        private static int[] _HeavyimpactPatternAmplitude = { 0, HeavyAmplitude };
        private static long[] _successPattern = { 0, LightDuration, LightDuration, HeavyDuration };
        private static int[] _successPatternAmplitude = { 0, LightAmplitude, 0, HeavyAmplitude };
        private static long[] _warningPattern = { 0, HeavyDuration, LightDuration, MediumDuration };
        private static int[] _warningPatternAmplitude = { 0, HeavyAmplitude, 0, MediumAmplitude };
        private static long[] _failurePattern = { 0, MediumDuration, LightDuration, MediumDuration, LightDuration, HeavyDuration, LightDuration, LightDuration };
        private static int[] _failurePatternAmplitude = { 0, MediumAmplitude, 0, MediumAmplitude, 0, HeavyAmplitude, 0, LightAmplitude };


        public static bool Android()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
				return true;
#else
            return false;
#endif
        }

        public static bool iOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
				return true;
#else
            return false;
#endif
        }

        public static void Vibrate()
        {
            if (AudioManager.Instance.IsOnVibrate)
            {
                if (Android())
                {
                    AndroidVibrate(MediumDuration);
                }
                else if (iOS())
                {
                    iOSTriggerHaptics(HapticTypes.MediumImpact);
                }
            }
        }

        public static void Haptic(HapticTypes _type, bool _defaultToRegularVibrate = false)
        {
            if (!AudioManager.Instance.IsOnVibrate)
            {
                return;
            }
            if (Android())
            {
                switch (_type)
                {
                    case HapticTypes.None:
                        // do nothing
                        break;
                    case HapticTypes.Selection:
                        AndroidVibrate(LightDuration, LightAmplitude);
                        break;

                    case HapticTypes.Success:
                        AndroidVibrate(_successPattern, _successPatternAmplitude, -1);
                        break;

                    case HapticTypes.Warning:
                        AndroidVibrate(_warningPattern, _warningPatternAmplitude, -1);
                        break;

                    case HapticTypes.Failure:
                        AndroidVibrate(_failurePattern, _failurePatternAmplitude, -1);
                        break;

                    case HapticTypes.LightImpact:
                        AndroidVibrate(_lightimpactPattern, _lightimpactPatternAmplitude, -1);
                        break;

                    case HapticTypes.MediumImpact:
                        AndroidVibrate(_mediumimpactPattern, _mediumimpactPatternAmplitude, -1);
                        break;

                    case HapticTypes.HeavyImpact:
                        AndroidVibrate(_HeavyimpactPattern, _HeavyimpactPatternAmplitude, -1);
                        break;
                }
            }
            else if (iOS())
            {
                iOSTriggerHaptics(_type, _defaultToRegularVibrate);
            }
        }


#if UNITY_ANDROID && !UNITY_EDITOR
			private static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			private static AndroidJavaObject CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			private static AndroidJavaObject AndroidVibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
			private static AndroidJavaClass VibrationEffectClass;
			private static AndroidJavaObject VibrationEffect;
			private static int DefaultAmplitude;
            private static IntPtr AndroidVibrateMethodRawClass = AndroidJNIHelper.GetMethodID(AndroidVibrator.GetRawClass(), "vibrate", "(J)V", false);
            private static jvalue[] AndroidVibrateMethodRawClassParameters = new jvalue[1];
#else
        private static AndroidJavaClass UnityPlayer;
        private static AndroidJavaObject CurrentActivity;
        private static AndroidJavaObject AndroidVibrator = null;
        private static AndroidJavaClass VibrationEffectClass = null;
        private static AndroidJavaObject VibrationEffect;
        private static int DefaultAmplitude;
        private static IntPtr AndroidVibrateMethodRawClass = IntPtr.Zero;
        private static jvalue[] AndroidVibrateMethodRawClassParameters = null;
#endif

        public static void AndroidVibrate(long _milliseconds)
        {

            if (!Android() || !AudioManager.Instance.IsOnVibrate) { return; }
            AndroidVibrateMethodRawClassParameters[0].j = _milliseconds;
            AndroidJNI.CallVoidMethod(AndroidVibrator.GetRawObject(), AndroidVibrateMethodRawClass, AndroidVibrateMethodRawClassParameters);
        }

        public static void AndroidVibrate(long _milliseconds, int _amplitude)
        {
            if (!Android() || !AudioManager.Instance.IsOnVibrate) { return; }
            if ((AndroidSDKVersion() < 26))
            {
                AndroidVibrate(_milliseconds);
            }
            else
            {
                VibrationEffectClassInitialization();
                VibrationEffect = VibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", new object[] { _milliseconds, _amplitude });
                AndroidVibrator.Call("vibrate", VibrationEffect);
            }
        }

        public static void AndroidVibrate(long[] _pattern, int _repeat)
        {
            if (!Android() || !AudioManager.Instance.IsOnVibrate) { return; }
            if ((AndroidSDKVersion() < 26))
            {
                AndroidVibrator.Call("vibrate", _pattern, _repeat);
            }
            else
            {
                VibrationEffectClassInitialization();
                VibrationEffect = VibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", new object[] { _pattern, _repeat });
                AndroidVibrator.Call("vibrate", VibrationEffect);
            }
        }

        public static void AndroidVibrate(long[] _pattern, int[] _amplitudes, int _repeat)
        {
            if (!Android() || !AudioManager.Instance.IsOnVibrate) { return; }
            if ((AndroidSDKVersion() < 26))
            {
                AndroidVibrator.Call("vibrate", _pattern, _repeat);
            }
            else
            {
                VibrationEffectClassInitialization();
                VibrationEffect = VibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", new object[] { _pattern, _amplitudes, _repeat });
                AndroidVibrator.Call("vibrate", VibrationEffect);
            }
        }

        public static void AndroidCancelVibrations()
        {
            if (!Android() || !AudioManager.Instance.IsOnVibrate) { return; }
            AndroidVibrator.Call("cancel");
        }

        private static void VibrationEffectClassInitialization()
        {
            if (VibrationEffectClass == null)
            {
                VibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            }
        }

        public static int AndroidSDKVersion()
        {
            if (_sdkVersion == -1)
            {
                int apiLevel = int.Parse(SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.IndexOf("-") + 1, 3));
                _sdkVersion = apiLevel;
                return apiLevel;
            }
            else
            {
                return _sdkVersion;
            }
        }


#if UNITY_IOS && !UNITY_EDITOR
			[DllImport ("__Internal")]
			private static extern void InstantiateFeedbackGenerators();
			[DllImport ("__Internal")]
			private static extern void ReleaseFeedbackGenerators();
			[DllImport ("__Internal")]
			private static extern void SelectionHaptic();
			[DllImport ("__Internal")]
			private static extern void SuccessHaptic();
			[DllImport ("__Internal")]
			private static extern void WarningHaptic();
			[DllImport ("__Internal")]
			private static extern void FailureHaptic();
			[DllImport ("__Internal")]
			private static extern void LightImpactHaptic();
			[DllImport ("__Internal")]
			private static extern void MediumImpactHaptic();
			[DllImport ("__Internal")]
			private static extern void HeavyImpactHaptic();
#else
        private static void InstantiateFeedbackGenerators() { }
        private static void ReleaseFeedbackGenerators() { }
        private static void SelectionHaptic() { }
        private static void SuccessHaptic() { }
        private static void WarningHaptic() { }
        private static void FailureHaptic() { }
        private static void LightImpactHaptic() { }
        private static void MediumImpactHaptic() { }
        private static void HeavyImpactHaptic() { }
#endif
        private static bool iOSHapticsInitialized = false;

        public static void iOSInitializeHaptics()
        {
            if (!iOS()) { return; }
            InstantiateFeedbackGenerators();
            iOSHapticsInitialized = true;
        }

        public static void iOSReleaseHaptics()
        {
            if (!iOS()) { return; }
            ReleaseFeedbackGenerators();
        }

        public static bool HapticsSupported()
        {
            bool hapticsSupported = false;
#if UNITY_IOS
            DeviceGeneration generation = Device.generation;
            if ((generation == DeviceGeneration.iPhone3G)
            || (generation == DeviceGeneration.iPhone3GS)
            || (generation == DeviceGeneration.iPodTouch1Gen)
            || (generation == DeviceGeneration.iPodTouch2Gen)
            || (generation == DeviceGeneration.iPodTouch3Gen)
            || (generation == DeviceGeneration.iPodTouch4Gen)
            || (generation == DeviceGeneration.iPhone4)
            || (generation == DeviceGeneration.iPhone4S)
            || (generation == DeviceGeneration.iPhone5)
            || (generation == DeviceGeneration.iPhone5C)
            || (generation == DeviceGeneration.iPhone5S)
            || (generation == DeviceGeneration.iPhone6)
            || (generation == DeviceGeneration.iPhone6Plus)
            || (generation == DeviceGeneration.iPhone6S)
            || (generation == DeviceGeneration.iPhone6SPlus)
            || (generation == DeviceGeneration.iPhoneSE1Gen)
            || (generation == DeviceGeneration.iPad1Gen)
            || (generation == DeviceGeneration.iPad2Gen)
            || (generation == DeviceGeneration.iPad3Gen)
            || (generation == DeviceGeneration.iPad4Gen)
            || (generation == DeviceGeneration.iPad5Gen)
            || (generation == DeviceGeneration.iPadAir1)
            || (generation == DeviceGeneration.iPadAir2)
            || (generation == DeviceGeneration.iPadMini1Gen)
            || (generation == DeviceGeneration.iPadMini2Gen)
            || (generation == DeviceGeneration.iPadMini3Gen)
            || (generation == DeviceGeneration.iPadMini4Gen)
            || (generation == DeviceGeneration.iPadPro10Inch1Gen)
            || (generation == DeviceGeneration.iPadPro10Inch2Gen)
            || (generation == DeviceGeneration.iPadPro11Inch)
            || (generation == DeviceGeneration.iPadPro1Gen)
            || (generation == DeviceGeneration.iPadPro2Gen)
            || (generation == DeviceGeneration.iPadPro3Gen)
            || (generation == DeviceGeneration.iPadUnknown)
            || (generation == DeviceGeneration.iPodTouch1Gen)
            || (generation == DeviceGeneration.iPodTouch2Gen)
            || (generation == DeviceGeneration.iPodTouch3Gen)
            || (generation == DeviceGeneration.iPodTouch4Gen)
            || (generation == DeviceGeneration.iPodTouch5Gen)
            || (generation == DeviceGeneration.iPodTouch6Gen)
            || (generation == DeviceGeneration.iPhone6SPlus))
            {
                hapticsSupported = false;
            }
            else
            {
                hapticsSupported = true;
            }
#endif

            return hapticsSupported;


        }

        private static void iOSTriggerHaptics(HapticTypes _type, bool _defaultToRegularVibrate = false)
        {
            if (!iOS()) { return; }

            if (!iOSHapticsInitialized)
            {
                iOSInitializeHaptics();
            }

            if (HapticsSupported())
            {
                switch (_type)
                {
                    case HapticTypes.Selection:
                        SelectionHaptic();
                        break;

                    case HapticTypes.Success:
                        SuccessHaptic();
                        break;

                    case HapticTypes.Warning:
                        WarningHaptic();
                        break;

                    case HapticTypes.Failure:
                        FailureHaptic();
                        break;

                    case HapticTypes.LightImpact:
                        LightImpactHaptic();
                        break;

                    case HapticTypes.MediumImpact:
                        MediumImpactHaptic();
                        break;

                    case HapticTypes.HeavyImpact:
                        HeavyImpactHaptic();
                        break;
                }
            }
            else if (_defaultToRegularVibrate)
            {
#if UNITY_IOS
                Handheld.Vibrate();
#endif
            }
        }

        public static string iOSSDKVersion()
        {
#if UNITY_IOS && !UNITY_EDITOR
				return Device.systemVersion;
#else
            return null;
#endif
        }

    }
}