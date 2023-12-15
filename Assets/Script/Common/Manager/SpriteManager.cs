using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : GameObjectSingleton<SpriteManager>
{
    public Sprite GetSprite(ObjectTag objectTag)
    {
        Sprite sprite = ObjectPooler.Instance.fruitList
            .Find(t => t.tag == objectTag).gameObject.GetComponent<SpriteRenderer>().sprite;

        if (sprite)
            return sprite;

        return null;
    }
    
}
