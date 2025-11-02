using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _hint;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _insightPrice;
    private Ingredient _ingredient;
    private bool _unlocked;
    private int _unlockPrice;
    public UnityEvent<Ingredient> IngredientUnlocked;

    public void SetData(Ingredient ingredient, int price, bool unlocked)
    {
        _unlocked = unlocked;
        _ingredient = ingredient;
        _unlockPrice = price;

        _icon.sprite = _ingredient.Data.Icon;
        _hint.text = _ingredient.Data.hint;
        _name.text = _ingredient.Data.Name;
        _insightPrice.text = _unlockPrice.ToString();
        _icon.gameObject.SetActive(_unlocked);
        _hint.transform.parent.gameObject.SetActive(false);
        _name.transform.parent.gameObject.SetActive(false);
        _insightPrice.transform.parent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (_unlocked)
        {
            _name.transform.parent.gameObject.SetActive(true);
        } else
        {
            _hint.transform.parent.gameObject.SetActive(true);
            _insightPrice.transform.parent.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (_unlocked)
        {
            _name.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _hint.transform.parent.gameObject.SetActive(false);
            _insightPrice.transform.parent.gameObject.SetActive(false);
        }
    }

    public void _OnClick()
    {
        if (_unlocked) return;

        if (G.Currency.RemoveCurrency(CurrencyType.Insight, _unlockPrice))
        {
            _unlocked = true;
            _insightPrice.transform.parent.gameObject.SetActive(false);
            _hint.transform.parent.gameObject.SetActive(false);
            _name.transform.parent.gameObject.SetActive(true);
            _icon.gameObject.SetActive(true);
            IngredientUnlocked?.Invoke(_ingredient);
        }
    }



}
