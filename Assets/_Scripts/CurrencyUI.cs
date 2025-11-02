using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private CurrencyType _type;
    private TextMeshProUGUI _amountText;

    private void Awake()
    {
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        G.Currency.CurrencyChanged.AddListener(UpdateUI);
    }

    private void Start()
    {
        _amountText.text = ((_type == CurrencyType.Coin? G.Currency.Coins : G.Currency.Insight)).ToString();
    }

    private void UpdateUI(CurrencyType type, int amount)
    {
        if (type == _type)
            _amountText.text = amount.ToString();
    }
}
