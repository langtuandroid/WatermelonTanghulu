using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Duration
{
    SHORT = 0,
    NORMAL = 1,
    LONG = 2,
}

public class ToastPop : UEPopup
{
    /////////////////////////////////////////////////////////////
    /// public static
    ///
    protected static string _lastMsg;

    public static void ShowPop(string msg, Duration duration = Duration.NORMAL)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }
        
        ToastPop pop = UEPopup.GetInstantiateComponent<ToastPop>();
        pop.Show(msg, duration);
    }

    /////////////////////////////////////////////////////////////
    /// public
    public void Show(string msg, Duration duration)
    {
        messageText.text = msg;
        if (duration == Duration.SHORT)
        {
            messageText.color = Color.red;
        }
        SoundManager2.Instance.SfxPlaySound("ToastPop");
        this.StartCoroutine(this.ShowHide(duration));
    }

    public void OnClickSkip()
    {
        if (this.canClose)
        {
            this.StopAllCoroutines();
            this.OnCompleteHide();
        }
    }

    /////////////////////////////////////////////////////////////
    /// private
    private readonly Vector3 hideScale = new Vector3(1f, 0f, 1f);

    private readonly Vector3 showScale = Vector3.one;

    [SerializeField] private Text messageText;
    private bool canClose = false;

    private IEnumerator ShowHide(Duration durationType)
    {
        float duration = 0.5f + (float)durationType;

        yield return StartCoroutine(this.ScaleTo(hideScale, showScale, DEFAULT_TWEEN_DURATION));
        this.OnCompleteShow();
        this.canClose = true;
        yield return new WaitForSecondsRealtime(duration);
        yield return StartCoroutine(this.ScaleTo(showScale, hideScale, DEFAULT_TWEEN_DURATION));

        _lastMsg = string.Empty;
        this.OnCompleteHide();
    }

    private IEnumerator ScaleTo(Vector3 from, Vector3 to, float duration)
    {
        float acc = 0f;
        while (acc < duration)
        {
            acc += Time.unscaledDeltaTime;
            if (acc > duration) acc = duration;
            this.popBody.transform.localScale = Vector3.Lerp(from, to, acc / duration);
            yield return new WaitForEndOfFrame();
        }
    }
}