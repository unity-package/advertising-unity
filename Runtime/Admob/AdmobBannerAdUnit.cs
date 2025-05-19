using System;
using UnityEngine;
using VirtueSky.Misc;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
using GoogleMobileAds.Api;
#endif
using System.Collections;
using VirtueSky.Core;

namespace VirtueSky.Ads
{
    [Serializable]
    public class AdmobBannerAdUnit : AdUnit
    {
        public AdsSize size = AdsSize.Adaptive;
        public AdsPosition position = AdsPosition.Bottom;
        public bool useCollapsible;
        public bool useTestId;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private BannerView _bannerView;
#endif
        private readonly WaitForSeconds _waitBannerReload = new WaitForSeconds(5f);
        private IEnumerator _reload;
        private bool _isBannerShowing;
        private bool _previousBannerShowStatus;

        public override void Init()
        {
            if (useTestId)
            {
                GetUnitTest();
            }
#if VIRTUESKY_TRACKING
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            paidedCallback += VirtueSky.Tracking.AppTracking.TrackRevenue;
#endif
        }

        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            Destroy();
            _bannerView = new BannerView(Id, ConvertSize(), ConvertPosition());
            _bannerView.OnAdFullScreenContentClosed += OnAdClosed;
            _bannerView.OnBannerAdLoadFailed += OnAdFailedToLoad;
            _bannerView.OnBannerAdLoaded += OnAdLoaded;
            _bannerView.OnAdFullScreenContentOpened += OnAdOpening;
            _bannerView.OnAdPaid += OnAdPaided;
            _bannerView.OnAdClicked += OnAdClicked;
            var adRequest = new AdRequest();
            if (useCollapsible)
            {
                adRequest.Extras.Add("collapsible", ConvertPlacementCollapsible());
            }

            _bannerView.LoadAd(adRequest);

#endif
        }

        public bool IsCollapsible()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_bannerView == null) return false;
            return _bannerView.IsCollapsible();
#else
            return false;
#endif
        }

        void OnWaitAppOpenClosed()
        {
            if (_previousBannerShowStatus)
            {
                _previousBannerShowStatus = false;
                Show();
            }
        }

        void OnWaitAppOpenDisplayed()
        {
            _previousBannerShowStatus = _isBannerShowing;
            if (_isBannerShowing) HideBanner();
        }

        public override bool IsReady()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            return _bannerView != null;
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _isBannerShowing = true;
            AdStatic.waitAppOpenClosedAction = OnWaitAppOpenClosed;
            AdStatic.waitAppOpenDisplayedAction = OnWaitAppOpenDisplayed;
            if (_bannerView == null)
            {
                Load();
            }

            _bannerView.Show();
#endif
        }

        public override void Destroy()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_bannerView == null) return;
            _isBannerShowing = false;
            AdStatic.waitAppOpenClosedAction = null;
            AdStatic.waitAppOpenDisplayedAction = null;
            _bannerView.Destroy();
            _bannerView = null;
#endif
        }

        public override void HideBanner()
        {
            base.HideBanner();
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _isBannerShowing = false;
            if (_bannerView != null) _bannerView.Hide();
#endif
        }

        #region Fun Callback

#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        public AdSize ConvertSize()
        {
            switch (size)
            {
                case AdsSize.Adaptive:
                    return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(
                        AdSize.FullWidth);
                case AdsSize.MediumRectangle: return AdSize.MediumRectangle;
                case AdsSize.Leaderboard: return AdSize.Leaderboard;
                case AdsSize.IABBanner: return AdSize.IABBanner;
                //case BannerSize.SmartBanner: return AdSize.SmartBanner;
                default: return AdSize.Banner;
            }
        }

        private void OnAdClicked()
        {
            Common.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        public AdPosition ConvertPosition()
        {
            switch (position)
            {
                case AdsPosition.Top: return AdPosition.Top;
                case AdsPosition.Bottom: return AdPosition.Bottom;
                case AdsPosition.TopLeft: return AdPosition.TopLeft;
                case AdsPosition.TopRight: return AdPosition.TopRight;
                case AdsPosition.BottomLeft: return AdPosition.BottomLeft;
                case AdsPosition.BottomRight: return AdPosition.BottomRight;
                default: return AdPosition.Bottom;
            }
        }

        public string ConvertPlacementCollapsible()
        {
            if (position == AdsPosition.Top)
            {
                return "top";
            }
            else if (position == AdsPosition.Bottom)
            {
                return "bottom";
            }

            return "bottom";
        }

        private void OnAdPaided(AdValue value)
        {
            paidedCallback?.Invoke(value.Value / 1000000f,
                "Admob",
                Id,
                "BannerAd", AdNetwork.Admob.ToString());
        }

        private void OnAdOpening()
        {
            Common.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }

        private void OnAdLoaded()
        {
            Common.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdFailedToLoad(LoadAdError error)
        {
            Common.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(error.GetCode().ToString(), error.GetMessage());
            if (_reload != null) App.StopCoroutine(_reload);
            _reload = DelayBannerReload();
            App.StartCoroutine(_reload);
        }

        private void OnAdClosed()
        {
            Common.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
        }

        private IEnumerator DelayBannerReload()
        {
            yield return _waitBannerReload;
            Load();
        }
#endif

        #endregion

        void GetUnitTest()
        {
#if UNITY_ANDROID
            androidId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
            iOSId = "ca-app-pub-3940256099942544/2934735716";
#endif
        }
    }
}