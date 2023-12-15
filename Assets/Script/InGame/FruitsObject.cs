using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitsObject : PoolObject
{
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// public

    public FruitData FruitData => _fruitData;
    
    public Rigidbody2D Rigidbody2D => _rb;
    public bool Merged => _merged; 
    public ObjectTag FxObjectTag => _fxObjectTag;
    
    public override void InitObject(Transform parent = null)
    {
        base.InitObject(parent);
        _merged = false;
        SetData();
    }

    private void SetData()
    {
        switch (tag)
        {
            case ObjectTag.FRUITS_01:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_01));
                break;
            case ObjectTag.FRUITS_02:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_02));
                break;
            case ObjectTag.FRUITS_03:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_03));
                break;
            case ObjectTag.FRUITS_04:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_04));
                break;
            case ObjectTag.FRUITS_05:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_05));
                break;
            case ObjectTag.FRUITS_06:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_06));
                break;
            case ObjectTag.FRUITS_07:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_07));
                break;
            case ObjectTag.FRUITS_08:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_08));
                break;
            case ObjectTag.FRUITS_09:
                _fruitData = new FruitData(SpecDataManager.Instance.Fruit.Get((int)ObjectTag.FRUITS_09));
                break;
            default:
                Debug.LogError($"잘못된 태그입니다. ObjectTag : {tag}");
                return;
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// private
    
    private bool _merged = false;
    private Rigidbody2D _rb;

    private FruitData _fruitData;

    [SerializeField] private ObjectTag _fxObjectTag = ObjectTag.FX_01;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_merged) return;
        
        if(tag == ObjectTag.FRUITS_09) return;
        
        collision.gameObject.TryGetComponent<FruitsObject>(out var collisionObject);
        if (collisionObject != null)
        {
            if (tag == collisionObject.tag)
            {
                _merged = true;
                collisionObject._merged = true;
                FruitsManager.Instance.MergeObject(this, collisionObject);
            }
        }
    }
}