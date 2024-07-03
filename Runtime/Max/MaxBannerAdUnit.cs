using System;
using VirtueSky.Misc;

namespace VirtueSky.Ads
{
    [Serializable]
    public class MaxBannerAdUnit : AdUnit
    {
        public BannerSize size = BannerSize.Banner;
        public BannerPosition position = BannerPosition.Bottom;

        private bool _isBannerDestroyed = true;
        private bool _isBannerShowing;
        private bool _previousBannerShowStatus;

        public override void Init()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
#if VIRTUESKY_TRACKING
            paidedCallback = VirtueSky.Tracking.AppTracking.TrackRevenue;
#endif
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpanded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsed;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClicked;
            if (size != BannerSize.Adaptive)
            {
                MaxSdk.SetBannerExtraParameter(Id, "adaptive_banner", "false");
            }
#endif
        }

        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            if (AdStatic.IsRemoveAd || string.IsNullOrEmpty(Id)) return;
            if (_isBannerDestroyed)
            {
                MaxSdk.CreateBanner(Id, ConvertPosition());
                _isBannerDestroyed = false;
            }
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
            return !string.IsNullOrEmpty(Id);
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            _isBannerShowing = true;
            AdStatic.waitAppOpenClosedAction = OnWaitAppOpenClosed;
            AdStatic.waitAppOpenDisplayedAction = OnWaitAppOpenDisplayed;
            Load();
            MaxSdk.ShowBanner(Id);
#endif
        }

        public override void Destroy()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            if (string.IsNullOrEmpty(Id)) return;
            _isBannerShowing = false;
            _isBannerDestroyed = true;
            AdStatic.waitAppOpenClosedAction = null;
            AdStatic.waitAppOpenDisplayedAction = null;
            MaxSdk.DestroyBanner(Id);
#endif
        }

        public override void HideBanner()
        {
            base.HideBanner();
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            _isBannerShowing = false;
            if (string.IsNullOrEmpty(Id)) return;
            MaxSdk.HideBanner(Id);
#endif
        }

        #region Fun Callback

#if VIRTUESKY_ADS && VIRTUESKY_MAX
        public MaxSdkBase.BannerPosition ConvertPosition()
        {
            switch (position)
            {
                case BannerPosition.Top: return MaxSdkBase.BannerPosition.TopCenter;
                case BannerPosition.Bottom: return MaxSdkBase.BannerPosition.BottomCenter;
                case BannerPosition.TopLeft: return MaxSdkBase.BannerPosition.TopLeft;
                case BannerPosition.TopRight: return MaxSdkBase.BannerPosition.TopRight;
                case BannerPosition.BottomLeft: return MaxSdkBase.BannerPosition.BottomLeft;
                case BannerPosition.BottomRight: return MaxSdkBase.BannerPosition.BottomRight;
                default:
                    return MaxSdkBase.BannerPosition.BottomCenter;
            }
        }

        private void OnAdRevenuePaid(string unit, MaxSdkBase.AdInfo info)
        {
            paidedCallback?.Invoke(info.Revenue,
                info.NetworkName,
                unit,
                info.AdFormat, AdNetwork.Max.ToString());
        }

        private void OnAdLoaded(string unit, MaxSdkBase.AdInfo info)
        {
            Common.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdClicked(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Common.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        private void OnAdExpanded(string unit, MaxSdkBase.AdInfo info)
        {
            Common.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }

        private void OnAdLoadFailed(string unit, MaxSdkBase.ErrorInfo info)
        {
            Common.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(info.Message);
        }

        private void OnAdCollapsed(string unit, MaxSdkBase.AdInfo info)
        {
            Common.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
        }
#endif

        #endregion
    }
}