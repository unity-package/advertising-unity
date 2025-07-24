#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
using System;
using GoogleMobileAds.Api;
#endif

namespace VirtueSky.Ads
{
    public class AdmobClient : AdClient
    {
        public override void Initialize()
        {
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This setting makes iOS behave consistently with Android.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            MobileAds.Initialize(initStatus =>
            {
                if (!adSettings.AdmobEnableTestMode) return;
                var configuration = new RequestConfiguration
                    { TestDeviceIds = adSettings.AdmobDevicesTest };
                MobileAds.SetRequestConfiguration(configuration);
            });

            adSettings.AdmobBannerAdUnit.Init();
            adSettings.AdmobInterstitialAdUnit.Init();
            adSettings.AdmobRewardAdUnit.Init();
            adSettings.AdmobRewardedInterstitialAdUnit.Init();
            adSettings.AdmobAppOpenAdUnit.Init();
            adSettings.AdmobNativeOverlayAdUnit.Init();

            RegisterAppStateChange();
            LoadInterstitial();
            LoadRewarded();
            LoadRewardedInterstitial();
            LoadAppOpen();
            LoadNativeOverlay();
#endif
        }

        void LoadNativeOverlay()
        {
            if (!adSettings.AdmobNativeOverlayAdUnit.IsReady())
            {
                adSettings.AdmobNativeOverlayAdUnit.Load();
            }
        }
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
        private void RegisterAppStateChange()
        {
            GoogleMobileAds.Api.AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        void OnAppStateChanged(GoogleMobileAds.Common.AppState state)
        {
            if (state == GoogleMobileAds.Common.AppState.Foreground && adSettings.AdmobAppOpenAdUnit.autoShow)
            {
                if (adSettings.CurrentAdNetwork == AdNetwork.Admob) ShowAppOpen();
            }
        }
#endif
    }
}