using System;
#if VIRTUESKY_ADMOB && VIRTUESKY_ADS
using GoogleMobileAds.Api;
#endif
using VirtueSky.Misc;

namespace VirtueSky.Ads
{
    [Serializable]
    public class AdmobRewardedInterstitialAdUnit : AdUnit
    {
        public bool useTestId;
        [NonSerialized] internal Action completedCallback;
        [NonSerialized] internal Action skippedCallback;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private RewardedInterstitialAd _rewardedInterstitialAd;
#endif
        public override void Init()
        {
            if (useTestId)
            {
                GetUnitTest();
            }
#if VIRTUESKY_TRACKING
            if (string.IsNullOrEmpty(Id)) return;
            paidedCallback += VirtueSky.Tracking.AppTracking.TrackRevenue;
#endif
        }

        public bool IsEarnRewarded { get; private set; }

        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (string.IsNullOrEmpty(Id)) return;
            Destroy();
            RewardedInterstitialAd.Load(Id, new AdRequest(), OnAdLoadCallback);
#endif
        }

        public override bool IsReady()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            return _rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd();
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _rewardedInterstitialAd.Show(UserEarnedRewardCallback);
#endif
        }

        protected override void ResetChainCallback()
        {
            base.ResetChainCallback();
            completedCallback = null;
            skippedCallback = null;
        }

        public override AdUnit Show()
        {
            ResetChainCallback();
            if (!UnityEngine.Application.isMobilePlatform || string.IsNullOrEmpty(Id) || !IsReady())
                return this;
            ShowImpl();
            return this;
        }

        public override void Destroy()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (_rewardedInterstitialAd == null) return;
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
            IsEarnRewarded = false;
#endif
        }

        #region Fun Callback

#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private void OnAdLoadCallback(RewardedInterstitialAd ad, LoadAdError error)
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                OnAdFailedToLoad(error);
                return;
            }

            _rewardedInterstitialAd = ad;
            _rewardedInterstitialAd.OnAdFullScreenContentClosed += OnAdClosed;
            _rewardedInterstitialAd.OnAdFullScreenContentOpened += OnAdOpening;
            _rewardedInterstitialAd.OnAdFullScreenContentFailed += OnAdFailedToShow;
            _rewardedInterstitialAd.OnAdPaid += OnAdPaided;
            _rewardedInterstitialAd.OnAdClicked += OnAdClicked;
            OnAdLoaded();
        }

        private void OnAdClicked()
        {
            Common.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        private void OnAdFailedToLoad(LoadAdError error)
        {
            Common.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(error.GetCode().ToString(), error.GetMessage());
        }

        private void OnAdLoaded()
        {
            Common.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdPaided(AdValue value)
        {
            paidedCallback?.Invoke(value.Value / 1000000f,
                "Admob",
                Id,
                "RewardedInterstitialAd", AdNetwork.Admob.ToString());
        }

        private void OnAdFailedToShow(AdError error)
        {
            Common.CallActionAndClean(ref failedToDisplayCallback);
            OnFailedToDisplayAdEvent?.Invoke(error.GetMessage());
        }

        private void OnAdOpening()
        {
            AdStatic.isShowingAd = true;
            Common.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }

        private void OnAdClosed()
        {
            AdStatic.isShowingAd = false;
            Common.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
            if (IsEarnRewarded)
            {
                Common.CallActionAndClean(ref completedCallback);
                _rewardedInterstitialAd.Destroy();
                return;
            }

            Common.CallActionAndClean(ref skippedCallback);
            _rewardedInterstitialAd.Destroy();
        }

        private void UserEarnedRewardCallback(Reward reward)
        {
            IsEarnRewarded = true;
        }
#endif

        #endregion

        void GetUnitTest()
        {
#if UNITY_ANDROID
            androidId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IOS
            iOSId = "ca-app-pub-3940256099942544/6978759866";
#endif
        }
    }
}