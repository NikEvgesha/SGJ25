using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private CurrencyType _type;
    [SerializeField] private TextMeshProUGUI _diffText;
    private TextMeshProUGUI _amountText;

    private int _currentAmount;

    private float _changeDuration = 0.5f;
    private int _diff;
    private Vector3 _initPos;
    private Vector3 _shiftedPos;
    private Color _greenT = new Color(0,1,0,0);
    private Color _redT = new Color(1, 0, 0, 0);

    private float _noCurrencyDuration = 0.5f;
    private Color _amountColor;

    private void Awake()
    {
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
        _initPos = _diffText.transform.position;
        _shiftedPos = new Vector3(_initPos.x, _initPos.y - 80, _initPos.z);
        _diffText.gameObject.SetActive(false);
        _amountColor = _amountText.color;
    }

    private void OnEnable()
    {
        G.Currency.CurrencyChanged.AddListener(UpdateUI);
        G.Currency.NoCurrency.AddListener(NotEnoughCurrency);
    }

    private void Start()
    {
        _currentAmount = _type == CurrencyType.Coin ? G.Currency.Coins : G.Currency.Insight;
        _amountText.text = _currentAmount.ToString();
    }

    private void NotEnoughCurrency(CurrencyType type)
    {
        if (type == _type)
        {
            StopCoroutine(NoCurrencyAnimation());
            StartCoroutine(NoCurrencyAnimation());
        }
    }

    private void UpdateUI(CurrencyType type, int amount)
    {
        if (type == _type)
        {
            _diff = amount - _currentAmount;
            _currentAmount = amount;
            StopCoroutine(DiffAnimation());
            StartCoroutine(DiffAnimation());
        }
            
    }
    private IEnumerator DiffAnimation()
    {
        bool positive = _diff >= 0;
        _diffText.text = positive ? "+" + _diff.ToString() : _diff.ToString();
        _diffText.color = positive ? Color.green : Color.red;
        _diffText.transform.position = positive ? _shiftedPos : _initPos;
        _diffText.gameObject.SetActive(true);
        float timer = 0;

        while (timer < _changeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / _changeDuration;
            if (positive)
            {
                _diffText.transform.position = Vector3.Lerp(_shiftedPos, _initPos, t);
                _diffText.color = Color.Lerp(Color.green, _greenT, t);
            } else
            {
                _diffText.transform.position = Vector3.Lerp(_initPos, _shiftedPos, t);
                _diffText.color = Color.Lerp(Color.red, _redT, t);
            }
            yield return null;
        }
        _diffText.gameObject.SetActive(false);
        _amountText.text = _currentAmount.ToString();
    }


    private IEnumerator NoCurrencyAnimation()
    {
        float timer = 0;
        float halfTime = _noCurrencyDuration / 2;
        _amountText.color = _amountColor;
        _amountText.transform.localScale = Vector3.one;

        while (timer < halfTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / halfTime;
            _amountText.color = Color.Lerp(_amountColor, Color.red, t);
            _amountText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.3f, t);
            yield return null;
        }
        timer = 0;
        while (timer < halfTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / halfTime;
            _amountText.color = Color.Lerp(Color.red, _amountColor, t);
            _amountText.transform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t);
            yield return null;
        }
        _amountText.color = _amountColor;
        _amountText.transform.localScale = Vector3.one;
    }
}
