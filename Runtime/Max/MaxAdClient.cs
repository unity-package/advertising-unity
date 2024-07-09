using VirtueSky.Core;

namespace VirtueSky.Ads
{
    public class MaxAdClient : AdClient
    {
        public override void Initialize()
        {
#if VIRTUESKY_ADS && VIRTUESKY_MAX
            MaxSdk.SetSdkKey(adSettings.SdkKey);
            MaxSdk.InitializeSdk();
            MaxSdk.SetIsAgeRestrictedUser(adSettings.ApplovinEnableAgeRestrictedUser);
            adSettings.MaxBannerAdUnit.Init();
            adSettings.MaxInterstitialAdUnit.Init();
            adSettings.MaxRewardAdUnit.Init();
            adSettings.MaxAppOpenAdUnit.Init();
            adSettings.MaxRewardedInterstitialAdUnit.Init();
            App.AddPauseCallback(OnAppStateChange);
            LoadInterstitial();
            LoadRewarded();
            LoadRewardedInterstitial();
            LoadAppOpen();
#endif
        }
#if VIRTUESKY_ADS && VIRTUESKY_MAX
        private void OnAppStateChange(bool pauseStatus)
        {
            if (!pauseStatus && adSettings.MaxAppOpenAdUnit.autoShow && !AdStatic.isShowingAd) ShowAppOpen();
        }
#endif
    }
}