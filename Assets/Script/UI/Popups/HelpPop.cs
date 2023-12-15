using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpPop : UEPopup
{
    private static HelpPop Instance;
    
    public void OnClickExit()
    {
        base.Hide();
        SoundManager2.Instance.SfxPlaySound("Click");
        
    }
    public static void ShowPop()
    {
        if (Instance == false)
        {
            Instance = UEPopup.GetInstantiateComponent<HelpPop>();
            Instance.Init();
        }
    }

    private void Init()
    {
        base.Show();
        
    }

}
