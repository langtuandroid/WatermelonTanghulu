using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPop : UEPopup
{
    private static GameOverPop Instance;
    [SerializeField] private GameObject _adsClearButton;
    [SerializeField] private Text _bestScoreText;
    public void OnClickHome()
    {
        base.Hide();
        SoundManager2.Instance.SfxPlaySound("Click");
        SceneManager.LoadScene("Title"); 
    }
    
    public void OnClickAds()
    {
        //광고 버튼 로직
        SoundManager2.Instance.SfxPlaySound("Click");
        
        Ads.Instance.isClearAds = true;
        Ads.Instance.isAds = true;
        Ads.Instance.ActiveAds(AdsType.INTERSTITIAL,true);
        
        base.Hide();
    }

    public static void ShowPop()
    {
        if (Instance == false)
        {
            Instance = UEPopup.GetInstantiateComponent<GameOverPop>();
            Instance.Init();
        }
    }

    private void Init()
    {
        if(Ads.Instance.isClearAds)
            _adsClearButton.SetActive(true);
        else
            _adsClearButton.SetActive(false);
            
        _bestScoreText.text = FruitsManager.Instance.Score.ToString(); 
        
        SoundManager2.Instance.BgmStopSound("Ingame");
        SoundManager2.Instance.SfxPlaySound("GameOver");
        
        base.Show();
       
    }

}
