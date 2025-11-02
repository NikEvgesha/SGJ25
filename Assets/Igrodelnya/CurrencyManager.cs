using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private Sprite _coinIcon;
    [SerializeField] private Sprite _insightIcon;
    [SerializeField] private int _startCoinsAmount;
    [SerializeField] private int _startInsightAmount;

    [SerializeField] private AudioClip _audioBuy;
    [SerializeField] private AudioClip _audioSell;
    [SerializeField] private AudioSource _audioSource;
    private Dictionary<CurrencyType, int> _amount;
    public int Coins { get { return _amount[CurrencyType.Coin]; } }
    public int Insight { get { return _amount[CurrencyType.Insight]; } }

    public UnityEvent<CurrencyType, int> CurrencyChanged;
    public UnityEvent NoCoins;
    public UnityEvent NoInsight;

    private void Awake()
    {
        if (G.Currency == null)
        {
            G.Currency = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _amount = new Dictionary<CurrencyType, int>
        {
            { CurrencyType.Coin, 0 },
            { CurrencyType.Insight, 0 }
        };
    }

    private void Start()
    {
        //AddCurrency(CurrencyType.Coin, G.SaveManager.LoadCoins());
        //AddCurrency(CurrencyType.Insight, G.SaveManager.LoadInsight());
        AddCurrency(CurrencyType.Coin, _startCoinsAmount);
        AddCurrency(CurrencyType.Insight, _startInsightAmount);


    }


    //public void Reset()
    //{
    //    _amount = 0;
    //    AddCurrency(_startCoinsAmount);
    //}

    public void AddCurrency(CurrencyType type, int amount)
    {
        //if (_audioSource)
        //    if(_audioSell)
        //        _audioSource.PlayOneShot(_audioSell);
        _amount[type] += amount;
        CurrencyChanged?.Invoke(type, _amount[type]);
        //G.SaveManager.SaveCoins(_amount);
       //G.SaveManager.SaveGameCoin(_balance[type]);
    }

    public bool RemoveCurrency(CurrencyType type, int amount)
    {
        if (_amount[type] >= amount)
        {
            //if (_audioSource)
            //    if (_audioBuy)
            //        _audioSource.PlayOneShot(_audioBuy);

            _amount[type] -= amount;
            CurrencyChanged?.Invoke(type, _amount[type]);
            //G.SaveManager.SaveCoins(_amount);
            //G.SaveManager.SaveGameCoin(_balance[type]);
            return true;
        }
        if (type == CurrencyType.Coin)
        {
            NoCoins?.Invoke();
        }       
        else
        {
            NoInsight?.Invoke();
        }

            return false;
    }




    public Sprite GetCurrencyIcon(CurrencyType type)
    {
        return (type == CurrencyType.Coin) ? _coinIcon : _insightIcon;
    }
    
}
