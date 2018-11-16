using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdScript : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //Request Ad
        RequestInterstitialAds();
    }

    public void showInterstitialAd()
    {
        //Show Ad
        if (interstitial.IsLoaded())
        {
            interstitial.Show();

            FindObjectOfType<GameManager>().PauseMusic(true);

            Debug.Log("SHOW AD XXX");
        }

    }

    InterstitialAd interstitial;
    private void RequestInterstitialAds()
    {
        // ***Test***

        /*string adID = "ca-app-pub-3940256099942544/1033173712"; 

        #if UNITY_ANDROID
                string adUnitId = adID;
        #elif UNITY_IOS
                string adUnitId = adID;
        #else
                string adUnitId = adID;
        #endif

        interstitial = new InterstitialAd(adUnitId);

        AdRequest request = new AdRequest.Builder()
        .AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
        .AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")  // My test device.
        .Build();*/

        //***End Test***

        //***Production***

        #if UNITY_ANDROID
            string appID = "ca-app-pub-7050819570050866~4842209208";
        #else
            string appID = "unexpected platform";
        #endif

        MobileAds.Initialize(appID);

        string adUnitID = "ca-app-pub-7050819570050866/9328249121";

        interstitial = new InterstitialAd(adUnitID);
        AdRequest request = new AdRequest.Builder().Build();

        //***End Production

        //Register Ad Close Event
        interstitial.OnAdClosed += Interstitial_OnAdClosed;

        // Load the interstitial with the request.
        interstitial.LoadAd(request);

        Debug.Log("AD LOADED XXX");
    }

    //Ad Close Event
    private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
    {
        FindObjectOfType<GameManager>().PauseMusic(false);
    }
}
