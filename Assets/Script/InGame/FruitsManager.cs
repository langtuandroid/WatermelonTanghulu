using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FruitsManager : GameObjectSingleton<FruitsManager>
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// public

    public int Coin => _coin;
    public int Score => _score;
    public int GenCoin => _genCoin;
    public List<ObjectTag> NextSpawnObjectList => _nextSpawnObjectList;

    public List<FruitsObject> SpawnObjects => _spawnObjects;

    public void MergeObject(FruitsObject object1, FruitsObject object2)
    {
        StartCoroutine(MergeObjectCoroutine(object1, object2));
    }

    public void UseFruit(FruitsObject fruitsObject)
    {
        StartCoroutine(UseFruitCoroutine(fruitsObject));
    }

   
    public void AddCoin(int coin)
    {
        _coin += coin;
    }

    public void AddScore(int score)
    {
        _score += score;
    }

    public void UseCoin(int coin)
    {
        _coin -= coin;
    }

    public void AddGenCoin()
    {
        _genCoin++;
    }

    public void CheckGenCoin()
    {
        //젠코인 시트 8개
        if (_genCoin > SpecDataManager.Instance.GenCoin.All.Count)
        {
            _genCoin = SpecDataManager.Instance.GenCoin.All.Count;
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// private
    [SerializeField] private Camera cameraToUse;
    [SerializeField] private GameObject deadline;
    [SerializeField] private GameObject spawnLine;
    
    private List<FruitsObject> _spawnObjects;

    [SerializeField] private List<ObjectTag> _nextSpawnObjectList;
    
    private int _score;
    private int _coin;
    private int _genCoin = 1;
    
    private FruitsObject _spawnedObject;
    private bool _isMerging = false;

    private readonly float MIN_X_POSITION = -5;
    private readonly float MAX_X_POSITION = 5;

    private readonly float NEXT_OBJECT_COUNT = 2;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        ReadyToStartPlay();
    }
    private void Update()
    {
        // 터치 이벤트가 발생했는지 확인
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) && Physics.Raycast(ray, out hit) && InGameManager.Instance.isTouchActive)
            {
                if (hit.collider)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        SetObject(touch.position);
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        MoveObject(touch.position);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        ReleaseObject(touch.position, true);
                    }
                }
            }
            else
            {
                Debug.Log("<color=red>UI 터치</color>");
            }
        }

        // 마우스 클릭 이벤트가 발생했는지 확인
        else if (Input.GetMouseButtonDown(0))
        {
            SetObject(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveObject(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseObject(Input.mousePosition, true);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ReleaseObject(Input.mousePosition);
        }
    }

    private void SetObject(Vector3 screenPosition)
    {
        // 터치한 위치 또는 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPosition = cameraToUse.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cameraToUse.nearClipPlane));

        // 값 보정
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // 해당 위치에 오브젝트 생성
        MoveSpawnedObject(worldPosition);

        if(_spawnedObject)
            InGameManager.Instance.ActiveLine(true, worldPosition);
    }

    private void MoveObject(Vector3 screenPosition)
    {
        // 터치한 위치 또는 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPosition = cameraToUse.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cameraToUse.nearClipPlane));

        // 값 보정
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // 해당 위치에 오브젝트 생성
        MoveSpawnedObject(worldPosition);
        if(_spawnedObject)
            InGameManager.Instance.ActiveLine(true, worldPosition);
    }

    private void ReleaseObject(Vector3 screenPosition)
    {
        // 터치한 위치 또는 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPosition = cameraToUse.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cameraToUse.nearClipPlane));

        // 값 보정
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // 해당 위치에 오브젝트 생성
        ReleaseSpawnedObject(worldPosition, false);
        
        
        InGameManager.Instance.ActiveLine(false, worldPosition);
    }

    
    private void ReleaseObject(Vector3 screenPosition, bool isUseCurrency)
    {
        // 터치한 위치 또는 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPosition = cameraToUse.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cameraToUse.nearClipPlane));

        // 값 보정
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // 해당 위치에 오브젝트 생성
        ReleaseSpawnedObject(worldPosition, isUseCurrency);
        
        
        InGameManager.Instance.ActiveLine(false, worldPosition);
    }

   
    private void ReadyToStartPlay()
    {
        _spawnObjects = new List<FruitsObject>();

        for (int i = 0; i < NEXT_OBJECT_COUNT; i++)
        {
            SelectNextSpawnObject();
        }
        
        if(InGameUI.Instance)
            InGameUI.Instance.UpdateNextFruitImage(_nextSpawnObjectList[1]);


        SetCurrency();
        
        InGameManager.Instance.ActiveLine(false, new Vector3(0,0,0));
        
        Run.After(0.5f, () => { SpawnObject(Vector3.zero); });
    }

    private void SetCurrency()
    {
        DataManager.Instance.MaxFruitBox();
        
        _coin = 0;
        _score = 0;
        _genCoin = 1;
    }
    private void SelectNextSpawnObject()
    {
        if (_spawnObjects.Count == 0) _nextSpawnObjectList.Add(ObjectTag.FRUITS_01);
        else if (_spawnObjects.Count < 10) _nextSpawnObjectList.Add(ObjectTag.FRUITS_01 + Random.Range(0, 2));
        else if (_spawnObjects.Count < 30) _nextSpawnObjectList.Add(ObjectTag.FRUITS_01 + Random.Range(0, 3));
        else _nextSpawnObjectList.Add(ObjectTag.FRUITS_01 + Random.Range(0, 4));
        
    }
    
    private void ReleaseSpawnedObject(Vector3 movePosition, bool isUseCurrency)
    {
        if (_spawnedObject != null)
        {
            float posX = Mathf.Clamp(movePosition.x, MIN_X_POSITION, MAX_X_POSITION);
            _spawnedObject.transform.position = new Vector2(posX, spawnLine.transform.position.y);
            _spawnedObject.Rigidbody2D.simulated = true;
            _spawnObjects.Add(_spawnedObject);
            SelectNextSpawnObject();
            
            _nextSpawnObjectList.RemoveAt(0);
            _spawnedObject = null;

            
            if(isUseCurrency)
                DataManager.Instance.UseCurrency(Currency.FRUIT_BOX, 1);

            if (!DataManager.Instance.CheckCanUseCurrency(Currency.FRUIT_BOX, 1))
                DataManager.Instance.ChangeFruitBoxFromCoin();
            
            if (InGameUI.Instance)
            {
                InGameUI.Instance.RefreshCurrency();
                InGameUI.Instance.UpdateNextFruitImage(_nextSpawnObjectList[1]);
            }
            
            if (!DataManager.Instance.CheckCanUseCurrency(Currency.FRUIT_BOX, 1))
            {
                GameOverPop.ShowPop();
                    
                //게임 오버 시 체크
                DataManager.Instance.UpdateBestScore(_score);
                    
                TimeHelper.SetTime(TimeType.STOP);
                Debug.Log("코인이 부족합니다.");
                return;
            }
            
            Run.After(0.5f, () =>
            {
                SpawnObject(new Vector3(posX, 0f, 0f)); 
               
                InGameManager.Instance.CheckFruit();
            });
        }
    }
    
    public void SpawnObject(Vector3 spawnPosition)
    {
        if (_spawnedObject == null)
        {
            Vector3 pos = new Vector3(spawnPosition.x, spawnLine.transform.position.y, 0f);
            _spawnedObject = (FruitsObject)ObjectPooler.Instance.SpawnFromPool(_nextSpawnObjectList[0], pos, Quaternion.identity);
        }

        _spawnedObject.Rigidbody2D.simulated = false;
    }

    
    private void MoveSpawnedObject(Vector3 movePosition)
    {
        if (_spawnedObject != null)
        {
            _spawnedObject.transform.position = new Vector2(Mathf.Clamp(movePosition.x, MIN_X_POSITION, MAX_X_POSITION), spawnLine.transform.position.y);
        }
    }

    private IEnumerator MergeObjectCoroutine(FruitsObject object1, FruitsObject object2)
    {
        while (_isMerging) yield return new WaitForEndOfFrame();

        _isMerging = true;

        Vector3 spawnPosition = Vector3.Lerp(object1.transform.position, object2.transform.position, 0.5f);
        ObjectPooler.Instance.ReturnToPool(object1);
        ObjectPooler.Instance.ReturnToPool(object2);
        
        ObjectPooler.Instance.SpawnFromPool(object1.FxObjectTag, spawnPosition, Quaternion.identity);

        //머지될때 점수 추가
        int sumScore = object1.FruitData.FruitMetaData.score + object2.FruitData.FruitMetaData.score;
        AddScore(sumScore);
        
        yield return new WaitForEndOfFrame();

        FruitsObject poolObject = (FruitsObject)ObjectPooler.Instance.SpawnFromPool(object1.tag + 1, spawnPosition, Quaternion.identity);
        
        _spawnObjects.Add(poolObject);
        _spawnObjects.Remove(object1);
        _spawnObjects.Remove(object2);
        
        
        yield return new WaitForEndOfFrame();

        _isMerging = false;
        
        InGameManager.Instance.CheckFruit();
    }

    private IEnumerator UseFruitCoroutine(FruitsObject fruitsObject)
    {
        Vector3 spawnPosition = fruitsObject.transform.position;

        //fruitsObject.FruitData.IsActive = false;
        
        ObjectPooler.Instance.ReturnToPool(fruitsObject);
        
        _spawnObjects.Remove(fruitsObject);

        yield return new WaitForEndOfFrame();
        
        ObjectPooler.Instance.SpawnFromPool(fruitsObject.FxObjectTag, spawnPosition, Quaternion.identity);
    }
    
}