/*********************************************
 * UGUI Extends
 * CHOI YOONBIN
 * 
 *********************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;

public class UEListContainer : MonoBehaviour
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // public

    //delegate for update list item
    public delegate void OnUpdateItem(int index, UEListComponent comp);

    public void InitListData(GameObject itemPrefab, int count, int countPerLine, float bottomMargin, ScrollRect scrollRect, OnUpdateItem onUpdateItemDelegate)
    {
        this.itemPrefab = itemPrefab;
        this.itemPrefab.SetActive(false);
        this.cellwidth = this.itemPrefab.GetComponent<RectTransform>().sizeDelta.x;
        this.cellheight = this.itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        this.dataCount = count;
		this.countPerLine = countPerLine;
        this.fitParent = countPerLine == 0;
		this.bottomMargin = bottomMargin;
		this.scrollRect = scrollRect;

        this.onUpdateItem = onUpdateItemDelegate;
        this.InitData();
    }

    public void ClearList()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        this.transform.DetachChildren();
        this.transform.localPosition = Vector3.zero;
    }

    public void RefreshAll()
    {
        if (this.items == null) return;

        for (int i = 0; i < this.items.Length; i++)
        {
            int dataIndex = this.items[i].Index;
            if (dataIndex >= 0)
            {
                this.onUpdateItem(dataIndex, this.items[i]);
            }
        }
    }

    public void RefreshAll(int count)
    {
        this.dataCount = count;
        this.InitData(true);
    }

    public void MoveToTop()
    {
        this.StartCoroutine(this.MoveToTopCoroutine());
    }

    public void MoveToBottom()
    {
        if (this.isActiveAndEnabled)
        {
            StopAllCoroutines();
            this.StartCoroutine(this.MoveToBottonCoroutine());
        }
    }

    public int Count
    {
        get { return this.dataCount; }
    }

    public int ItemCount
    {
        get { return this.itemCount; }
    }

    public int CountPerLine
    {
        get { return this.countPerLine; }
    }

    public ScrollRect ScrollRect
    {
        get { return this.scrollRect; }
    }

    public float CellWidth
    {
        get { return this.cellwidth; }
    }

    public float CelllHeight
    {
        get { return this.cellheight; }
    }

    public float BottomMargin
    {
        get { return this.bottomMargin; }
    }

    public float ScrollLimitCount
    {
        get { return this.itemCount - this.countPerLine; }
    }

    public float Alpha
    {
        set
        {
            this.alpha = value;
            if (this.alphaCanvas != null)
                this.alphaCanvas.alpha = this.alpha;
        }
    }

    public void CenterItem(int index)
    {
        if (this.scrollRect.vertical)
        {
            float offSet = (float)index * this.CelllHeight;
            float scrollCenter = this.scrollRectValue.height / 2f;
            float itemCenter = this.CelllHeight / 2f;
            float max = this.rectTransform.rect.height - this.scrollRectValue.height;
            
            float t = Mathf.Clamp((offSet - scrollCenter + itemCenter), 0, max);
            this.rectTransform.anchoredPosition = new Vector2(0f, t);
        }
        else
        {
            float offSet = (float)index * this.CellWidth;
            float scrollCenter = this.scrollRectValue.width / 2f;
            float itemCenter = this.CellWidth / 2f;
            float max = this.rectTransform.rect.width - this.scrollRectValue.width;
            
            float t = Mathf.Clamp((offSet - scrollCenter + itemCenter), 0, max);
            this.rectTransform.anchoredPosition = new Vector2(t, 0f);
        }
        this.UpdateItemPosition();
    }
    
    public void CenterItemCoroutine(int index)
    {
        if (this.scrollRect.vertical)
        {
            float offSet = (float)index * this.CelllHeight;
            float scrollCenter = this.scrollRectValue.height / 2f;
            float itemCenter = this.CelllHeight / 2f;
            float max = this.rectTransform.rect.height - this.scrollRectValue.height;
            
            float t = Mathf.Clamp((offSet - scrollCenter + itemCenter), 0, max);
            StartCoroutine(MoveToCenterItem(this.rectTransform.anchoredPosition, new Vector2(0f, t)));
        }
        else
        {
            float offSet = (float)index * this.CellWidth;
            float scrollCenter = this.scrollRectValue.width / 2f;
            float itemCenter = this.CellWidth / 2f;
            float max = this.rectTransform.rect.width - this.scrollRectValue.width;
            
            float t = Mathf.Clamp((offSet - scrollCenter + itemCenter), 0, max);
            StartCoroutine(MoveToCenterItem(this.rectTransform.anchoredPosition, new Vector2(t, 0f)));
        }
    }

    private IEnumerator MoveToCenterItem(Vector2 start, Vector2 goal)
    {
        float t = 0f;
        float addFloat = Time.fixedDeltaTime * 3f;
        WaitForFixedUpdate delay = new WaitForFixedUpdate();
        while (t < 1)
        {
            t += addFloat;
            this.rectTransform.anchoredPosition = Vector2.Lerp(start, goal, t);
            this.UpdateItemPosition();
            
            yield return delay;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // protected

    protected void Awake()
    {
        if (this.scrollRect == null)
            this.scrollRect = GetComponentInParent<ScrollRect>();

        this.scrollRect.onValueChanged.AddListener(this.OnScrollChange);
        this.rectTransform = this.GetComponent<RectTransform>();
        this.alphaCanvas = this.GetComponent<CanvasGroup>();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // private

    [SerializeField] private bool _useCenterAlign = false;
    
    private float cellwidth;
    private float cellheight;
    private int dataCount;
    private int countPerLine = 1;
    private bool fitParent = false;
    private float bottomMargin;
    private GameObject itemPrefab;
    private ScrollRect scrollRect;
    private RectTransform rectTransform;
    private Rect scrollRectValue;

    private OnUpdateItem onUpdateItem;

    private int itemCount;
    private UEListComponent[] items;

    private float alpha;
    private CanvasGroup alphaCanvas;

    private void InitData(bool keepPostion = false)
    {
        this.ClearAllChild();
        this.rectTransform = this.GetComponent<RectTransform>();
        this.scrollRectValue = this.scrollRect.GetComponent<RectTransform>().rect;

        if (this.scrollRect.vertical)
            this.countPerLine = this.fitParent ? Mathf.Max(this.countPerLine, Mathf.FloorToInt(this.scrollRectValue.width / this.cellwidth)) : this.countPerLine;
        else
            this.countPerLine = this.fitParent ? Mathf.Max(this.countPerLine, Mathf.FloorToInt(this.scrollRectValue.height / this.cellheight)) : this.countPerLine;
        
        int rowCount = Mathf.CeilToInt((float)this.dataCount / (float)this.countPerLine);
        
        if (this.scrollRect.vertical)
        {
            float width = this.fitParent ? this.countPerLine * this.cellwidth : this.rectTransform.sizeDelta.x;
            this.rectTransform.sizeDelta = new Vector2(width, this.bottomMargin + Mathf.Max(this.scrollRect.GetComponent<RectTransform>().sizeDelta.y, this.cellheight * (float)rowCount));
            this.rectTransform.anchorMax = new Vector2(0.5f, 1f); 
            this.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            this.rectTransform.pivot = new Vector2(0.5f, 1f);
            this.itemCount = (int)(Mathf.Abs(Mathf.Ceil(this.scrollRectValue.height / this.cellheight)) + 1) * this.countPerLine;
            this.scrollRect.enabled = (this.scrollRectValue.height - this.bottomMargin) < (rowCount * this.cellheight);
        }
        else
        {
            float height = this.fitParent ? this.countPerLine * this.cellheight : this.rectTransform.sizeDelta.y;
            this.rectTransform.sizeDelta = new Vector2(Mathf.Max(this.scrollRect.GetComponent<RectTransform>().sizeDelta.x, this.cellwidth * (float)rowCount), height);
            this.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            this.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            this.rectTransform.pivot = new Vector2(0f, 0.5f);
            this.itemCount = (int)(Mathf.Abs(Mathf.Ceil(this.scrollRectValue.width / this.cellwidth)) + 1) * this.countPerLine;
            this.scrollRect.enabled = (this.scrollRectValue.width) < (rowCount * this.cellwidth);
        }

        if (!keepPostion)
            this.rectTransform.anchoredPosition = Vector2.zero;

        this.items = new UEListComponent[this.itemCount];

        for (int i = 0; i < this.itemCount; i++)
        {
            GameObject go = Instantiate(this.itemPrefab) as GameObject;
            go.SetActive(true);
            this.items[i] = go.GetComponent<UEListComponent>();
            this.items[i].RectTransform.SetParent(this.transform);
            this.items[i].RectTransform.localScale = Vector3.one;
            this.items[i].RectTransform.localPosition = Vector3.zero;
            this.items[i].RectTransform.anchorMax = new Vector2(0, 1);
            this.items[i].RectTransform.anchorMin = new Vector2(0, 1);
            this.items[i].RectTransform.pivot = new Vector2(0, 1);
            this.items[i].RectTransform.sizeDelta = new Vector2(this.cellwidth, this.cellheight);
        }

        this.UpdateItemPosition();
    }

    private void UpdateItemPosition()
    {
        float offsetForAlignCenter = 0;
        if (this.scrollRect.vertical)
        {
            if (_useCenterAlign && this.scrollRect.enabled == false)
            {
                offsetForAlignCenter = this.rectTransform.rect.height / 2 - this.itemCount * this.cellheight / 2;
            }
            //Remove Spring area when list Dragging on each end of side.
            float min = 0f;
            float max = this.rectTransform.rect.height - this.scrollRectValue.height;
            float curPosY = Mathf.Clamp(this.rectTransform.anchoredPosition.y, min, max);

            //Start Y Position for item's position in first line
            float startPosY = Mathf.Floor(curPosY / this.cellheight) * this.cellheight;
            //Start index for first item in list
            int startIndex = Mathf.FloorToInt(curPosY / this.cellheight) * this.countPerLine;

            for (int i = 0; i < this.itemCount; i++)
            {
                int dataIndex = startIndex + i;

                if (!this.items[i]) continue;

                if (dataIndex < 0 || dataIndex >= this.dataCount)
                {
                    this.items[i].gameObject.SetActive(false);
                }
                else
                {
                    bool forceUpdate = false;
                    if (!this.items[i].gameObject.activeSelf)
                    {
                        this.items[i].gameObject.SetActive(true);
                        forceUpdate = true;
                    }

                    float posX = this.cellwidth * (i % this.countPerLine);
                    float posY = -(startPosY + (int)((i / this.countPerLine) * this.cellheight));

                    this.items[i].RectTransform.anchoredPosition = new Vector2(posX, posY + offsetForAlignCenter);
                    if (forceUpdate || dataIndex != this.items[i].Index)
                    {
                        this.items[i].Index = dataIndex;
                        this.onUpdateItem(dataIndex, this.items[i]);
                    }
                }
            }
        }
        else
        {
            if (_useCenterAlign && this.scrollRect.enabled == false)
            {
                offsetForAlignCenter = this.rectTransform.rect.width / 2 - this.dataCount * this.cellwidth / 2;
            }
            
            //Remove Spring area when list Dragging on each end of side.
            float min = -this.rectTransform.rect.width - this.scrollRectValue.width;
            float max = 0f;
            float curPosX = Mathf.Abs(Mathf.Clamp(this.rectTransform.anchoredPosition.x, min, max));

            //Start X Position for item's position in first line
            float startPosX = Mathf.Floor(curPosX / this.cellwidth) * this.cellwidth;
            //Start index for first item in list
            int startIndex = Mathf.FloorToInt(curPosX / this.cellwidth) * this.countPerLine;

            for (int i = 0; i < this.itemCount; i++)
            {
                int dataIndex = startIndex + i;

                if (!this.items[i]) continue;

                if (dataIndex < 0 || dataIndex >= this.dataCount)
                {
                    this.items[i].gameObject.SetActive(false);
                }
                else
                {
                    bool forceUpdate = false;
                    if (!this.items[i].gameObject.activeSelf)
                    {
                        this.items[i].gameObject.SetActive(true);
                        forceUpdate = true;
                    }

                    float posX = startPosX + (int)((i / this.countPerLine) * this.cellwidth);
                    float posY = -(this.cellheight * (i % this.countPerLine));

                    this.items[i].RectTransform.anchoredPosition = new Vector2(posX + offsetForAlignCenter, posY);
                    if (forceUpdate || dataIndex != this.items[i].Index)
                    {
                        this.items[i].Index = dataIndex;
                        this.onUpdateItem(dataIndex, this.items[i]);
                    }
                }
            }
        }
    }

    public void OnScrollChange(Vector2 value)
    {
        if (this.items != null)
            this.UpdateItemPosition();
    }

    private void ClearAllChild()
    {
        int childCount = this.transform.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }
            this.transform.DetachChildren();
        }
    }

    private IEnumerator MoveToTopCoroutine()
    {
        float verticalNormalizePos = this.scrollRect.verticalNormalizedPosition;
        while (verticalNormalizePos < 1f)
        {
            verticalNormalizePos += 0.05f;
            if (verticalNormalizePos > 1f)
                verticalNormalizePos = 1f;
            this.scrollRect.verticalNormalizedPosition = verticalNormalizePos;
            yield return null;
        }
        yield break;
    }

    private IEnumerator MoveToBottonCoroutine()
    {
        yield return null;

        if (this.scrollRect.vertical)
        {
            float verticalNormalizePos = this.scrollRect.verticalNormalizedPosition;
            while (verticalNormalizePos > 0f)
            {
                verticalNormalizePos -= 0.05f;
                if (verticalNormalizePos < 0f)
                    verticalNormalizePos = 0f;
                this.scrollRect.verticalNormalizedPosition = verticalNormalizePos;
                yield return null;
            }
        }
        else
        {
            float horizontalNormalizePos = this.scrollRect.horizontalNormalizedPosition;
            while (horizontalNormalizePos < 1f)
            {
                horizontalNormalizePos += 0.05f;
                if (horizontalNormalizePos > 1f)
                    horizontalNormalizePos = 1f;
                this.scrollRect.horizontalNormalizedPosition = horizontalNormalizePos;
                yield return null;
            }
        }
    }
}
