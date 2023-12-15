using System.Collections;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameUI : GameObjectSingleton<InGameUI>
{
    [SerializeField] private Text _fruitBoxText;
    [SerializeField] private Text _coinText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _genCoinText;
    [SerializeField] private Image _nextFruitImage;

    [SerializeField] private Text[] _skewersCoinsText;
    [SerializeField] private Text[] _refreshFruitText;
    [SerializeField] private GameObject[] _onSellButton;
    
    public Image NextFruitImage => _nextFruitImage;
    
    private void Start()
    {
        InitData();
        
        InGameManager.Instance.Init();
        SetButtonAlpha();
        
        Ads.Instance.ActiveAds(AdsType.BANNER,true);
        
        SoundManager2.Instance.BgmPlaySound("Ingame");
        TimeHelper.SetTime(TimeType.PLAY);
        
    }
    
    
    
    
    private void InitData()
    {
        _fruitBoxText.text = $"{DataManager.Instance.UserData.FruitBox} / {SpecDataManager.Instance.GameConfig.Get(10001).value}";
        _coinText.text = $"{FruitsManager.Instance.Coin}";
        _scoreText.text = $"{FruitsManager.Instance.Score}";
        _genCoinText.text = $"{SpecDataManager.Instance.GenCoin.Get(2000 + FruitsManager.Instance.GenCoin).gencoin_value}";

        for (int i = 0; i < _refreshFruitText.Length; i++)
        {
            _refreshFruitText[i].text = $"{SpecDataManager.Instance.GameConfig.Get(10002).value}";
        }
    }

    public void RefreshCurrency()
    {
        _fruitBoxText.text = $"{DataManager.Instance.UserData.FruitBox} / {SpecDataManager.Instance.GameConfig.Get(10001).value}";
        _coinText.text = $"{FruitsManager.Instance.Coin}";
        _scoreText.text = $"{FruitsManager.Instance.Score}";
        _genCoinText.text = $"{SpecDataManager.Instance.GenCoin.Get(2000 + FruitsManager.Instance.GenCoin).gencoin_value}";
    }

    public IEnumerator ChangeTextColor(Text text, Color color)
    {
        text.color = color;
        yield return new WaitForSeconds(0.1f);
        text.color = Color.white;
    }
    private void SetButtonAlpha()
    {
        for (int i = 0; i < _onSellButton.Length; i++)
        {
            InGameManager.Instance.SetTargetFruitAlpha(GetSellButtonImage(i), 0.5f);
            InGameManager.Instance.SetTargetFruitAlpha(GetSellButtonChildrenImage(i), 0.5f);
        }
    }
    
    public void OnClickSell(int index)
    {
        if (!InGameManager.Instance.isSell[index])
        {
            ToastPop.ShowPop("과일 재료가 부족합니다.", Duration.SHORT); //spec으로 빼도 됨
            return;
        }

        SoundManager2.Instance.SfxPlaySound("AddCoin");
        
        InGameManager.Instance.ClearFruit(index, false);
        
        InGameManager.Instance.ChangeTargetTangHuru(index);
        
        InGameManager.Instance.PlayParticle(ParticleType.ADD_COIN,index);
        
        InGameManager.Instance.CheckFruit();
        
        RefreshCurrency(); // 임시 처리
        
        StartCoroutine(ChangeTextColor(_coinText, Color.green));
    }

    //과일 교채 버튼
    public void OnClickRefreshFruit(int index)
    {
        if (FruitsManager.Instance.Coin < SpecDataManager.Instance.GameConfig.Get(10002).value)
        {
            ToastPop.ShowPop($"코인 {SpecDataManager.Instance.GameConfig.Get(10002).value} 부족합니다.", Duration.SHORT); //spec으로 빼도 됨
            return;
        }
        SoundManager2.Instance.SfxPlaySound("ChangeFruit");
        
        InGameManager.Instance.ClearFruit(index, true);
        
        InGameManager.Instance.ChangeTargetTangHuru(index);
        
        InGameManager.Instance.PlayParticle(ParticleType.REFRESH_FRUIT,index);
        
        InGameManager.Instance.CheckFruit();
        
        RefreshCurrency(); // 임시 처리
        
        StartCoroutine(ChangeTextColor(_coinText, Color.red));
    }
    
    public void UpdateNextFruitImage(ObjectTag objectTag)
    {
        _nextFruitImage.sprite = SpriteManager.Instance.GetSprite(objectTag);
    }

    public void OnClickOption()
    {
        OptionPop.ShowPop();
        SoundManager2.Instance.SfxPlaySound("Click");
    }
    
    public void UpdateSkewersCoinText(int index, int coin, bool isWaterMelon = false)
    {
        if (isWaterMelon)
        {
            _skewersCoinsText[index].text = "점수x2";
            _skewersCoinsText[index].color = Color.red;
        }
        else
        {
            _skewersCoinsText[index].text = coin.ToString();
            _skewersCoinsText[index].color = Color.black;
        }
    }
    
    public Image GetSellButtonImage(int index)
    {
        Image image = _onSellButton[index].GetComponent<Image>();
        return image;
    }
    
    public Image GetSellButtonChildrenImage(int index)
    {
        Image image = _onSellButton[index].GetComponentsInChildren<Image>()[1];
        return image;
    }
    
}
