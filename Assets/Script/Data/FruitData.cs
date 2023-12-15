using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitData
{
    public FruitData(Fruit fruitMetaData)
    {
        FruitMetaData = fruitMetaData;
    }
    
    public bool IsActive = false;
    
    public Fruit FruitMetaData;
}
