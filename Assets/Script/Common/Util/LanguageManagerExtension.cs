using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class LanguageManagerExtension
{
    #region UGUIText

    public static void ToLocalize(this UnityEngine.UI.Text text, string tokenOrText, params object[] args)
    {
        // if (text.font == null)
        //     text.font = LanguageManager.Instance.GetFont();
        // else if(text.font.name != LanguageManager.Instance.GetFont().name)
        //     text.font = LanguageManager.Instance.GetFont();
        //
        // var data = string.IsNullOrEmpty(tokenOrText) ? null : SpecDataManager.Instance.GetLanguageData(tokenOrText);
        //
        // if (args.Length > 0 && !string.IsNullOrEmpty(tokenOrText))
        // {
        //     text.text = string.Format(data == null ? tokenOrText : data.desc, args); // not token
        // }
        // else
        // {
        //     if(data == null)
        //         text.text = tokenOrText; // not token
        //     else
        //         text.text = data.desc;
        // }
    }
    
    public static void ToLocalize(this UnityEngine.UI.Text text, int value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this UnityEngine.UI.Text text, float value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this UnityEngine.UI.Text text, long value)
    {
        text.ToLocalize(value.ToString());
    }
    
    #endregion
    
    #region TextMeshPro
    

    public static void ToLocalize(this TextMeshPro text, string tokenOrText, params object[] args)
    {
        // if (text.font == null)
        //     text.font = LanguageManager.Instance.GetTextMeshProFont();
        // else if(text.font.name != LanguageManager.Instance.GetTextMeshProFont().name)
        //     text.font = LanguageManager.Instance.GetTextMeshProFont();
        //
        // var data = string.IsNullOrEmpty(tokenOrText) ? null : SpecDataManager.Instance.GetLanguageData(tokenOrText);
        //
        // if (args.Length > 0 && !string.IsNullOrEmpty(tokenOrText))
        // {
        //     text.text = string.Format(data == null ? tokenOrText : data.desc, args); // not token
        // }
        // else
        // {
        //     if(data == null)
        //         text.text = tokenOrText; // not token
        //     else
        //         text.text = data.desc;
        // }
    }
    
    public static void ToLocalize(this TextMeshPro text, int value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this TextMeshPro text, float value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this TextMeshPro text, long value)
    {
        text.ToLocalize(value.ToString());
    }
    
    #endregion
    
    #region TextMeshProUGUI
    

    public static void ToLocalize(this TextMeshProUGUI text, string tokenOrText, params object[] args)
    {
        // if (text.font == null)
        //     text.font = LanguageManager.Instance.GetTextMeshProFont();
        // else if(text.font.name != LanguageManager.Instance.GetTextMeshProFont().name)
        //     text.font = LanguageManager.Instance.GetTextMeshProFont();
        //
        // var data = string.IsNullOrEmpty(tokenOrText) ? null : SpecDataManager.Instance.GetLanguageData(tokenOrText);
        //
        // if (args.Length > 0 && !string.IsNullOrEmpty(tokenOrText))
        // {
        //     text.text = string.Format(data == null ? tokenOrText : data.desc, args); // not token
        // }
        // else
        // {
        //     if(data == null)
        //         text.text = tokenOrText; // not token
        //     else
        //         text.text = data.desc;
        // }
    }
    
    public static void ToLocalize(this TextMeshProUGUI text, int value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this TextMeshProUGUI text, float value)
    {
        text.ToLocalize(value.ToString());
    }
    
    public static void ToLocalize(this TextMeshProUGUI text, long value)
    {
        text.ToLocalize(value.ToString());
    }
    
    #endregion
}
