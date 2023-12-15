using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using ClockStone;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _scenario;
    
    [SerializeField] private Text _bestScoreText;

    [SerializeField] private GameObject _bestScoreGrp;
    [SerializeField] private GameObject _helpButton;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _skipButton;
    [SerializeField] private GameObject _leaderBoardButton;
    private PlayableDirector _playableDirector;
    
    private void Start()
    {
        InitData();
        SoundManager2.Instance.BgmPlaySound("Title");
    }

    private void OnDestroy()
    {
        if(_playableDirector != null)
            _playableDirector.stopped -= OnStoppedScenario;
    }

    private void InitData()
    {
        _bestScoreText.text = $"{DataManager.Instance.UserData.BestScore}";
        
        _bestScoreGrp.SetActive(true);
        _startButton.SetActive(true);
        _helpButton.SetActive(true);
        _leaderBoardButton.SetActive(true);
        _skipButton.SetActive(false);
        
        Ads.Instance.isClearAds = false;
        TimeHelper.SetTime(TimeType.PLAY);
    }

    public void OnClickStart()
    {
        Destroy(_title);
        
        _playableDirector = Instantiate(_scenario).GetComponentInChildren<PlayableDirector>();
        
        if(_playableDirector.playableGraph.IsValid() == false)
            _playableDirector.Play();

        _playableDirector.stopped -= OnStoppedScenario;
        _playableDirector.stopped += OnStoppedScenario;
        
        _bestScoreGrp.SetActive(false);
        _startButton.SetActive(false);
        _helpButton.SetActive(false);
        _leaderBoardButton.SetActive(false);
        _skipButton.SetActive(true);
        SoundManager2.Instance.SfxPlaySound("Click");
    }

    public void OnClickSkip()
    {
        _playableDirector.Stop();
        SoundManager2.Instance.SfxPlaySound("Click");
    }

    public void OnClickHelp()
    {
        HelpPop.ShowPop();
        SoundManager2.Instance.SfxPlaySound("Click");
    }

    private void OnStoppedScenario(PlayableDirector playableDirector)
    {
        if(playableDirector == _playableDirector)
            SceneManager.LoadScene("Scenes/InGame");
    }

    
}
