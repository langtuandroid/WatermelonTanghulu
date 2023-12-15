/*********************************************
 * NHN StarFish - UI Extends
 * CHOI YOONBIN
 * 
 *********************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UEPopup : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public static

    public static T GetInstantiateComponent<T>() where T : MonoBehaviour
    {
        GameObject go = GameObject.Instantiate(Resources.Load(StringConst.UI_POPUP_PATH + typeof(T).Name) as GameObject) as GameObject;
        T instance = go.GetComponent<T>();
        UEPopup.instance = instance as UEPopup;
        return instance;
    }


    public static T GetInstantiateComponent<T>(string path) where T : MonoBehaviour
    {
        GameObject go = GameObject.Instantiate(Resources.Load(path) as GameObject) as GameObject;
        T instance = go.GetComponent<T>();
        UEPopup.instance = instance as UEPopup;
        return instance;
    }

    public static T GetInstantiateComponent<T>(GameObject prefab) where T : MonoBehaviour
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        T instance = go.GetComponent<T>();
        UEPopup.instance = instance as UEPopup;
        return instance;
    }

    public static void HideAll()
    {
        foreach (UEPopup pop in FindObjectsOfType<UEPopup>())
        {
            pop.Hide();
        }
    }

    public static void HideAllForReset()
    {
        foreach (UEPopup pop in FindObjectsOfType<UEPopup>())
        {
            Destroy(pop.gameObject);
        }
    }

    public static bool HasPopup
    {
        get { return popupCount > 0; }
    }

    public static bool HasCanEscapePopup
    {
        get { return escapePopupCount > 0; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // protected

    protected static int rewardsPopCount = 0;
    protected static int toastPopCount = 0;


    public static int HidePopCount = 0;
    public static int HideMainUICount = 0;
    private static List<(float, UEPopup)> _hideMainPops = new();
    [SerializeField] private bool _isHidePop = false;
    [SerializeField] private bool _hideMainUI = false;
    [SerializeField] private bool _isCountingPop = true;


    protected static UEPopup instance;

    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!this.bodyScaleTweener.Completed)
        {
            this.bodyScaleTweener.Update(Time.unscaledDeltaTime);
            if (this.popBody) this.popBody.localScale = this.bodyScaleTweenValue.Value;
        }

        if (!this.alphaTweener.Completed)
        {
            this.alphaTweener.Update(Time.unscaledDeltaTime);
            if (this.bodyCanvasGroup) this.bodyCanvasGroup.alpha = this.alphaTweenValue.Value;
        }

        durationTime += Time.deltaTime;

        OnEscapeKeyDown();
    }

    // Esc키, Back키 가상화
    protected virtual void OnEscapeKeyDown()
    {
        //if (Application.platform == RuntimePlatform.Android)
#if UNITY_ANDROID
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this._canBackgroundClose && this.countIndex == popupCount &&
                    this.completeShow && this.showing && _canEscapeHide)
                    this.Hide();

                if (this._canBackgroundClose && this.completeShow && this.showing && _canEscapeHide &&
                    this.countIndex > popupCount && this.escapeCountIndex == escapePopupCount)
                {
                    this.Hide();
                }
            }
        }
#endif
    }


    public static void ResetWhenSceneChange()
    {
        escapePopupCount = 0;
        popupCount = 0;
        HideMainUICount = 0;
        HidePopCount = 0;
        _hideMainPops = new();
    }

    private void RefreshHidePops()
    {
        var sortedTuples = _hideMainPops; //ī�޶� depth ������ ����

        if (sortedTuples.Count > 0)
        {
            foreach (var camera in sortedTuples[sortedTuples.Count - 1].Item2.cameras) //������� depth�� ī�޶�� �� ���ش�
            {
                camera.enabled = true;
            }

            for (var i = 0; i < sortedTuples.Count - 1; i++) //������ depth���� ī�޶�� ��� ���ش� 
            {
                foreach (var camera in sortedTuples[i].Item2.cameras)
                {
                    camera.enabled = false;
                }
            }
        }
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    protected static int popupCount = 0;
    protected static int escapePopupCount = 0;
    protected int countIndex;
    protected int escapeCountIndex;

    public delegate void DelOnCompleteHide();

    public virtual void Show(float tweenDuration = DEFAULT_TWEEN_DURATION, bool usingScaleAnim = true, float delay = 0)
    {
        durationTime = 0;
        this.Initialized(usingScaleAnim);
        if (this.darkBackground != null) this.darkBackground.Show(tweenDuration);

        if (usingScaleAnim)
        {
            this.bodyScaleTweener.Reset(tweenDuration, EasingObject.BackEasingInOut);
            this.bodyScaleTweener.Delay(delay);
            this.bodyScaleTweenValue = this.bodyScaleTweener.CreateTween(this.bodyScaleBeginValue, this.bodyScaleEndValue);
        }

        this.alphaTweener.Reset(tweenDuration, EasingObject.LinearEasing, this.OnCompleteShow);
        this.alphaTweener.Delay(delay);
        float startAlpha = this.hasOpenAlpha ? 0.0f : 1.0f;
        this.alphaTweenValue = this.alphaTweener.CreateTween(startAlpha, 1f);

        this.showing = true;

        // 최적화
        if (_canEscapeHide)
        {
            if (_isCountingPop)
            {
                escapePopupCount++;
                this.escapeCountIndex = escapePopupCount;
            }
        }

        if (_isCountingPop)
        {
            popupCount++;
            this.countIndex = popupCount;
        }

        if (_hideMainUI)
        {
            if (_isCountingPop)
                HideMainUICount++;
            // if (HideMainUICount > 0)
            // {
            //     if (HomeUI.Instance && HomeUI.Instance.HomeCamera)
            //         HomeUI.Instance.HomeCamera.enabled = false;
            //
            //     if (HomeManager.Instance && HomeManager.Instance.HomeCamera)
            //         HomeManager.Instance.HomeCamera.enabled = false;
            // }
        }

        if (_isHidePop)
        {
            if (_isCountingPop)
                HidePopCount++;
            _hideMainPops.Add((0, this));
            RefreshHidePops();
        }
    }

    public virtual void Hide(float tweenDuration = DEFAULT_TWEEN_DURATION, bool usingScaleAnim = true)
    {
        if (!this.showing)
            return;

        if (this.darkBackground != null) this.darkBackground.Hide(tweenDuration);

        if (usingScaleAnim)
        {
            this.bodyScaleTweener.Reset(tweenDuration / 2f, EasingObject.BackEasingIn);
            this.bodyScaleTweenValue = this.bodyScaleTweener.CreateTween(this.popBody.localScale, this.bodyScaleBeginValue);
        }

        this.showing = false;
        this.initialized = false;

        // 최적화
        if (_isCountingPop)
        {
            if (_canEscapeHide)
                escapePopupCount--;

            popupCount--;
        }
        
        if (tweenDuration > 0f)
        {
            this.alphaTweener.Reset(tweenDuration / 2f, EasingObject.LinearEasing, this.OnCompleteHide);
            this.alphaTweenValue = this.alphaTweener.CreateTween(this.bodyCanvasGroup.alpha, 0f);
        }
        else
        {
            this.OnCompleteHide();
        }

        if (_hideMainUI)
        {
            if (_isCountingPop)
                HideMainUICount--;

            if (HideMainUICount <= 0)
            {
                // if (HomeUI.Instance && HomeUI.Instance.HomeCamera)
                //     HomeUI.Instance.HomeCamera.enabled = true;
                //
                // if (HomeManager.Instance && HomeManager.Instance.HomeCamera)
                //     HomeManager.Instance.HomeCamera.enabled = true;
            }
        }

        if (_isHidePop)
        {
            if (_isCountingPop)
                HidePopCount--;

            _hideMainPops.Remove((0, this));

            int _popToRemoved = -1;

            for (var i = 0; i < _hideMainPops.Count; i++)
            {
                if (ReferenceEquals(_hideMainPops[i].Item2, this))
                {
                    _popToRemoved = i;
                    break;
                }
            }

            if (_popToRemoved != -1)
            {
                _hideMainPops.RemoveAt(_popToRemoved);
            }

            RefreshHidePops();
        }
    }

    public void SetOnCompleteHide(DelOnCompleteHide onCompleteHide)
    {
        this.onCompleteHide = onCompleteHide;
    }

    public bool Showing
    {
        get { return this.showing; }
    }

    public bool CompleteShow
    {
        get { return this.completeShow; }
    }

    public bool CompleteHide
    {
        get { return this.completeHide; }
    }

    public float GetCameraDepthMax()
    {
        float depth = -100f;
        foreach (var camera in this.cameras)
        {
            depth = Mathf.Max(depth, camera.depth);
        }

        return depth;
    }

    public void SetCameraDepth(float startDepth)
    {
        if (this.cameras == null)
            return;

        for (int i = 0; i < this.cameras.Length; i++)
        {
            this.cameras[i].depth = startDepth + (float)i;
        }
    }

    public virtual bool Closeable
    {
        get { return false; }
    }

    public virtual void OnClickClose()
    {
        this.Hide();
        SoundManager.Instance.PlayCancel();
    }

    public virtual void OnClickDarkBackground()
    {
        if (this._canBackgroundClose && this.completeShow && this.showing)
        {
            this.Hide();
            SoundManager.Instance.PlayCancel();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // protected
    protected const float DEFAULT_TWEEN_DURATION = 0.2f;

    //UI Components
    [SerializeField] protected UEPopupBackground darkBackground;
    [SerializeField] protected RectTransform popBody;
    [SerializeField] protected Camera[] cameras;
    [SerializeField] protected bool _canBackgroundClose = true;
    [SerializeField] protected bool hasOpenAlpha = true;
    [SerializeField] protected bool _canEscapeHide = true;

    protected SimpleTweenerEx bodyScaleTweener = new SimpleTweenerEx();
    protected TweenLerp<Vector3> bodyScaleTweenValue;
    protected Vector3 bodyScaleBeginValue = Vector3.zero;
    protected Vector3 bodyScaleEndValue = Vector3.one;

    protected SimpleTweenerEx alphaTweener = new SimpleTweenerEx();
    protected TweenLerp<float> alphaTweenValue;

    protected DelOnCompleteHide onCompleteHide;

    protected bool showing = false;
    protected bool completeShow = false;
    protected bool completeHide = false;
    protected bool initialized = false;

    protected CanvasGroup bodyCanvasGroup;

    protected float durationTime;

    //out of spac
    //protected bool closeable = false;

    protected virtual void OnCompleteShow(object[] onCompleteParms = null)
    {
        this.completeShow = true;
    }

    protected virtual void OnCompleteHide(object[] onCompleteParms = null)
    {
        if(InGameManager.Instance)
            InGameManager.Instance.isTouchActive = true;
        this.completeHide = true;
        this.gameObject.SetActive(false);
        if (this.onCompleteHide != null)
            this.onCompleteHide();
        Destroy(this.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private

    private void Initialized(bool usingScaleAnim)
    {
        if (this.initialized)
            return;

        this.gameObject.SetActive(true);

        // 팝바디 어싸인 안했다면 찾아주기
        if (popBody == null)
        {
            popBody = transform.Find("Canvas").Find("Body").GetComponent<RectTransform>();
            Debug.LogWarning("Warning : POPBODY IS NULL : " + name);
        }
        
        if (this.popBody)
        {
            this.bodyCanvasGroup = this.popBody.GetComponent<CanvasGroup>();
            this.bodyCanvasGroup.alpha = 0f;

            if (usingScaleAnim)
                this.popBody.transform.localScale = Vector3.zero;
        }

        this.initialized = true;
    }
}