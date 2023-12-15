using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaDetection : MonoBehaviour
{
    public static SafeAreaDetection Instance; 
    public delegate void SafeAreaChanged(Rect safeArea);

    public static event SafeAreaChanged OnSafeAreaChanged;

    public static Rect SafeArea { get; private set; }

    protected void Awake()
    {
        Instance = this;
        SafeArea = Screen.safeArea;
        OnSafeAreaChanged?.Invoke(SafeArea);
    }

    private void Update()
    {
        if (SafeArea != Screen.safeArea)
        {
            SafeArea = Screen.safeArea;
            OnSafeAreaChanged?.Invoke(SafeArea);
        }
    }
}