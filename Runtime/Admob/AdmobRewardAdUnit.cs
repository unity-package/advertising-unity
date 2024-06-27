using System;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
using GoogleMobileAds.Api;
#endif


namespace VirtueSky.Ads
{
    [Serializable]
    public class AdmobRewardAdUnit : AdUnit
    {
        public bool useTestId;
        [NonSerialized] internal Action completedCallback;
        [NonSerialized] internal Action skippedCallback;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private RewardedAd _rewardedAd;
#endif
        public override void Init()
        {
            if (useTestId)
            {
                GetUnitTest();
            }
        }

        public bool IsEarnRewarded { get; private set; }

        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            if (string.IsNullOrEmpty(Id)) return;
            Destroy();
            RewardedAd.Load(Id, new AdRequest(), AdLoadCallback);
#endif
        }

        public override bool IsReady()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            return _rewardedAd != null && _rewardedAd.CanShowAd();
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            _rewardedAd.Show(UserRewardEarnedCallback);
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
            if (_rewardedAd == null) return;
            _rewardedAd.Destroy();
            _rewardedAd = null;
            IsEarnRewarded = false;
#endif
        }

        #region Fun Callback

#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private void AdLoadCallback(RewardedAd ad, LoadAdError error)
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                OnAdFailedToLoad(error);
                return;
            }

            _rewardedAd = ad;
            _rewardedAd.OnAdFullScreenContentClosed += OnAdClosed;
            _rewardedAd.OnAdFullScreenContentFailed += OnAdFailedToShow;
            _rewardedAd.OnAdFullScreenContentOpened += OnAdOpening;
            _rewardedAd.OnAdPaid += OnAdPaided;
            _rewardedAd.OnAdClicked += OnAdClicked;
            OnAdLoaded();
        }

        private void OnAdClicked()
        {
            AdStatic.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        private void OnAdPaided(AdValue value)
        {
            paidedCallback?.Invoke(value.Value / 1000000f,
                "Admob",
                Id,
                "RewardedAd", AdNetwork.Admob.ToString());
        }

        private void OnAdOpening()
        {
            AdStatic.isShowingAd = true;
            AdStatic.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }

        private void OnAdFailedToShow(AdError obj)
        {
            AdStatic.CallActionAndClean(ref failedToDisplayCallback);
            OnFailedToDisplayAdEvent?.Invoke(obj.GetMessage());
        }

        private void OnAdClosed()
        {
            AdStatic.isShowingAd = false;
            AdStatic.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
            if (IsEarnRewarded)
            {
                AdStatic.CallActionAndClean(ref completedCallback);
                Destroy();
                return;
            }

            AdStatic.CallActionAndClean(ref skippedCallback);
            Destroy();
        }

        private void OnAdLoaded()
        {
            AdStatic.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdFailedToLoad(LoadAdError error)
        {
            AdStatic.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(error.GetMessage());
        }

        private void UserRewardEarnedCallback(Reward reward)
        {
            IsEarnRewarded = true;
        }
#endif

        #endregion

        void GetUnitTest()
        {
#if UNITY_ANDROID
            androidId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
            iOSId = "ca-app-pub-3940256099942544/1712485313";
#endif
        }
    }
}