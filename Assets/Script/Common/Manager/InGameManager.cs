using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameManager : GameObjectSingleton<InGameManager>
{
    /////////////////////////////////////////////////////////////////////////////////////////////////
    /// public
    public List<List<FruitData>> TargetTanghulus => _targetTanghulus;

    public bool isTouchActive = true;
    public bool[] isSell = new []{false, false, false};
    
    public void Init()
    {
        
        for (int i = 0; i < TARGET_COUNT; ++i)
        {
            _targetTanghulus.Add(new List<FruitData>());
            
            int coin = 0;
            for (int j = 0; j < TANGHURU_LENGTH; ++j)
            {
                FruitData fruit = new FruitData(SpecDataManager.Instance.Fruit.Get(Random.Range(0, _maxFruitLevel + 1) +
                    (int)ObjectTag.FRUITS_01));
                
                coin += fruit.FruitMetaData.score;
                
                _targetTanghulus[i].Add(fruit);
                CoatedFruitObject coatedFruitObject = (CoatedFruitObject)ObjectPooler.Instance.SpawnFromPool((ObjectTag)(fruit.FruitMetaData.id + 1000), _skewers[i].FruitTransform[j].position,
                    _skewers[i].transform.localRotation, _skewers[i].FruitTransform[j]);

                SetTargetFruitAlpha(coatedFruitObject.GetComponent<SpriteRenderer>(), 0.5f);
                
                _skewers[i].CoatedFruitObjects.Add(coatedFruitObject);
            }
            
            _skewersCoin[i] = coin;
            
            if(InGameUI.Instance)
                InGameUI.Instance.UpdateSkewersCoinText(i,_skewersCoin[i]);
            Debug.Log($"{i + 1}번 탕후루 : <b>{_targetTanghulus[i][0].FruitMetaData.key}</b> <b>{_targetTanghulus[i][1].FruitMetaData.key}</b> <b>{_targetTanghulus[i][2].FruitMetaData.key}</b> <b>{_targetTanghulus[i][3].FruitMetaData.key}</b>");
        }
    }

    //투명도 조절
    public void SetTargetFruitAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        spriteRenderer.color = new Color(1, 1, 1, alpha);
    }
    public void SetTargetFruitAlpha(Image image, float alpha)
    {
        image.color = new Color(1, 1, 1, alpha);
    }
    public void ChangeTargetTangHuru(int targetIndex)
    {
        int coin = 0;
        bool isWaterMelon = false;
        for (int i = 0; i < TANGHURU_LENGTH; ++i)
        {
            //수박 이후 과일은 수박까지만 예외처리
            int fruitId = Random.Range(0, _maxFruitLevel + 1) + (int)ObjectTag.FRUITS_01;
            if (fruitId > (int)ObjectTag.FRUITS_09)
                fruitId = (int)ObjectTag.FRUITS_09;
            
            FruitData fruit = new FruitData(SpecDataManager.Instance.Fruit.Get(fruitId));
            _targetTanghulus[targetIndex][i] = fruit;
            Debug.Log($"{targetIndex + 1}번 탕후루에 {i + 1}번 {_targetTanghulus[targetIndex][i].FruitMetaData.key} <color=blue>비활성화</color>");
            
            coin += fruit.FruitMetaData.score;
            
            
            ObjectPooler.Instance.ReturnToPool(_skewers[targetIndex].FruitTransform[i].GetChild(0).GetComponent<PoolObject>() , _skewers[targetIndex].FruitTransform[i].DetachChildren);
            
            CoatedFruitObject coatedFruitObject = (CoatedFruitObject)ObjectPooler.Instance.SpawnFromPool((ObjectTag)(fruit.FruitMetaData.id + 1000), _skewers[targetIndex].FruitTransform[i].position,
                _skewers[targetIndex].transform.localRotation, _skewers[targetIndex].FruitTransform[i]);

            SetTargetFruitAlpha(coatedFruitObject.GetComponent<SpriteRenderer>(), 0.5f);
            
            _skewers[targetIndex].CoatedFruitObjects.Add(coatedFruitObject);
            
            //수박 탕후루 이면 한번만 돌게 처리
            if (_skewers[targetIndex].CoatedFruitObjects[i].tag == ObjectTag.COATED_FRUITS_09) //ObjectTag.COATED_FRUITS_09
            {
                _skewers[targetIndex].CoatedFruitObjects[i].transform.position =
                    _skewers[targetIndex].FruitTransform[2].position;
                
                coin = 0;
                isWaterMelon = true;
                RemoveCoatedFruitObjects(targetIndex);
                break;
            }
        }
        
        _skewersCoin[targetIndex] = coin;
        
        if(InGameUI.Instance)
            InGameUI.Instance.UpdateSkewersCoinText(targetIndex,_skewersCoin[targetIndex], isWaterMelon);
    }

    private void RemoveCoatedFruitObjects(int targetIndex)
    {
        if (_skewers[targetIndex].CoatedFruitObjects.Count <= 0)
            return;
        
        _skewers[targetIndex].CoatedFruitObjects.RemoveRange(0, _skewers[targetIndex].CoatedFruitObjects.Count-1);

        for (int i = 0; i < TANGHURU_LENGTH; i++)
        {
            var targetObject = _skewers[targetIndex].FruitTransform[i].GetChild(0).GetComponent<PoolObject>();
            if (!targetObject)
                continue;
            
            if (targetObject.tag != ObjectTag.COATED_FRUITS_09) //COATED_FRUITS_09
            {
                targetObject.gameObject.SetActive(false); //, _skewers[targetIndex].FruitTransform[i].DetachChildren
            }
            
        }
    }

    private bool CheckWaterMelon(int index)
    {
        if (_skewers[index].CoatedFruitObjects.Find(t=> t.tag == ObjectTag.COATED_FRUITS_09)) //ObjectTag.COATED_FRUITS_09
        {
            return true;
        }
        
        return false;
    }
    // 과일 쏠 때 or 과일 머지 될 때 호출
    public void CheckFruit()
    {
        for (int i = 0; i < _skewers.Count; ++i)
        {
            if (_skewers[i].CheckTargetFruit(CheckWaterMelon(i)))
            {
                isSell[i] = true;
                
                SetTargetFruitAlpha(InGameUI.Instance.GetSellButtonImage(i), 1);
                SetTargetFruitAlpha(InGameUI.Instance.GetSellButtonChildrenImage(i), 1f);
                
                //탕후루 활성화
                UpdateSetFruitTransParency(i, 1);

            }
            else
            {
                isSell[i] = false;
                
                SetTargetFruitAlpha(InGameUI.Instance.GetSellButtonImage(i), 0.5f);
                SetTargetFruitAlpha(InGameUI.Instance.GetSellButtonChildrenImage(i), 0.5f);
                
                //탕후루 비 활성화
                UpdateSetFruitTransParency(i, 0.5f);
            }
        }
    }
    
    private void UpdateSetFruitTransParency(int i, float alpha)
    {
        for (int k = 0; k < _skewers[i].CoatedFruitObjects.Count; k++)
        {
            SetTargetFruitAlpha(_skewers[i].CoatedFruitObjects[k].GetComponent<SpriteRenderer>(), alpha);
        }
    }

    public void ClearFruit(int index, bool isRefresh)
    {
        List<CoatedFruitObject> list = _skewers[index].CoatedFruitObjects;

        if (isRefresh == false)
        {
            for (int i = 0; i < list.Count; i++)
            {
                FruitsObject fruitsObject = FruitsManager.Instance.SpawnObjects.Find(item =>
                    item.FruitData.FruitMetaData.id == ((int)list[i].tag - 1000));
            
                //수박일 때는 코인을 안주고 점수만 준다.
                if (fruitsObject.FruitData.FruitMetaData.id == (int)ObjectTag.FRUITS_09) //FRUITS_09
                {
                    //현재 점수 x2가 된다.
                    FruitsManager.Instance.AddScore(FruitsManager.Instance.Score);
                }
                else
                {
                    FruitsManager.Instance.AddCoin(fruitsObject.FruitData.FruitMetaData.score);
                }
            
                FruitsManager.Instance.UseFruit(fruitsObject);
             
            
            }
        }
        else
        {
            FruitsManager.Instance.UseCoin(SpecDataManager.Instance.GameConfig.Get(10002).value);
        }
            
        //꼬치에 있는 오브젝트 클리어
        if(_skewers[index].CoatedFruitObjects.Count > 0)
            _skewers[index].CoatedFruitObjects.Clear();
        
        SetLevelRange();
    }

    public void AlertGameOver()
    {
        // 게임 오버 로직
        GameOverPop.ShowPop();
        //게임 오버 시 체크
        DataManager.Instance.UpdateBestScore(FruitsManager.Instance.Score);
        TimeHelper.SetTime(TimeType.STOP);
        Debug.Log("Game Over !! Game Over !! Game Over !! Game Over !! Game Over !!");
    }
    

    public void PlayParticle(ParticleType particleType,int index)
    {
        switch (particleType)
        {
            case ParticleType.ADD_COIN:
                _Fx[(int)particleType].gameObject.transform.position = _skewers[index].gameObject.transform.position;
                _Fx[(int)particleType].Play();
                break;
            case ParticleType.REFRESH_FRUIT:
                _Fx[(int)particleType].gameObject.transform.position = _skewers[index].gameObject.transform.position;
                _Fx[(int)particleType].Play();
                break;
        }
    }
    
    
    
    
    public void ActiveLine(bool isActive,Vector3 pos)
    {
        _lineObject.SetActive(isActive);

        Vector3 vec = new Vector3(pos.x, -4f, pos.z);
        _lineObject.transform.position = vec;
    }
    
    
    /////////////////////////////////////////////////////////////////////////////////////////////////
    /// private
    
    [SerializeField] private List<Skewer> _skewers;
    
    [SerializeField] private int[] _skewersCoin = new int[3];
    
    //과일 선
    [SerializeField] private GameObject _lineObject;
    
    private readonly int TARGET_COUNT = 3;
    private readonly int TANGHURU_LENGTH = 4;
    private readonly int MIN_FRUIT_LEVEL = 1;
    
    private int _maxFruitLevel = 4;
    
    private List<List<FruitData>> _targetTanghulus = new List<List<FruitData>>();
    
    [SerializeField] private List<ParticleSystem> _Fx;
    
    private void SetLevelRange()
    {
        int maxLevel = 4;

        List<FruitsObject> list = FruitsManager.Instance.SpawnObjects;

        for (int i = 0; i < list.Count; ++i)
        {
            Fruit fruit = list[i].FruitData.FruitMetaData;

            if (fruit.level > maxLevel)
            {
                maxLevel = fruit.level;
            }
        }

        _maxFruitLevel = maxLevel;
        
    }

    private void Update()
    {
        AdsRegame();
    }

    private void AdsRegame()
    {
        if (Ads.Instance.isAds)
        {
            Ads.Instance.isAds = false;
            DataManager.Instance.MaxFruitBox();
            InGameUI.Instance.RefreshCurrency();
            SoundManager2.Instance.BgmPlaySound("Ingame");
            FruitsManager.Instance.SpawnObject(Vector3.zero);
            Debug.Log("박스 충전");
            TimeHelper.SetTime(TimeType.PLAY);
        }
    }
}
