using System.Collections;
using System.Collections.Generic;
using CookApps.Admob;
using UnityEngine;

public enum AdsType
{
    BANNER,
    INTERSTITIAL
}
public class Ads : GameObjectSingleton<Ads>
{
    public bool isAds = false;
    public bool isClearAds = false;
    protected override void Awake()
    {
        base.Awake();
        CAppAdmob.Banner.OnLoaded += OnLoadedBanner;
        CAppAdmob.Rewarded.OnLoaded += OnLoadedRewarded;
    }

    private void OnLoadedBanner()
    {
        // 배너 광고 로딩 성공
        Debug.Log("배너광고 로딩 성공");
    }

    private void OnLoadedRewarded()
    {
        // 보상형 광고 로딩 성공
        Debug.Log("전면광고 로딩 성공");
    }


    public void ActiveAds(AdsType adsType, bool isActive)
    {
        if (adsType == AdsType.BANNER)
        {
            // 배너 광고 로딩 되었는가?
            if(CAppAdmob.Banner.IsLoaded)
            {
                switch (isActive)
                {
                    case true:
                        // 배너 광고 Show
                        CAppAdmob.Banner.Show();
                        break;
                    case false:
                        // 배너 광고 Hide
                        CAppAdmob.Banner.Hide();
                        break;
                }
            }
        }else if (adsType == AdsType.INTERSTITIAL)
        {
            // 전면 광고 로딩 되었는가?
            if(CAppAdmob.Interstitial.IsLoaded)
            {
                switch (isActive)
                {
                    case true:
                        // 전면 광고 Show
                        CAppAdmob.Interstitial.Show();
                        break;
                    case false:
                        break;
                }
            }
        }
        
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        CAppAdmob.Banner.OnLoaded -= OnLoadedBanner;
        CAppAdmob.Rewarded.OnLoaded -= OnLoadedRewarded;
    }
}
