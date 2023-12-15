using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Pref
{
    LANGUAGE,
    BGM,
    SFX,
    SAFE_MODE_T,
    USE_SAFE_MODE,
    SPEC_VERSION,
    SPEC_DATA,
    LAN_VERSION,
    LAN_SAVE,
    LAN_DATA,
    GUEST_ID,
    SOCIAL_ID,
    SHOW_FX,
    SHOW_DAMAGE,
    SHOW_INTRO,
    UID,
    IG_AUTO_SKILL,
    IG_SPEED_UP,
    LAST_SCENE,
    FPS,
    SPLASH_IDFA_CHECK,
    PATCH_NOTE,
    TEMP_UID,
    IS_TAB_MVP,
    IS_SHOW_TAB,
    FIRST_PLAY,
    GUIDE_SHOW,
    EXIST_POST,
    SHOW_EQUIPPED,
    USING_MAX_LEVELUP,
    LAST_STAGE,
    LAST_MODE,
    BGM_V,
    SFX_V,
    VOX_V,
    IS_CAMERA_SHAKE,
    APPLE_ID,
    SORT_LEVEL,
    SORT_GRADE,
    SORT_FAVORITE,
    SELECT_REGION,
    SELECT_REGION_URL,
    AUDIO_LANGUAGE,
    TERM_NOTICE,
    SELECT_SOCIAL,
    FIRST_ENHANCE_6,
    IS_RUNE_FX,
    FIRST_LOGIN_GAMECENTER,
    IS_AUTO_WEAPON_GRIND,
    FIRST_GET_ARTIFACT_BOX,
    IS_SKILL_GUIDE,
    CHAT_BLOCK_LIST,
    LAST_CHAT_COUNT,
}

public class Preference
{
    public static List<T> LoadListPreference<T>(Pref pref)
    {
        List<T> ret = null;
        try
        {
            string json = UnityEngine.PlayerPrefs.GetString(pref.ToString());

            if (string.IsNullOrEmpty(json))
            {
                return new List<T>();
            }

            ret = JsonConvert.DeserializeObject<List<T>>(json);
        }
        catch
        {
            return new List<T>();
        }

        return ret;
    }

    public static List<int> LoadPreference(Pref pref, List<int> defaultValue)
    {
        string json = UnityEngine.PlayerPrefs.GetString(pref.ToString());
        int[] list = BiniLab.JsonHelper.ListFromJson<int>(json);
        if (list == null) return defaultValue;
        return new List<int>(list);
    }

    public static void SavePreference<T>(Pref pref, List<T> value)
    {
        string json = JsonConvert.SerializeObject(value);
        UnityEngine.PlayerPrefs.SetString(pref.ToString(), json);
        UnityEngine.PlayerPrefs.Save();
    }

    public static void SavePreference(Pref pref, List<int> value)
    {
        string json = BiniLab.JsonHelper.ListToJson<int>(value.ToArray());
        UnityEngine.PlayerPrefs.SetString(pref.ToString(), json);
        UnityEngine.PlayerPrefs.Save();
    }

    public static bool LoadPreference(Pref pref, bool defaultValue, int server = 0)
    {
        return LoadPreference(pref, defaultValue ? 1 : 0, server) > 0;
    }

    public static void SavePreference(Pref pref, bool value, int server = 0)
    {
        SavePreference(pref, value ? 1 : 0, server);
    }

    public static int LoadPreference(Pref pref, int defaultValue, int server = 0)
    {
        int returnInt = UnityEngine.PlayerPrefs.GetInt((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), defaultValue);
        // if (pref != Pref.FPS)
        //     Debug.Log("Preference Loaded " + returnInt);
        return returnInt;
    }

    public static void SavePreference(Pref pref, int value, int server = 0)
    {
        UnityEngine.PlayerPrefs.SetInt((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), value);
        UnityEngine.PlayerPrefs.Save();
        // Debug.Log("Preference Saved " + value);
    }

    public static string LoadPreference(Pref pref, string defaultValue, int server = 0)
    {
        string returnStr = UnityEngine.PlayerPrefs.GetString((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), defaultValue);
        // Debug.Log("Preference Loaded " + returnStr);
        return returnStr;
    }

    public static void SavePreference(Pref pref, string value, int server = 0)
    {
        UnityEngine.PlayerPrefs.SetString((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), value);
        UnityEngine.PlayerPrefs.Save();
        // Debug.Log("Preference Saved " + value);
    }

    public static float LoadPreference(Pref pref, float defaultValue, int server = 0)
    {
        float returnFloat = UnityEngine.PlayerPrefs.GetFloat((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), defaultValue);
        // Debug.Log("Preference Loaded " + returnFloat);
        return returnFloat;
    }

    public static void SavePreference(Pref pref, float value, int server = 0)
    {
        UnityEngine.PlayerPrefs.SetFloat((server > 1 ? server.ToString() : string.Empty) + pref.ToString(), value);
        UnityEngine.PlayerPrefs.Save();
        // Debug.Log("Preference Saved " + value);
    }

    public static void Clear()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        UnityEngine.PlayerPrefs.Save();
    }
}