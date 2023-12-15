using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum ObjectTag
{
    FRUITS_01 = 1001,
    FRUITS_02,
    FRUITS_03,
    FRUITS_04,
    FRUITS_05,
    FRUITS_06,
    FRUITS_07,
    FRUITS_08,
    FRUITS_09,
    
    COATED_FRUITS_01 = 2001,
    COATED_FRUITS_02,
    COATED_FRUITS_03,
    COATED_FRUITS_04,
    COATED_FRUITS_05,
    COATED_FRUITS_06,
    COATED_FRUITS_07,
    COATED_FRUITS_08,
    COATED_FRUITS_09,
    
    FX_01 = 10001,
    FX_02,
    FX_03,
    FX_04,
    FX_05,
    FX_06,
    FX_07,
    FX_08,
    FX_09,
}

[System.Serializable]
public class PoolObject : MonoBehaviour
{
    public ObjectTag tag;
    public int size = 1;

    public virtual void InitObject(Transform parent = null)
    {
        //오브젝트 초기화
        if(parent != null)
            transform.SetParent(parent);
    }
}

public class ObjectPooler : GameObjectSingleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public PoolObject prefab;
        public int size;
    }

    public List<PoolObject> fruitList;
    public List<PoolObject> fxList;
    public List<PoolObject> coatedFruitList;
    public Dictionary<ObjectTag, Queue<PoolObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        
        poolDictionary = new Dictionary<ObjectTag, Queue<PoolObject>>();
        
        AddToDic(fruitList);
        AddToDic(fxList);
        AddToDic(coatedFruitList);
    }

    public PoolObject SpawnFromPool(ObjectTag objectTag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(objectTag))
        {
            Debug.LogWarning("Pool with tag " + objectTag + " doesn't exist.");
            return null;
        }

        poolDictionary[objectTag].TryDequeue(out var objectToSpawn);

        if (objectToSpawn != null)
        {
            objectToSpawn.gameObject.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.InitObject(parent);
            return objectToSpawn;
        }
        else
        {
            // 풀이 비어 있을 때 추가로 게임 오브젝트 생성
            PoolObject currentPool = fruitList.Find(p => p.tag == objectTag);
            
            if(currentPool == null) 
                currentPool = fxList.Find(p => p.tag == objectTag);

            if (currentPool == null)
                currentPool = coatedFruitList.Find(p => p.tag == objectTag);
            
            if (currentPool != null)
            {
                objectToSpawn = Instantiate(currentPool, transform);
                objectToSpawn.gameObject.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.InitObject(parent);
                return objectToSpawn; // 새로 생성된 객체 반환
            }
            else
            {
                Debug.LogWarning("No pool found with tag " + objectTag);
                return null;
            }
        }
    }

    public void ReturnToPool(PoolObject objectToReturn, Action onComplete = null)
    {
        if (!poolDictionary.ContainsKey(objectToReturn.tag))
        {
            Debug.LogWarning("Pool with tag " + objectToReturn.tag + " doesn't exist.");
            return;
        }

        objectToReturn.gameObject.SetActive(false);
        poolDictionary[objectToReturn.tag].Enqueue(objectToReturn);
        
        onComplete?.Invoke();
    }

    private void AddToDic(List<PoolObject> poolObjects)
    {
        foreach (PoolObject pool in poolObjects)
        {
            Queue<PoolObject> objectPool = new Queue<PoolObject>();

            for (int i = 0; i < pool.size; i++)
            {
                PoolObject obj = Instantiate<PoolObject>(pool, transform);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
}