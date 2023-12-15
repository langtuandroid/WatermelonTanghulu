/*********************************************
 * NHN StarFish - Simple Tween
 * CHOI YOONBIN
 * 
 *********************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STweenSpriteAlpha : STweenBase<float>
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    public float Value
    {
        get { return tweenValue.Value; }
    }

    public override void Restore()
    {
        base.Restore();

        if (this._spriteRenderer == null) this._spriteRenderer = this.GetComponent<SpriteRenderer>();

        if (this._spriteRenderer != null)
        {
            Color color = this._spriteRenderer.color;
            color.a = start;
            this._spriteRenderer.color = color;
        }
        else
        {
            SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = this._spriteRenderer.color;
                color.a = start;
                this._spriteRenderer.color = color;
            } 
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // overrides STweenBase 

    protected void Awake()
    {
        this._spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void PlayTween()
    {
        base.PlayTween();
        this.SetValue(this.start);
        base.tweenValue = this.tweener.CreateTween(this.start, this.end);
    }

    protected override void UpdateValue(float value)
    {
        base.UpdateValue(value);
        this.SetValue(value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private 

    private SpriteRenderer _spriteRenderer;

    private void SetValue(float alphaValue)
    {
        if (this._spriteRenderer != null)
        {
            Color color = this._spriteRenderer.color;
            color.a = alphaValue;
            this._spriteRenderer.color = color;
        }
        else
        {
            SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = this._spriteRenderer.color;
                color.a = start;
                this._spriteRenderer.color = color;
            } 
        }
    }

}