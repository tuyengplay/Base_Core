using System;
using System.Reflection;
using UnityEngine;
using EranCore.Tweening.Plugins.Options;
using EranCore.Tweening.Core;
using EranCore.Tweening.Plugins.Core.PathCore;
using EranCore.Tweening;
namespace GameCore.Tweening
{
    public static class TweenModuleUtils
    {
        static bool _initialized;

        #region Reflection

        [UnityEngine.Scripting.Preserve]
        public static void Init()
        {
            if (_initialized) return;

            _initialized = true;
            TweenExternalCommand.SetOrientationOnPath += Physics.SetOrientationOnPath;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += PlaymodeStateChanged;
#endif
        }

        [UnityEngine.Scripting.Preserve]
        static void Preserver()
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            MethodInfo mi = typeof(MonoBehaviour).GetMethod("Stub");
        }

        #endregion
#if UNITY_EDITOR
        static void PlaymodeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (GameCoreTween.instance == null) return;
            GameCoreTween.instance.OnApplicationPause(UnityEditor.EditorApplication.isPaused);
        }
#endif
        public static class Physics
        {
            public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
            {
                if (options.isRigidbody) ((Rigidbody)t.target).rotation = newRot;
                else trans.rotation = newRot;
            }

            public static bool HasRigidbody2D(Component target)
            {
                return target.GetComponent<Rigidbody2D>() != null;
            }



            [UnityEngine.Scripting.Preserve]
            public static bool HasRigidbody(Component target)
            {
                return target.GetComponent<Rigidbody>() != null;

            }

            [UnityEngine.Scripting.Preserve]
            public static TweenerCore<Vector3, Path, PathOptions> CreateTweenPathTween(
                MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode
            )
            {
                TweenerCore<Vector3, Path, PathOptions> t = null;
                bool rBodyFoundAndTweened = false;
                if (tweenRigidbody)
                {
                    Rigidbody rBody = target.GetComponent<Rigidbody>();
                    if (rBody != null)
                    {
                        rBodyFoundAndTweened = true;
                        t = isLocal
                            ? rBody.TweenLocalPath(path, duration, pathMode)
                            : rBody.TweenPath(path, duration, pathMode);
                    }
                }
                if (!rBodyFoundAndTweened && tweenRigidbody)
                {
                    Rigidbody2D rBody2D = target.GetComponent<Rigidbody2D>();
                    if (rBody2D != null)
                    {
                        rBodyFoundAndTweened = true;
                        t = isLocal
                            ? rBody2D.TweenLocalPath(path, duration, pathMode)
                            : rBody2D.TweenPath(path, duration, pathMode);
                    }
                }
                if (!rBodyFoundAndTweened)
                {
                    t = isLocal
                        ? target.transform.TweenLocalPath(path, duration, pathMode)
                        : target.transform.TweenPath(path, duration, pathMode);
                }
                return t;
            }

        }
    }
}
