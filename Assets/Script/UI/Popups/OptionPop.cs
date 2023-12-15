using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionPop : UEPopup
{
    private static OptionPop Instance;

    public void OnClickHome()
    {
        SceneManager.LoadScene("Title");
        SoundManager2.Instance.SfxPlaySound("Click");
    }
    
    public void OnClickExit()
    {
        base.Hide();
        SoundManager2.Instance.SfxPlaySound("Click");
        TimeHelper.SetTime(TimeType.PLAY);
        
    }
    public static void ShowPop()
    {
        if (Instance == false)
        {
            Instance = UEPopup.GetInstantiateComponent<OptionPop>();
            Instance.Init();
        }
    }

    private void Init()
    {
        base.Show();
        
        InGameManager.Instance.isTouchActive = false;
        
        TimeHelper.SetTime(TimeType.STOP);
    }
    
}
