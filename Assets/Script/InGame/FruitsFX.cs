using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitsFX : PoolObject
{
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// public

    public void OnParticleSystemStopped()
    {
        ObjectPooler.Instance.ReturnToPool(this);
    }
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// private
    
    private bool _merged = false;
    private ParticleSystem _particleSystem;
    
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
}