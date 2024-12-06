using System;
using System.Collections;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
using GoogleMobileAds.Api;
using VirtueSky.Core;
using VirtueSky.Misc;
#endif
using UnityEngine;
using VirtueSky.Tracking;

namespace VirtueSky.Ads
{
    [Serializable]
    public class AdmobNativeOverlayAdUnit : AdUnit
    {
        public enum NativeTemplate
        {
            Small,
            Medium
        }

        [SerializeField] private bool useTestId;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        [Header("Native Options"), SerializeField]
        private AdChoicesPlacement adChoicesPlacement;

        [SerializeField] private MediaAspectRatio mediaAspectRatio;
        [SerializeField] private VideoOptions videoOptions;

        [Header("NativeAd Style")] public NativeTemplate nativeTemplate;
        public Color mainBackgroundColor = Color.white;
        public AdsSize adsSize = AdsSize.MediumRectangle;
        public AdsPosition adsPosition = AdsPosition.Bottom;

        private NativeOverlayAd _nativeOverlayAd;
#endif
        private readonly WaitForSeconds _waitReload = new WaitForSeconds(5);
        private IEnumerator _reload;

        /// <summary>
        /// Init ads and register callback tracking revenue
        /// </summary>
        public override void Init()
        {
            if (useTestId) GetUnitTest();
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            paidedCallback += AppTracking.TrackRevenue;
#endif
        }

        /// <summary>
        /// Load ads
        /// </summary>
        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            if (_nativeOverlayAd != null) Destroy();
            var adRequest = new AdRequest();
            var option = new NativeAdOptions
            {
                AdChoicesPlacement = adChoicesPlacement,
                MediaAspectRatio = mediaAspectRatio,
                VideoOptions = videoOptions
            };
            NativeOverlayAd.Load(Id, adRequest, option, AdLoadCallback);
#endif
        }

        /// <summary>
        /// Check native overlay is ready
        /// </summary>
        /// <returns>If true then native overlay ads ready</returns>
        public override bool IsReady()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            return _nativeOverlayAd != null;
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_nativeOverlayAd != null) _nativeOverlayAd.Show();
#endif
        }

        /// <summary>
        /// Destroy native overlay ads
        /// </summary>
        public override void Destroy()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_nativeOverlayAd != null)
            {
                _nativeOverlayAd.Destroy();
                _nativeOverlayAd = null;
            }
#endif
        }

        /// <summary>
        /// Hide native overlay ads
        /// </summary>
        public void Hide()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_nativeOverlayAd != null) _nativeOverlayAd.Hide();
#endif
        }

        /// <summary>
        /// Render native overlay ads default
        /// </summary>
        public void RenderAd()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADS
            if (_nativeOverlayAd == null) return;
            _nativeOverlayAd.RenderTemplate(Style(), ConvertSize(), ConvertPosition(adsPosition));
#endif
        }

        /// <summary>
        /// Render native ads according to uiElement, use canvas overlay
        /// </summary>
        /// <param name="uiElement">RectTransform of uiElement, used to determine position for native overlay ads</param>
        public void RenderAd(RectTransform uiElement)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADS
            if (_nativeOverlayAd == null) return;
            (int admobX, int admobY) = ConvertUiElementPosToNativeAdsPos(uiElement);
            _nativeOverlayAd.RenderTemplate(Style(), admobX, admobY);
#endif
        }

        /// <summary>
        /// Render native ads according to uiElement, use canvas overlay
        /// </summary>
        /// <param name="uiElement">RectTransform of uiElement, used to determine position for native overlay ads</param>
        /// <param name="width">Custom width for native overlay ads</param>
        /// <param name="height">Custom height for native overlay ads</param>
        public void RenderAd(RectTransform uiElement, int width, int height)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADS
            if (_nativeOverlayAd == null) return;
            (int admobX, int admobY) = ConvertUiElementPosToNativeAdsPos(uiElement);
            _nativeOverlayAd.RenderTemplate(Style(), new AdSize(width, height), admobX, admobY);
#endif
        }

        /// <summary>
        /// Render native ads according to uiElement, use canvas screen-space camera
        /// Can use position and size of uiElement for native overlay ads
        /// </summary>
        /// <param name="uiElement">RectTransform of uiElement, used to determine position for native overlay ads</param>
        /// <param name="camera">Camera render uiElement</param>
        public void RenderAd(RectTransform uiElement, Camera camera, bool useSizeUiElement = true)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_nativeOverlayAd == null) return;
            (int admobX, int admobY) = ConvertUiElementPosToNativeAdsPos(uiElement, camera);
            if (useSizeUiElement)
            {
                _nativeOverlayAd?.RenderTemplate(Style(), new AdSize((int)uiElement.rect.width, (int)uiElement.rect.height), admobX, admobY);
            }
            else
            {
                _nativeOverlayAd?.RenderTemplate(Style(), admobX, admobY);
            }
#endif
        }

        /// <summary>
        /// Render native ads according to uiElement, use canvas screen-space camera
        /// Can use position of uiElement and custom size for native overlay ads
        /// </summary>
        /// <param name="uiElement">RectTransform of uiElement, used to determine position for native overlay ads</param>
        /// <param name="camera">Camera render uiElement</param>
        /// <param name="width">Custom width for native overlay ads</param>
        /// <param name="height">Custom height for native overlay ads</param>
        public void RenderAd(RectTransform uiElement, Camera camera, int width, int height)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_nativeOverlayAd == null) return;
            (int admobX, int admobY) = ConvertUiElementPosToNativeAdsPos(uiElement, camera, width, height);
            _nativeOverlayAd?.RenderTemplate(Style(), new AdSize(width, height), admobX, admobY);
#endif
        }

        (int, int) ConvertUiElementPosToNativeAdsPos(RectTransform uiElement, Camera camera, int width, int height)
        {
            var worldPosition = uiElement.TransformPoint(uiElement.position);
            Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);

            float dpi = Screen.dpi / 160f;
            var admobX = (int)((screenPosition.x - width / 2) / dpi);
            var admobY = (int)(((Screen.height - (int)screenPosition.y) - height / 2) / dpi);
            return (admobX, admobY);
        }

        (int, int) ConvertUiElementPosToNativeAdsPos(RectTransform uiElement, Camera camera)
        {
            var worldPosition = uiElement.TransformPoint(uiElement.position);
            Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);

            float dpi = Screen.dpi / 160f;
            var admobX = (int)((screenPosition.x - (int)uiElement.rect.width / 2) / dpi);
            var admobY = (int)(((Screen.height - (int)screenPosition.y) - (int)uiElement.rect.height / 2) / dpi);
            return (admobX, admobY);
        }

        (int, int) ConvertUiElementPosToNativeAdsPos(RectTransform uiElement)
        {
            var screenPosition = uiElement.ToWorldPosition();
            float dpi = Screen.dpi / 160f;
            var admobX = (int)(screenPosition.x / dpi);
            var admobY = (int)((Screen.height - (int)screenPosition.y) / dpi);
            return (admobX, admobY);
        }

        public void SetPosition(AdsPosition adsPosition)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _nativeOverlayAd.SetTemplatePosition(ConvertPosition(adsPosition));
#endif
        }

        public void SetPosition(int x, int y)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _nativeOverlayAd.SetTemplatePosition(x, y);
#endif
        }

        public void SetPosition(RectTransform uiElement)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            (int x, int y) = ConvertUiElementPosToNativeAdsPos(uiElement);
            _nativeOverlayAd.SetTemplatePosition(x, y);
#endif
        }

        public void SetPosition(RectTransform uiElement, Camera camera)
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            (int x, int y) = ConvertUiElementPosToNativeAdsPos(uiElement, camera);
            _nativeOverlayAd.SetTemplatePosition(x, y);
#endif
        }

#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        NativeTemplateStyle Style()
        {
            return new NativeTemplateStyle
            {
                TemplateId = nativeTemplate.ToString().ToLower(),
                MainBackgroundColor = mainBackgroundColor
            };
        }

        AdPosition ConvertPosition(AdsPosition _adsPosition)
        {
            return _adsPosition switch
            {
                AdsPosition.Top => AdPosition.Top,
                AdsPosition.Bottom => AdPosition.Bottom,
                AdsPosition.TopLeft => AdPosition.TopLeft,
                AdsPosition.TopRight => AdPosition.TopRight,
                AdsPosition.BottomLeft => AdPosition.BottomLeft,
                AdsPosition.BottomRight => AdPosition.BottomRight,
                _ => AdPosition.Center
            };
        }

        AdSize ConvertSize()
        {
            return adsSize switch
            {
                AdsSize.Banner => AdSize.Banner,
                AdsSize.MediumRectangle => AdSize.MediumRectangle,
                AdsSize.IABBanner => AdSize.IABBanner,
                AdsSize.Leaderboard => AdSize.Leaderboard,
                _ => AdSize.MediumRectangle,
            };
        }

        private void AdLoadCallback(NativeOverlayAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                OnAdFailedToLoad(error);
                return;
            }

            _nativeOverlayAd = ad;
            _nativeOverlayAd.OnAdPaid += OnAdPaided;
            _nativeOverlayAd.OnAdClicked += OnAdClicked;
            _nativeOverlayAd.OnAdFullScreenContentOpened += OnAdOpening;
            _nativeOverlayAd.OnAdFullScreenContentClosed += OnAdClosed;
            OnAdLoaded();
        }

        private void OnAdLoaded()
        {
            Common.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdClosed()
        {
            Common.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
        }

        private void OnAdOpening()
        {
            Common.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }

        private void OnAdClicked()
        {
            Common.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        private void OnAdPaided(AdValue value)
        {
            paidedCallback?.Invoke(value.Value / 1000000f,
                "Admob",
                Id,
                "NativeOverlayAd", AdNetwork.Admob.ToString());
        }

        private void OnAdFailedToLoad(LoadAdError error)
        {
            Common.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(error.GetMessage());
            if (_reload != null) App.StopCoroutine(_reload);
            _reload = DelayReload();
            App.StartCoroutine(_reload);
        }

        private IEnumerator DelayReload()
        {
            yield return _waitReload;
            Load();
        }
#endif


        void GetUnitTest()
        {
#if UNITY_ANDROID
            androidId = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IOS
            iOSId = "ca-app-pub-3940256099942544/3986624511";
#endif
        }
    }
}