using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using Scripts.Session;

public class RewardAdsManager : MonoBehaviour
{
    private RewardedAd rewardedAd;

    public bool isTestMode;
    private string adUnitId;
    // Start is called before the first frame update
    void Start()
    {
        if (isTestMode)
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif
        }
        else
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-2691806243750775/2494667132";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif
        }


        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        OnLoadAd();

    }


    private void OnLoadAd()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }



    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        popupMessage.Create("ระบบ", "ไม่สามารถดู video ads ได้ในขณะนี้");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        OnLoadAd();
    }

    public void HandleUserEarnedReward(object sender, EventArgs args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Receive();
        });
    }

    private async void Receive()
    {
        var resp = await GameApi.ClaimVideoAdsReward();
        popupMessage.Create("ระบบ", resp, Refresh);
    }
    private void Refresh()
    {
        NakamaSessionManager.Instance.Notify();
    }
    public void UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            CheckAds();
        }
        else
        {
            OnLoadAd();
        }
    }

    private async void CheckAds()
    {
        try
        {
            Nakama.IApiRpc res = await GameApi.CheckAdAvaliable();
            if (res.Payload.Equals("true"))
            {
                this.rewardedAd.Show();
            }
            else
            {
                popupMessage.Create("ระบบ", res.Payload);
            }
        }
        catch (Exception)
        {
            popupMessage.Create("ระบบ", "ไม่สามารถดู ads ได้ในขณะนี้");
        }
    }
}
