using System;
using VirtueSky.Misc;

namespace VirtueSky.Ads
{
    [Serializable]
    public class MaxRewardedInterstitialAdUnit : AdUnit
    {
        [NonSerialized] internal Action completedCallback;
        [NonSerialized] internal Action skippedCallback;
        public bool IsEarnRewarded { get; private set; }


        public override void Init()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            if (string.IsNullOrEmpty(Id)) return;
#if VIRTUESKY_TRACKING
            paidedCallback = VirtueSky.Tracking.AppTracking.TrackRevenue;
#endif
            MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayedEvent += OnAdDisplayed;
            MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnAdHidden;
            MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayFailedEvent += OnAdDisplayFailed;
            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.RewardedInterstitial.OnAdReceivedRewardEvent += OnAdReceivedReward;
            MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnAdClicked;
#endif
        }

        public override void Load()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            if (string.IsNullOrEmpty(Id)) return;
            MaxSdk.LoadRewardedInterstitialAd(Id);
#endif
        }


        public override bool IsReady()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            return !string.IsNullOrEmpty(Id) && MaxSdk.IsRewardedInterstitialAdReady(Id);
#else
            return false;
#endif
        }

        protected override void ShowImpl()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            MaxSdk.ShowRewardedInterstitialAd(Id);
#endif
        }

        public override void Destroy()
        {
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
            if (!UnityEngine.Application.isMobilePlatform || !IsReady()) return this;
            ShowImpl();
            return this;
        }

        #region Func Callback

#if VIRTUESKY_ADS && VIRTUESKY_MAX
        private void OnAdRevenuePaid(string unit, MaxSdkBase.AdInfo info)
        {
            paidedCallback?.Invoke(info.Revenue,
                info.NetworkName,
                unit,
                info.AdFormat, AdNetwork.Max.ToString());
        }

        private void OnAdReceivedReward(string unit, MaxSdkBase.Reward reward,
            MaxSdkBase.AdInfo info)
        {
            IsEarnRewarded
                =
                true;
        }

        private void OnAdClicked(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Common.CallActionAndClean(ref clickedCallback);
            OnClickedAdEvent?.Invoke();
        }

        private void OnAdLoadFailed(string unit, MaxSdkBase.ErrorInfo error)
        {
            Common.CallActionAndClean(ref failedToLoadCallback);
            OnFailedToLoadAdEvent?.Invoke(error.Message);
        }

        private void OnAdLoaded(string unit, MaxSdkBase.AdInfo info)
        {
            Common.CallActionAndClean(ref loadedCallback);
            OnLoadAdEvent?.Invoke();
        }

        private void OnAdDisplayFailed(string unit, MaxSdkBase.ErrorInfo error,
            MaxSdkBase.AdInfo info)
        {
            Common.CallActionAndClean(ref failedToDisplayCallback);
            OnFailedToDisplayAdEvent?.Invoke(error.Message);
        }

        private void OnAdHidden(string unit, MaxSdkBase.AdInfo info)
        {
            AdStatic.isShowingAd = false;
            Common.CallActionAndClean(ref closedCallback);
            OnClosedAdEvent?.Invoke();
            if (!IsReady()) MaxSdk.LoadRewardedInterstitialAd(Id);

            if (IsEarnRewarded)
            {
                Common.CallActionAndClean(ref completedCallback);
                IsEarnRewarded = false;
                return;
            }

            Common.CallActionAndClean(ref skippedCallback);
        }

        private void OnAdDisplayed(string unit, MaxSdkBase.AdInfo info)
        {
            AdStatic.isShowingAd = true;
            Common.CallActionAndClean(ref displayedCallback);
            OnDisplayedAdEvent?.Invoke();
        }
#endif

        #endregion
    }
}