using System.Collections;
using System.Collections.Generic;
using CookApps;
using CookApps.LocalData;
using UnityEngine;

public class DataManager : GameObjectSingleton<DataManager>
{
    public UserData UserData => _userData;
    
    private CookAppsLocalData _localData;
    private static readonly string FILE_NAME = "local_data_TanghuRookiez.dat";

    private UserData _userData;
    
    protected override void Awake()
    {
        base.Awake();

        SpecDataManager.Instance.LoadFromResource();

        if (_localData == null)
        {
            _localData = new CookAppsLocalData(LocalDataSample.GetKey());
            if (_localData.TryLoad(FILE_NAME, out _userData) == false)
            {
                _userData = new UserData();
            }
        }
    }

    public void SaveData()
    {
        CookAppsLocalData.EnumSaveResult enumSaveResult = _localData.Save(_userData, FILE_NAME);
        
        switch (enumSaveResult)
        {
            case CookAppsLocalData.EnumSaveResult.SUCCESS:
                Debug.Log("저장완료!");
                break;
            case CookAppsLocalData.EnumSaveResult.FAIL_UNKNOWN:
            case CookAppsLocalData.EnumSaveResult.FAIL_DISK_FULL:
                Debug.LogError($"저장에 실패했습니다. 이유 : {enumSaveResult}. 디바이스의 디스크 공간을 확보하세요.");
                break;
        }
    }

    public bool CheckCanUseCurrency(Currency currency, int cost)
    {
        bool canUse = false;
        
        switch (currency)
        {
            case Currency.COIN:
                if (_userData.Coin >= cost)
                    canUse = true;
                break;
            case Currency.FRUIT_BOX:
                if (_userData.FruitBox >= cost)
                    canUse = true;
                break;
        }

        return canUse;
    }

    public void UseCurrency(Currency currency, int cost)
    {
        switch (currency)
        {
            case Currency.COIN:
                _userData.Coin -= cost;
                break;
            case Currency.FRUIT_BOX:
                _userData.FruitBox -= cost;
                break;
        }
        
        SaveData();
    }

    public void ChangeFruitBoxFromCoin()
    {
        //여기까지 들어온거면 FRUITBOX가 없다라는 기준으로 함수를 실행하는 것
        
        int changeCoin = SpecDataManager.Instance.GenCoin.Get(2000 + FruitsManager.Instance.GenCoin).gencoin_value;
        if (FruitsManager.Instance.Coin >= changeCoin)
        {
            FruitsManager.Instance.UseCoin(changeCoin);
            _userData.FruitBox += SpecDataManager.Instance.GameConfig.Get(10001).value; //max
            
            ToastPop.ShowPop($"코인 {SpecDataManager.Instance.GenCoin.Get(2000 + FruitsManager.Instance.GenCoin).gencoin_value}을 소모하여 과일을 충전 하였습니다."); //spec으로 빼도 됨
            
            FruitsManager.Instance.AddGenCoin();
            
            SaveData();
        }
        
        FruitsManager.Instance.CheckGenCoin();
    }

    public void MaxFruitBox(int box = 0)
    {
        if (box > 0)
        {
            _userData.FruitBox = box;
        }
        else
        {
            _userData.FruitBox = SpecDataManager.Instance.GameConfig.Get(10001).value;
        }
        
    }

    public void UpdateBestScore(int bestScore)
    {
        if (_userData.BestScore >= bestScore)
            return;
        
        _userData.BestScore = bestScore;
        SaveData();
    }
}
