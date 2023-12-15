using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Skewer : MonoBehaviour
{
    public List<Transform> FruitTransform => _fruitTransforms;
    
    [SerializeField] private List<Transform> _fruitTransforms;
    
    public List<CoatedFruitObject> CoatedFruitObjects => _coatedFruitObjects;
    
    [SerializeField] private List<CoatedFruitObject> _coatedFruitObjects;

    public bool CheckTargetFruit(bool isWaterMelon)
    {
        List<FruitsObject> fruits = FruitsManager.Instance.SpawnObjects.ToList();

        bool isComplete = false;
        int count = 0;

        if (isWaterMelon)
        {
            foreach (var fruit in _fruitTransforms)
            {
                CoatedFruitObject target = fruit.GetComponentInChildren<CoatedFruitObject>();

                if (target)
                {
                    if(fruits.Exists(f => (int)f.tag == ((int)(target.tag) - 1000)))
                    {
                        FruitsObject removedFruit = fruits.Find(f => (int)f.tag == ((int)(target.tag) - 1000));
                        fruits.Remove(removedFruit);
                        ++count;
                    }
                }
            }
            
            if (count == 1)
                isComplete = true;
        }
        else
        {
            foreach (var fruit in _fruitTransforms)
            {
                CoatedFruitObject target = fruit.GetComponentInChildren<CoatedFruitObject>();
            
                if (target)
                {
                    if(fruits.Exists(f => (int)f.tag == ((int)(target.tag) - 1000)))
                    {
                        FruitsObject removedFruit = fruits.Find(f => (int)f.tag == ((int)(target.tag) - 1000));
                        fruits.Remove(removedFruit);
                        ++count;
                    }
                }
            }
            
            if (count == 4)
                isComplete = true;
        }
       

       

        return isComplete;
    }
}
