using System;
using UnityEngine;

namespace VirtueSky.Ads
{
    public static class AdStatic
    {
        public static bool IsRemoveAd
        {
            get => PlayerPrefs.GetInt($"{Application.identifier}_removeads", 0) > 0;
            set => PlayerPrefs.SetInt($"{Application.identifier}_removeads", value ? 1 : 0);
        }

        public static bool isShowingAd;
        internal static Action waitAppOpenDisplayedAction;
        internal static Action waitAppOpenClosedAction;

        public static AdUnit OnDisplayed(this AdUnit unit, Action onDisplayed)
        {
            unit.displayedCallback = onDisplayed;
            return unit;
        }

        public static AdUnit OnClosed(this AdUnit unit, Action onClosed)
        {
            unit.closedCallback = onClosed;
            return unit;
        }

        public static AdUnit OnLoaded(this AdUnit unit, Action onLoaded)
        {
            unit.loadedCallback = onLoaded;
            return unit;
        }

        public static AdUnit OnFailedToLoad(this AdUnit unit, Action onFailedToLoad)
        {
            unit.failedToLoadCallback = onFailedToLoad;
            return unit;
        }

        public static AdUnit OnFailedToDisplay(this AdUnit unit, Action onFailedToDisplay)
        {
            unit.failedToDisplayCallback = onFailedToDisplay;
            return unit;
        }

        public static AdUnit OnClicked(this AdUnit unit, Action onClicked)
        {
            unit.clickedCallback = onClicked;
            return unit;
        }

        public static AdUnit OnCompleted(this AdUnit unit, Action onCompleted)
        {
            if (!Application.isMobilePlatform)
            {
                onCompleted?.Invoke();
            }

            switch (unit)
            {
                case AdmobInterstitialAdUnit admobInter:
                    admobInter.completedCallback = onCompleted;
                    return unit;
                case AdmobRewardAdUnit admobReward:
                    admobReward.completedCallback = onCompleted;
                    return unit;
                case AdmobRewardedInterstitialAdUnit admobRewardInter:
                    admobRewardInter.completedCallback = onCompleted;
                    return unit;
                case MaxInterstitialAdUnit maxInter:
                    maxInter.completedCallback = onCompleted;
                    return unit;
                case MaxRewardAdUnit maxReward:
                    maxReward.completedCallback = onCompleted;
                    return unit;
                case MaxRewardedInterstitialAdUnit maxRewardInter:
                    maxRewardInter.completedCallback = onCompleted;
                    return unit;
            }

            return unit;
        }

        public static AdUnit OnSkipped(this AdUnit unit, Action onSkipped)
        {
            switch (unit)
            {
                case AdmobRewardAdUnit admobReward:
                    admobReward.skippedCallback = onSkipped;
                    return unit;
                case AdmobRewardedInterstitialAdUnit admobRewardInter:
                    admobRewardInter.skippedCallback = onSkipped;
                    return unit;
                case MaxRewardAdUnit maxReward:
                    maxReward.skippedCallback = onSkipped;
                    return unit;
                case MaxRewardedInterstitialAdUnit maxRewardInter:
                    maxRewardInter.skippedCallback = onSkipped;
                    return unit;
            }

            return unit;
        }
    }
}