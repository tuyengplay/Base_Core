using System.Collections;
using EranCore;
using EranCore.Audio;
using EranCore.Tweening;
using EranCore.UI;
using GameCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace GameCore
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasScaler))]
    public class Loader : Singleton<Loader>
    {
        [SerializeField] private CanvasGroup splashBG;
        [SerializeField] private Image imgCountDown;
        [SerializeField] private Text txtLoading;
        [SerializeField] private bool isLoadDone;
        public bool IsLoadDone => isLoadDone;
        public void FakeLoading()
        {
            splashBG.alpha = 1f;
            imgCountDown.fillAmount = 0f;
            imgCountDown.TweenFillAmount(0.8f, 4f);
            txtLoading.text = $"Loading";
            if (txtLoadingRoutine == null)
            {
                txtLoadingRoutine = StartCoroutine(TextLoading());
            }
            this.gameObject.SetActive(true);
        }
        public void LoadScene(ScenesName _sceneName)
        {
            AudioManager.Instance.StopMusic();
            isLoadDone = false;
            splashBG.alpha = 1f;
            imgCountDown.TweenKill();
            txtLoading.text = $"Loading";
            this.gameObject.SetActive(true);
            ResetData();
            StartCoroutine(RoutineLoadScene(_sceneName));
            if (txtLoadingRoutine == null)
            {
                txtLoadingRoutine = StartCoroutine(TextLoading());
            }
        }

        private IEnumerator RoutineLoadScene(ScenesName _sceneName)
        {
            //if (_sceneName == ScenesName.GameScene)
            //{
            //    ResourceRequest request = LevelManager.Instance.Preload(0);
            //    while (!request.isDone)
            //    {
            //        yield return null;
            //    }
            //}
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName.ToString());
            asyncOperation.allowSceneActivation = true;
            while (asyncOperation.isDone == false)
            {
                yield return null;
            }
            imgCountDown.TweenFillAmount(1, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                splashBG.TweenFade(0, 0.8f).OnComplete(() =>
                {
                    isLoadDone = true;
                    gameObject.SetActive(false);
                    imgCountDown.fillAmount = 0f;
                    if (txtLoadingRoutine != null)
                    {
                        StopCoroutine(txtLoadingRoutine);
                        txtLoadingRoutine = null;
                    }
                });
            });
        }
        private void ResetData()
        {
            GameEvent.ClearDelegates();
            if (UIManager.Instance)
            {
                UIManager.Instance.CloseAllUI();
            }
        }

        private IEnumerator TextLoading(string _nameLoading = "")
        {
            int indexCountText = 0;
            while (true)
            {
                switch (indexCountText % 4)
                {
                    case 0:
                        {
                            txtLoading.text = $"Loading {_nameLoading}";
                            indexCountText++;
                            break;
                        }
                    case 1:
                        {
                            txtLoading.text = $"Loading {_nameLoading}.";
                            indexCountText++;
                            break;
                        }
                    case 2:
                        {
                            txtLoading.text = $"Loading {_nameLoading}..";
                            indexCountText++;
                            break;
                        }
                    case 3:
                        {
                            txtLoading.text = $"Loading {_nameLoading}...";
                            indexCountText = 0;
                            break;
                        }
                }

                yield return new WaitForSeconds(0.15f);
            }
        }

        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasScaler canvasScale;
        private Coroutine txtLoadingRoutine;

        private void OnValidate()
        {
            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            canvasScale = GetComponent<CanvasScaler>();
            canvasScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScale.matchWidthOrHeight = 0.5f;
        }
    }
}