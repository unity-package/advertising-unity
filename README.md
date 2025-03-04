<p align="left">
  <a>
    <img alt="Made With Unity" src="https://img.shields.io/badge/made%20with-Unity-57b9d3.svg?logo=Unity">
  </a>
  <a>
    <img alt="License" src="https://img.shields.io/github/license/unity-package/advertising-unity?logo=github">
  </a>
  <a>
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/unity-package/advertising-unity?logo=Mapbox&color=orange">
  </a>
  <a>
    <img alt="Repo Size" src="https://img.shields.io/github/repo-size/unity-package/advertising-unity?logo=VirtualBox">
  </a>
  <a>
    <img alt="Last Release" src="https://img.shields.io/github/v/release/unity-package/advertising-unity?include_prereleases&logo=Dropbox&color=yellow">
  </a>
</p>

## What
### Show ads tool for unity games (support for Max-Applovin, Google Mobile Ads and IronSource-UnityLevelPlay)

## How To Install

### Add the line below to `Packages/manifest.json`

for version `1.1.6`
```csharp
"com.wolf-org.advertising":"https://github.com/unity-package/advertising-unity.git#1.1.6",
```
dependency `extensions-unity-1.0.5`
```csharp
"com.wolf-org.extensions":"https://github.com/unity-package/extensions-unity.git#1.0.5",
```

Install `app-tracking-unity` and add define symbol `VIRTUESKY_TRACKING` if you need ad revenue tracking
```csharp
"com.wolf-org.tracking":"https://github.com/unity-package/app-tracking-unity.git#1.0.3",
```

## Use

- Use via MenuItem `Unity-Common` > `AdSettings` or shortcut `Ctrl + E / Command + E` to open `AdSettings`


![Unity_SkbG2UOp9G](https://github.com/wolf-package/advertising/assets/126542083/ce4bddc9-61ae-4b62-b7bc-0588d0eacff6)



- Here, select `Ad Network` and enter the ad unit id you want to use, don't forget add `Define Symbol`.

- Add `Scripting Define Symbols` in `Project Settings` > `Player` > `Other Settings`
    - Applovin: `VIRTUESKY_ADS` and `VIRTUESKY_APPLOVIN`
    - Admob: `VIRTUESKY_ADS` and `VIRTUESKY_ADMOB`
    - IronSource: `VIRTUESKY_ADS` and `VIRTUESKY_IRONSOURCE`
  
- If you use `Runtime auto init`, `Advertising` will be created automatically when you load the scene. Conversely, you would attach `Advertising` to the GameObject in the scene so that the ads can be loaded

![Screenshot 2024-05-16 174541](https://github.com/wolf-package/unity-common/assets/102142404/451834ff-91e3-4ccf-90bd-b0c1d4b4f440)


- Demo API Show Ads

```csharp

    public void ShowBanner()
    {
        Advertising.BannerAd.Show();
    }

    public void HideBanner()
    {
        Advertising.BannerAd.HideBanner();
    }

    public void ShowInter()
    {
        Advertising.InterstitialAd.Show().OnCompleted(() =>
        {
            // handle show inter completed
        });
    }

    public void ShowReward()
    {
        Advertising.RewardAd.Show().OnCompleted(() =>
        {
            // handle show reward completed
        }).OnSkipped(() =>
        {
            // handle skip reward
        });
    }

    public void ShowRewardInter()
    {
        Advertising.RewardedInterstitialAd.Show().OnCompleted(() => { });
    }

    public void ShowAppOpen()
    {
        Advertising.AppOpenAd.Show();
    }

```


