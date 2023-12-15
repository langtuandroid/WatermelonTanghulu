using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

public class RESTfulApi
{
    public static int SuccessCount;
    public static int FailCount;
    public static float ResponseAccTime;

    public static IEnumerator Get(string url, string token, UnityAction<bool, string> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false, null);
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            DateTime sendTime = DateTime.Now;

            webRequest.SetRequestHeader("Content-Type", "application/json");
            if (token != null) webRequest.SetRequestHeader("Authorization", "Bearer " + token);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error url : " + url + " error : " + webRequest.error + " code : " + webRequest.responseCode);
                callback(false, webRequest.responseCode.ToString());

                FailCount++;
            }
            else
            {
                Debug.Log("Get Response: <color=yellow>" + webRequest.downloadHandler.text + "</color>");
                callback(true, webRequest.downloadHandler.text);

                SuccessCount++;
            }

            TimeSpan durationTime = DateTime.Now - sendTime;
            ResponseAccTime += Convert.ToSingle(durationTime.TotalSeconds);
        }
    }

    // public static IEnumerator Post(string url, string token, string paramsJson, UnityAction<bool, string> callback)
    // {
    //     Debug.Log("<color=yellow>Post: " + url + "</color> params: " + paramsJson);
    //     if (Application.internetReachability == NetworkReachability.NotReachable)
    //     {
    //         callback(false, null);
    //         yield break;
    //     }
    //
    //     using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
    //     {
    //         DateTime sendTime = DateTime.Now;
    //         
    //         webRequest.url = url;
    //
    //         byte[] myData = System.Text.Encoding.UTF8.GetBytes(paramsJson);
    //         webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(myData);
    //         webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //         webRequest.SetRequestHeader("Content-Type", "application/json");
    //         if (token != null)
    //             webRequest.SetRequestHeader("Authorization", "Bearer " + token);
    //         yield return webRequest.SendWebRequest();
    //
    //         if (webRequest.isNetworkError || webRequest.isHttpError)
    //         {
    //             Debug.LogError("Error: " + webRequest.error + " " + webRequest.responseCode + " " + webRequest.url);
    //             if (callback != null)
    //                 callback(false, webRequest.responseCode.ToString());
    //
    //             FailCount++;
    //         }
    //         else
    //         {
    //             Debug.Log(":Received: " + webRequest.downloadHandler.text);
    //             if (callback != null)
    //                 callback(true, webRequest.downloadHandler.text);
    //             
    //             SuccessCount++;
    //         }
    //         
    //         TimeSpan durationTime = DateTime.Now - sendTime;
    //         ResponseAccTime += Convert.ToSingle(durationTime.TotalSeconds);
    //     }
    // }

    [Serializable]
    public class EncryptReq
    {
        public string k;
    }

    //????? ????
    private static readonly string SECRET_KEY = "abcdefghij123456";
    private static readonly string SECRETKEY = "abcdefghij123456";
    private static readonly string SecretKey = "abcdefghij123456";

    //private static readonly string KEY = StringConst.GetSecretKey();
    private static readonly string KEY = "StringConst.GetSecretKey()";

    public static string EncryptString(string _value)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(_value);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.ECB;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream();

        ICryptoTransform encryptor = myRijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encryptBytes = memoryStream.ToArray();

        string encryptString = Convert.ToBase64String(encryptBytes);

        cryptoStream.Close();
        memoryStream.Close();

        return encryptString;
    }
    
    
    public static string DecryptString(string _value)
    {
        byte[] encryptBytes = Convert.FromBase64String(_value);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.ECB;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream(encryptBytes);

        ICryptoTransform decryptor = myRijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainBytes = new byte[encryptBytes.Length];

        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

        cryptoStream.Close();
        memoryStream.Close();

        return plainString;
    }

public static IEnumerator Post(string url, string token, string paramsJson, UnityAction<bool, string> callback)
    {
        Debug.Log("<color=yellow>Post: " + url + "</color> params: " + paramsJson);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false, null);
            yield break;
        }
        // else if (NetworkManager.Instance.Closed)
        // {
        //     callback(false, null);
        //     yield break;
        // }

        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            DateTime sendTime = DateTime.Now;
            webRequest.url = url;
            //bool isOldVersion = NumberUtil.GetVersionAsNumber() <= 1047;
            bool isOldVersion = true;
            if (url.Contains("https://unknown-login.bm.cookappsgames.com") || url.Contains("https://game.bm.py.cookapps.com/unknown-login/user/list") || isOldVersion)
            {
                EncryptReq enReq = new EncryptReq();
                enReq.k = EncryptString(paramsJson);
                string jsonValue = JsonConvert.SerializeObject(enReq);
                // Debug.Log("<color=yellow>Post_AES: " + url + "</color> params: " + jsonValue);
                byte[] myData = System.Text.Encoding.UTF8.GetBytes(jsonValue);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(myData);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (token != null)
                    webRequest.SetRequestHeader("Authorization", "Bearer " + token);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error + " " + webRequest.responseCode + " " + webRequest.url);
                    if (callback != null)
                        callback(false, webRequest.responseCode.ToString());

                    FailCount++;
                }
                else
                {
                    // Debug.Log(":Received: " + webRequest.downloadHandler.text);
                    // if (callback != null)
                    //     callback(true, webRequest.downloadHandler.text);
                    string responseText = DecryptString(webRequest.downloadHandler.text);
                    Debug.Log(":Received: " + responseText);
                    if (callback != null)
                        callback(true, responseText);

                    SuccessCount++;
                }
            }
            else
            {
                
                EncryptReq enReq = new EncryptReq();
                enReq.k = EncryptString(paramsJson);
                string jsonValue = JsonConvert.SerializeObject(enReq);
                Debug.Log("<color=yellow>Post_AES: " + url + "</color> params: " + jsonValue);
                byte[] myData = System.Text.Encoding.UTF8.GetBytes(jsonValue);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(myData);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (token != null)
                    webRequest.SetRequestHeader("Authorization", "Bearer " + token);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error + " " + webRequest.responseCode + " " + webRequest.url);
                    if (callback != null)
                        callback(false, webRequest.responseCode.ToString());

                    FailCount++;
                }
                else
                {
                    // Debug.Log(":Received: " + webRequest.downloadHandler.text);
                    // if (callback != null)
                    //     callback(true, webRequest.downloadHandler.text);
                    string responseText = DecryptString_unzip(webRequest.downloadHandler.text);
                    Debug.Log(":Received: " + responseText);
                    if (callback != null)
                        callback(true, responseText);

                    SuccessCount++;
                }
            }
            

            TimeSpan durationTime = DateTime.Now - sendTime;
            ResponseAccTime += Convert.ToSingle(durationTime.TotalSeconds);
        }
    }

    public static IEnumerator Put(string url, string token, string paramsJson, UnityAction<bool, string> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false, null);
            yield break;
        }

        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
        {
            byte[] myData = System.Text.Encoding.UTF8.GetBytes(paramsJson);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(myData);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            if (token != null)
                webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error + " " + webRequest.responseCode);
                if (callback != null)
                    callback(false, webRequest.responseCode.ToString());

                FailCount++;
            }
            else
            {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                if (callback != null)
                    callback(true, webRequest.downloadHandler.text);

                SuccessCount++;
            }
        }
    }

    public static IEnumerator Delete(string url, string token, string paramsJson, UnityAction<bool, string> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false, null);
            yield break;
        }

        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbDELETE))
        {
            byte[] myData = System.Text.Encoding.UTF8.GetBytes(paramsJson);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(myData);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            if (token != null)
                webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error + " " + webRequest.responseCode);
                if (callback != null)
                    callback(false, webRequest.responseCode.ToString());

                FailCount++;
            }
            else
            {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                if (callback != null)
                    callback(true, webRequest.downloadHandler.text);

                SuccessCount++;
            }
        }
    }
    
    public static string DecryptString_unzip(string _value)
    {
        byte[] encryptBytes = Convert.FromBase64String(_value);
        Debug.Log("string length = " + _value.Length);
        Debug.Log("encryptBytes length = " + encryptBytes.Length);
        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.ECB;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream(encryptBytes);

        ICryptoTransform decryptor = myRijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainBytes = new byte[encryptBytes.Length];

        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);
        
        Debug.Log("plainCount = " + plainCount);
        using (System.IO.MemoryStream output = new System.IO.MemoryStream())
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(plainBytes))
            using (System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
            {
                 sr.CopyTo(output);
            }

            string str = Encoding.UTF8.GetString(output.GetBuffer(), 0, (int)output.Length);
            Debug.Log("str.Length = " + str.Length);
            return str;
        }
    }
}