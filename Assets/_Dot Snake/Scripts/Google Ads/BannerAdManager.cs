using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class BannerAdManager : MonoBehaviour
{
    #if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";

    #elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
    #endif

    public BannerView _bannerView;

    private void Start() => CreateBannerView();

    public void CreateBannerView()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        
        ListenToAdEvents();
        LoadAd();
    }

    private void OnDestroy()
    {
        if (_bannerView != null)
            _bannerView.Destroy();
    }

    public void LoadAd()
    {
        var adRequest = new AdRequest.Builder().Build();

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : " + _bannerView.GetResponseInfo());
        };

        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };

        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };

        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };

        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };

        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };

        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
}
