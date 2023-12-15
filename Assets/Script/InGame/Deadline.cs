using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deadline : MonoBehaviour
{
    private float _elapsedTime = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent<FruitsObject>(out var fruitsObject);
        if (fruitsObject != null)
        {
            if(fruitsObject.Rigidbody2D.velocity.y >= 0)
                InGameManager.Instance.AlertGameOver();
        }
    }
}
