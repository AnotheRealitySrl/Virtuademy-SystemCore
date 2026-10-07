using Virtuademy.SDK.Core.Fade;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using SPACS.Utilities;

namespace Virtuademy.SDK.Fade.Samples
{
    public class VolumeManager : MonoBehaviour, IFadeManager
    {
        #region Inspector variables

        [Header("Configuration")]
        [SerializeField] private Volume fadeVolume;
        [SerializeField] private Volume desaturateVolume;
        [SerializeField] private Volume backgroundVolume;

        #endregion

        #region Private variables

        private Coroutine blackCoroutine = null;
        private Coroutine desaturatedCoroutine = null;
        private Coroutine backgroundCoroutine = null;

        #endregion

        #region Interface implementation

        public float FadeTime { get; set; }
        public bool FadeOnStart { get; set; }

        public ILayerManager LayerManager { get; set; }

        public UnityEvent OnFadeStart { get; } = new();
        public UnityEvent OnFadeEnd { get; } = new();

        public void Init()
        {
            // Desaturated volume begins always deactivated;
            desaturateVolume.weight = 0;

            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(GetComponentInChildren<Camera>());

            if (FadeOnStart)
            {
                FadeFromBlack();
            }
        }

        public void FadeToBackground(Action onEnd = null)
        {
            if (backgroundVolume != null)
            {
                if (backgroundCoroutine != null)
                    StopCoroutine(backgroundCoroutine);

                LayerManager.MoveObjectsToLayer();
                backgroundCoroutine = StartCoroutine(FadeVolumeWeight(backgroundVolume, backgroundVolume.weight, 1, FadeTime * (1f - fadeVolume.weight), onEnd));
            }
        }

        public void FadeFromBackground(Action onEnd = null)
        {
            if (backgroundVolume != null)
            {
                if (backgroundCoroutine != null)
                    StopCoroutine(backgroundCoroutine);

                backgroundCoroutine = StartCoroutine(FadeVolumeWeight(backgroundVolume, backgroundVolume.weight, 0, FadeTime * backgroundVolume.weight, () =>
                {
                    onEnd?.Invoke();
                    LayerManager.ResetObjectsLayer();
                }));
            }
        }

        public void FadeToBlack(System.Action onEnd = null)
        {
            if (fadeVolume != null)
            {
                if (blackCoroutine != null)
                    StopCoroutine(blackCoroutine);

                LayerManager.MoveObjectsToLayer();
                blackCoroutine = StartCoroutine(FadeVolumeWeight(fadeVolume, fadeVolume.weight, 1, FadeTime * (1f - fadeVolume.weight), onEnd));
            }
        }

        public void FadeFromBlack(System.Action onEnd = null)
        {
            if (fadeVolume != null)
            {
                if (blackCoroutine != null)
                    StopCoroutine(blackCoroutine);

                blackCoroutine = StartCoroutine(FadeVolumeWeight(fadeVolume, fadeVolume.weight, 0, FadeTime * fadeVolume.weight, () =>
                {
                    onEnd?.Invoke();
                    LayerManager.ResetObjectsLayer();
                }));
            }
        }

        public void FadeToDesaturated(System.Action onEnd = null)
        {
            if (desaturateVolume != null)
            {
                if (desaturatedCoroutine != null)
                    StopCoroutine(desaturatedCoroutine);

                LayerManager.MoveObjectsToLayer();
                desaturatedCoroutine = StartCoroutine(FadeVolumeWeight(desaturateVolume, desaturateVolume.weight, 1, FadeTime * (1f - desaturateVolume.weight), onEnd));
            }
        }

        public void FadeFromDesaturated(System.Action onEnd = null)
        {
            if (desaturateVolume != null)
            {
                if (desaturatedCoroutine != null)
                    StopCoroutine(desaturatedCoroutine);

                desaturatedCoroutine = StartCoroutine(FadeVolumeWeight(desaturateVolume, desaturateVolume.weight, 0, FadeTime * desaturateVolume.weight, () =>
                {
                    onEnd?.Invoke();
                    LayerManager.ResetObjectsLayer();
                }));
            }
        }

        public void InterruptFade()
        {
            if (blackCoroutine != null)
                StopCoroutine(blackCoroutine);

            if (desaturatedCoroutine != null)
                StopCoroutine(desaturatedCoroutine);

            if (backgroundCoroutine != null)
                StopCoroutine(backgroundCoroutine);

            StopAllCoroutines();
        }
        #endregion

        #region Private methods

        // I would do this with a tween, but unfortunately, Volume.weight can't be modified with a FloatTweener
        IEnumerator FadeVolumeWeight(Volume volume, float startWeight, float endWeight, float fadeTime, System.Action onEnd)
        {
            if (fadeTime > 0f)
            {
                for (float t = 0; t < 1; t += Time.deltaTime / fadeTime)
                {
                    volume.weight = EasingFunctions.EaseInOutQuad(startWeight, endWeight, t);
                    yield return null;
                }
            }
            volume.weight = endWeight;
            onEnd?.Invoke();
            yield return null;
        }

        public void SetTargetCamera(Camera camera)
        {

        }

        #endregion
    }
}
