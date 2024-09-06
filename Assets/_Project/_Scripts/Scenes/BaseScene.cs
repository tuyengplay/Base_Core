using EranCore;
using EranCore.UI;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    public class BaseScene : Singleton<BaseScene>
    {
        public static ScenesName currentScene = ScenesName.SplashScene;
        public static ScenesName prevScene = ScenesName.SplashScene;

        public Camera UICamera;
        public Camera GameCamera;

        [SerializeField] private Theme themeGame;
        public Theme ThemeGame => themeGame;
        [SerializeField] private ScenesName scenesName;
        public ScenesName ScenesName => scenesName;
        [SerializeField] private int targetFrame = -1;
        public UnityEvent Reset;

        protected override void Awake()
        {
            base.Awake();
            currentScene = scenesName;
            Time.timeScale = 1;
            Application.targetFrameRate = targetFrame;
        }
        protected virtual void Start()
        {
            ItemFX.Instance?.OnSetupCamera(UICamera);
            UIManager.Instance?.SetUpPopup(UICamera);
        }

        private void OnDisable()
        {
            Reset?.Invoke();
        }
    }
}

public enum ScenesName
{
    SplashScene,
    MenuScene,
    GameScene,
}

public enum Theme
{
    Nomal,
}