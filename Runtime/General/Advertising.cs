using System.Collections;
using System.Collections.Generic;
#if VIRTUESKY_ADS && VIRTUESKY_ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
using UnityEngine;


namespace VirtueSky.Ads
{
    public class Advertising : MonoBehaviour
    {
        public bool dontDestroyOnLoad = true;
        public static Advertising Instance;
        private IEnumerator autoLoadAdCoroutine;
        private float _lastTimeLoadInterstitialAdTimestamp = DEFAULT_TIMESTAMP;
        private float _lastTimeLoadRewardedTimestamp = DEFAULT_TIMESTAMP;
        private float _lastTimeLoadRewardedInterstitialTimestamp = DEFAULT_TIMESTAMP;
        private float _lastTimeLoadAppOpenTimestamp = DEFAULT_TIMESTAMP;
        private const float DEFAULT_TIMESTAMP = -1000;

        private AdClient currentAdClient;
        private AdSettings adSettings;

        private void Awake()
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            adSettings = AdSettings.Instance;
#if VIRTUESKY_IAP
            VirtueSky.Iap.IapManager.OnShowIapNativePopupEvent += OnChangePreventDisplayOpenAd;
#endif
            if (adSettings.EnableGDPR)
            {
#if VIRTUESKY_ADMOB
                InitGDPR();
#endif
            }
            else
            {
                InitAdClient();
            }
        }

        void InitAdClient()
        {
            switch (adSettings.CurrentAdNetwork)
            {
                case AdNetwork.Max:
                    currentAdClient = new MaxAdClient();
                    break;
                case AdNetwork.Admob:
                    currentAdClient = new AdmobClient();
                    break;
            }

            currentAdClient.SetupAdSettings(adSettings);
            currentAdClient.Initialize();
            Debug.Log("currentAdClient: " + currentAdClient);
            InitAutoLoadAds();
        }

        private void InitAutoLoadAds()
        {
            if (autoLoadAdCoroutine != null) StopCoroutine(autoLoadAdCoroutine);
            autoLoadAdCoroutine = IeAutoLoadAll();
            StartCoroutine(autoLoadAdCoroutine);
        }

        IEnumerator IeAutoLoadAll(float delay = 0)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            while (true)
            {
                AutoLoadInterAds();
                AutoLoadRewardAds();
                AutoLoadRewardInterAds();
                AutoLoadAppOpenAds();
                yield return new WaitForSeconds(adSettings.AdCheckingInterval);
            }
        }

        private void OnChangePreventDisplayOpenAd(bool state)
        {
            AdStatic.isShowingAd = state;
        }

        #region Method Load Ads

        void AutoLoadInterAds()
        {
            if (Time.realtimeSinceStartup - _lastTimeLoadInterstitialAdTimestamp <
                adSettings.AdLoadingInterval) return;
            currentAdClient.LoadInterstitial();
            _lastTimeLoadInterstitialAdTimestamp = Time.realtimeSinceStartup;
        }

        void AutoLoadRewardAds()
        {
            if (Time.realtimeSinceStartup - _lastTimeLoadRewardedTimestamp <
                adSettings.AdLoadingInterval) return;
            currentAdClient.LoadRewarded();
            _lastTimeLoadRewardedTimestamp = Time.realtimeSinceStartup;
        }

        void AutoLoadRewardInterAds()
        {
            if (Time.realtimeSinceStartup - _lastTimeLoadRewardedInterstitialTimestamp <
                adSettings.AdLoadingInterval) return;
            currentAdClient.LoadRewardedInterstitial();
            _lastTimeLoadRewardedInterstitialTimestamp = Time.realtimeSinceStartup;
        }

        void AutoLoadAppOpenAds()
        {
            if (Time.realtimeSinceStartup - _lastTimeLoadAppOpenTimestamp <
                adSettings.AdLoadingInterval) return;
            currentAdClient.LoadAppOpen();
            _lastTimeLoadAppOpenTimestamp = Time.realtimeSinceStartup;
        }

        #endregion

        #region Admob GDPR

#if VIRTUESKY_ADMOB
        private void InitGDPR()
        {
#if !UNITY_EDITOR
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            string deviceIDUpperCase = deviceID.ToUpper();

            Debug.Log("TestDeviceHashedId = " + deviceIDUpperCase);
            var request = new ConsentRequestParameters { TagForUnderAgeOfConsent = false };
            if (adSettings.EnableGDPRTestMode)
            {
                List<string> listDeviceIdTestMode = new List<string>();
                listDeviceIdTestMode.Add(deviceIDUpperCase);
                request.ConsentDebugSettings = new ConsentDebugSettings
                {
                    DebugGeography = DebugGeography.EEA,
                    TestDeviceHashedIds = listDeviceIdTestMode
                };
            }

            ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
        }

        private void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                Debug.Log("error consentError = " + consentError);
                return;
            }

            ConsentForm.LoadAndShowConsentFormIfRequired(
                (FormError formError) =>
                {
                    if (formError != null)
                    {
                        Debug.Log("error consentError = " + consentError);
                        return;
                    }

                    Debug.Log("ConsentStatus = " + ConsentInformation.ConsentStatus.ToString());
                    Debug.Log("CanRequestAds = " + ConsentInformation.CanRequestAds());


                    if (ConsentInformation.CanRequestAds())
                    {
                        MobileAds.RaiseAdEventsOnUnityMainThread = true;
                        InitAdClient();
                    }
                }
            );
        }

        public void LoadAndShowConsentForm()
        {
            Debug.Log("LoadAndShowConsentForm Start!");

            ConsentForm.Load((consentForm, loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("error loadError = " + loadError);
                    return;
                }


                consentForm.Show(showError =>
                {
                    if (showError != null)
                    {
                        Debug.Log("error showError = " + showError);
                        return;
                    }
                });
            });
        }

        public void ShowPrivacyOptionsForm()
        {
            Debug.Log("Showing privacy options form.");

            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                if (showError != null)
                {
                    Debug.LogError("Error showing privacy options form with error: " + showError.Message);
                }
            });
        }
#endif

        #endregion

#if VIRTUESKY_MAX
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && adSettings.MaxAppOpenAdUnit.autoShow && !AdStatic.isShowingAd)
                (currentAdClient as MaxAdClient)?.ShowAppOpen();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialize()
        {
            if (AdSettings.Instance == null) return;
            if (AdSettings.Instance.RuntimeAutoInit)
            {
                var ads = new GameObject("Advertising");
                ads.AddComponent<Advertising>();
                DontDestroyOnLoad(ads);
            }
        }

        #region Public API

        public static AdUnit BannerAd => Instance.currentAdClient.BannerAdUnit();
        public static AdUnit InterstitialAd => Instance.currentAdClient.InterstitialAdUnit();
        public static AdUnit RewardAd => Instance.currentAdClient.RewardAdUnit();
        public static AdUnit RewardedInterstitialAd => Instance.currentAdClient.RewardedInterstitialAdUnit();
        public static AdUnit AppOpenAd => Instance.currentAdClient.AppOpenAdUnit();

        #endregion
    }
}