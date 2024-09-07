using System;
using System.Reflection;
using UnityEngine;
using EranCore.Tweening.Plugins.Options;
using EranCore.Tweening.Core;
using EranCore.Tweening.Plugins.Core.PathCore;
namespace EranCore.Tweening
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
        static void PlaymodeStateChanged(UnityEditor.PlayModeStateChange _state)
        {
            if (GameCoreTween.instance == null) return;
            GameCoreTween.instance.OnApplicationPause(UnityEditor.EditorApplication.isPaused);
        }
#endif
        public static class Physics
        {
            public static void SetOrientationOnPath(PathOptions _options, Tween _t, Quaternion _newRot, Transform _trans)
            {
                if (_options.isRigidbody) ((Rigidbody)_t.target).rotation = _newRot;
                else _trans.rotation = _newRot;
            }

            public static bool HasRigidbody2D(Component _target)
            {
                return _target.GetComponent<Rigidbody2D>() != null;
            }



            [UnityEngine.Scripting.Preserve]
            public static bool HasRigidbody(Component _target)
            {
                return _target.GetComponent<Rigidbody>() != null;

            }

            [UnityEngine.Scripting.Preserve]
            public static TweenerCore<Vector3, Path, PathOptions> CreateTweenPathTween(
                MonoBehaviour _target, bool _tweenRigidbody, bool _isLocal, Path _path, float _duration, PathMode _pathMode
            )
            {
                TweenerCore<Vector3, Path, PathOptions> t = null;
                bool rBodyFoundAndTweened = false;
                if (_tweenRigidbody)
                {
                    Rigidbody rBody = _target.GetComponent<Rigidbody>();
                    if (rBody != null)
                    {
                        rBodyFoundAndTweened = true;
                        t = _isLocal
                            ? rBody.TweenLocalPath(_path, _duration, _pathMode)
                            : rBody.TweenPath(_path, _duration, _pathMode);
                    }
                }
                if (!rBodyFoundAndTweened && _tweenRigidbody)
                {
                    Rigidbody2D rBody2D = _target.GetComponent<Rigidbody2D>();
                    if (rBody2D != null)
                    {
                        rBodyFoundAndTweened = true;
                        t = _isLocal
                            ? rBody2D.TweenLocalPath(_path, _duration, _pathMode)
                            : rBody2D.TweenPath(_path, _duration, _pathMode);
                    }
                }
                if (!rBodyFoundAndTweened)
                {
                    t = _isLocal
                        ? _target.transform.TweenLocalPath(_path, _duration, _pathMode)
                        : _target.transform.TweenPath(_path, _duration, _pathMode);
                }
                return t;
            }

        }
    }
}
