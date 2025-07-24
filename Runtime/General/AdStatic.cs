using System;
using UnityEngine;
using VirtueSky.Core;

namespace VirtueSky.Ads
{
    public static class AdStatic
    {
#if VIRTUESKY_ADS
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeBeforeSceneLoad()
        {
            AutoInitialize(CoreEnum.RuntimeAutoInitType.BeforeSceneLoad);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeAfterSceneLoad()
        {
            AutoInitialize(CoreEnum.RuntimeAutoInitType.AfterSceneLoad);
        }

        private static void AutoInitialize(CoreEnum.RuntimeAutoInitType adsRuntimeAutoInitType)
        {
            if (AdSettings.Instance == null) return;
            if (!AdSettings.Instance.RuntimeAutoInit) return;
            if (AdSettings.Instance.RuntimeAutoInitType != adsRuntimeAutoInitType) return;
            var ads = new GameObject("Advertising");
            ads.AddComponent<Advertising>();
            UnityEngine.Object.DontDestroyOnLoad(ads);
        }

#endif
        public static bool IsRemoveAd
        {
#if VIRTUESKY_DATA
            get => VirtueSky.DataStorage.GameData.Get($"{Application.identifier}_removeads", false);
            set => VirtueSky.DataStorage.GameData.Set($"{Application.identifier}_removeads", value);
#else
            get => PlayerPrefs.GetInt($"{Application.identifier}_removeads", 0) > 0;
            set => PlayerPrefs.SetInt($"{Application.identifier}_removeads", value ? 1 : 0);
#endif
        }

        public static DateTime AdClosingTime { get; internal set; }

        internal static bool isShowingAd;

        public static bool IsShowingAd
        {
            get => isShowingAd;
            internal set
            {
                isShowingAd = value;
                if (!value)
                {
                    AdClosingTime = DateTime.Now;
                }
            }
        }

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
                case IronSourceInterstitialAdUnit ironSourceInterstitialAdUnit:
                    ironSourceInterstitialAdUnit.completedCallback = onCompleted;
                    return unit;
                case IronSourceRewardAdUnit ironSourceRewardAdUnit:
                    ironSourceRewardAdUnit.completedCallback = onCompleted;
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
                case IronSourceRewardAdUnit ironSourceRewardAdUnit:
                    ironSourceRewardAdUnit.skippedCallback = onSkipped;
                    return unit;
            }

            return unit;
        }
    }
}