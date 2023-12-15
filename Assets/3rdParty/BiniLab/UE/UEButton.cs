/*********************************************
 * NHN StarFish - UI Extends
 * CHOI YOONBIN
 * 
 *********************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

public enum UEReactionType
{
    None = 0,
    Jelly = 1,
    Punch = 11,
    Punch_Small = 12,
    Custom = 13,
}

public class UEButton : Button
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides MonoBehaviour

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        this.localScale = this.transform.localScale;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!this.tweenScaleX.Completed)
        {
            float delteTime = Time.unscaledDeltaTime;
            this.tweenScaleX.Update(delteTime);
            this.tweenScaleY.Update(delteTime);

            if (tweenScaleXValue.Value != null)
            {
                float scaleX = Mathf.Round(this.tweenScaleXValue.Value / .01f) * .01f;
                float scaleY = Mathf.Round(this.tweenScaleYValue.Value / .01f) * .01f;
                Vector3 scale = new Vector3(scaleX, scaleY, 1f);
                this.transform.localScale = scale;
            }
        }

        if (this.buttonPressed)
        {
            if (this.useContinueClick)
            {
                if (this.continueThreshold > 0)
                {
                    this.continueThreshold -= Time.deltaTime;
                }
                else
                {
                    this.ForceClick();
                    this.continueThreshold = Mathf.Max(0.15f, continue_continue_time - (this.continueCount * 0.04f));
                    this.continueCount++;
                }
            }

            if (long_button_time > _buttonPressTime)
                _buttonPressTime += Time.deltaTime;

            if (_stayAction != null)
                _stayAction(buttonPressed);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!Application.isPlaying) return;

        if (this.localScale == Vector3.zero)
            this.localScale = this.transform.localScale;
        else
            this.transform.localScale = this.localScale;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!Application.isPlaying) return;

        this.tweenScaleX.Kill();
        this.tweenScaleY.Kill();
        this.transform.localScale = this.localScale;
        this.buttonPressed = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public overrides Button

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (this.reactionType == UEReactionType.None) return;
        base.OnPointerDown(eventData);
        this.continueCount = 0;
        this.continueThreshold = inital_continue_time;
        this.clickInvoked = false;
        this.cancelClick = false;
        this.buttonPressed = true;

        switch (this.reactionType)
        {
            case UEReactionType.Jelly:
                this.JellyTweenReady();
                break;
            case UEReactionType.Punch:
                this.punchTweenReady();
                break;
            case UEReactionType.Punch_Small:
                this.punchSmallTweenReady();
                break;
            case UEReactionType.Custom:
                this.customTweenReady();
                if (_downAction != null)
                    _downAction.Invoke(true);
                break;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (this.reactionType == UEReactionType.None) return;
        if (this.cancelClick) return;

        base.OnPointerUp(eventData);
        this.buttonPressed = false;
        switch (this.reactionType)
        {
            case UEReactionType.Jelly:
                this.StartCoroutine(this.JellyTweenStart());
                break;
            case UEReactionType.Punch:
                this.BackToNormal();
                break;
            case UEReactionType.Punch_Small:
                this.BackToNormal();
                break;
            case UEReactionType.Custom:
                this.BackToNormal();
                if (_upAction != null)
                    _upAction.Invoke(true);
                break;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (this.reactionType == UEReactionType.None) return;
        base.OnPointerExit(eventData);

        if (this.buttonPressed)
        {
            this.cancelClick = true;
            this.buttonPressed = false;
            this.BackToNormal();
        }

        if (this.reactionType == UEReactionType.Custom)
            if (_upAction != null)
                _upAction.Invoke(true);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (this.reactionType == UEReactionType.None) return;
        if (!this.UseContinueClick && this.buttonPressed)
        {
            base.OnMove(eventData);
            this.cancelClick = true;
            this.buttonPressed = false;
            this.BackToNormal();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (this.reactionType == UEReactionType.None)
        {
            base.OnPointerClick(eventData);
        }
        else
        {
            this.clickInvoked = false;
            this.ForceClick();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public UEReactionType ReactionType
    {
        get { return this.reactionType; }
        set { this.reactionType = value; }
    }

    public bool UseContinueClick
    {
        get { return this.useContinueClick; }
        set { this.useContinueClick = value; }
    }

    public float ButtonPressTime => _buttonPressTime;

    public float LongButtonTime => long_button_time;

    protected bool ButtonPressed => buttonPressed;

    protected void ResetButtonPressedTime()
    {
        _buttonPressTime = 0;
    }

    public void SetAction(Action<bool> down, Action<bool> up)
    {
        _downAction = down;
        _upAction = up;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private

    private const float inital_continue_time = 0.5f;
    private const float continue_continue_time = 0.15f;
    private const float long_button_time = 1.0f;

    [SerializeField] private Action<bool> _stayAction = null;
    [SerializeField] private bool useContinueClick = false;
    [SerializeField] private UEReactionType reactionType = UEReactionType.None;
    [SerializeField] private Action<bool> _downAction = null;
    [SerializeField] private Action<bool> _upAction = null;

    private Vector3 localScale;

    private SimpleTweener tweenScaleX = new SimpleTweener();
    private TweenLerp<float> tweenScaleXValue;

    private SimpleTweener tweenScaleY = new SimpleTweener();
    private TweenLerp<float> tweenScaleYValue;

    private bool cancelClick = false;
    private bool clickInvoked = false;
    private bool buttonPressed = false;

    private int continueCount = 0;
    private float continueThreshold = 0f;
    private float _buttonPressTime = 0f;

    private void ForceClick()
    {
        if (!this.cancelClick && !this.clickInvoked)
        {
            if (!this.useContinueClick)
                this.clickInvoked = true;
            this.onClick.Invoke();
            _buttonPressTime = 0;
        }
    }

    private void JellyTweenReady()
    {
        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1f, 1.248f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1f, 0.904f);
    }

    private void punchTweenReady()
    {
        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1f, 1.1f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1f, 1.1f);
    }
    
    private void punchSmallTweenReady()
    {
        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1f, 1.05f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1f, 1.05f);
    }
    
    private void customTweenReady()
    {
        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1f, 1f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1f, 1f);
    }

    private void BackToNormal()
    {
        this.tweenScaleX.Reset(0.2f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(this.tweenScaleXValue.Value, 1f);

        this.tweenScaleY.Reset(0.2f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(this.tweenScaleYValue.Value, 1f);
    }

    private IEnumerator JellyTweenStart()
    {
        this.tweenScaleX.Reset(0.1f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1.248f, 0.92f);

        this.tweenScaleY.Reset(0.1f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(0.904f, 1.12f);

        yield return StartCoroutine(this.WaitForRealSeconds(0.15f));

        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(0.92f, 1.112f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1.12f, 0.888f);

        yield return StartCoroutine(this.WaitForRealSeconds(0.15f));

        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1.112f, 0.944f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(0.904f, 1f);

        yield return StartCoroutine(this.WaitForRealSeconds(0.15f));

        this.tweenScaleX.Reset(0.2f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(0.944f, 1.04f);

        this.tweenScaleY.Reset(0.2f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(1f, 0.976f);

        yield return StartCoroutine(this.WaitForRealSeconds(0.2f));

        this.tweenScaleX.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleXValue = this.tweenScaleX.CreateTween(1.04f, 1f);

        this.tweenScaleY.Reset(0.15f, EasingObject.LinearEasing);
        this.tweenScaleYValue = this.tweenScaleY.CreateTween(0.976f, 1f);

        yield break;
    }

    private IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }
}
