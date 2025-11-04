using System.Collections.Generic;
using UnityEngine;

public class PointBuyIngredient : MonoBehaviour
{
    [SerializeField] private List<PointIngredient> _ingredients = new();

    [SerializeField] private Transform _pointSocket;
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private int _startOpenIndex = 2;

    [Header("Endless Item")]
    [SerializeField] private ItemTaked _ingredientItem;

    private int _indexLastBuy = 0;

    private void Start()
    {
        foreach (PointIngredient item in _ingredients)
        {
            item.gameObject.SetActive(false);
        }
        _indexLastBuy = G.SaveManager.LoadIngredients();
        if (_indexLastBuy < _startOpenIndex)
        {
            _indexLastBuy = _startOpenIndex;
            G.SaveManager.SaveIngredients(_indexLastBuy);
        }
        for (int i = 0; i < _indexLastBuy; i++)
        {
            _ingredients[i].gameObject.SetActive(true);
        }
        GetNextIngredients();
        if (_ingredientItem == null)
        {
            gameObject.SetActive(false);
            return;
        }
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {
        if (_ingredientItem)
        {
            _ingredientItem.UseOutline(isView);
            _interactionPanel.SetInfo("Купить", _ingredientItem.Item.Data.BuyPrice.ToString());
        }

        _interactionPanel.gameObject.SetActive(isView);
    }
    public void _Interact()
    {
        if (TryBuy())
        {
            AddNewIngredient();
            _PlayerView(true);
        }
    }
    private bool TryBuy()
    {
        return G.Currency.RemoveCurrency(CurrencyType.Coin, _ingredientItem.Item.Data.BuyPrice);
    }
    private void AddNewIngredient()
    {
        _ingredients[_indexLastBuy].gameObject.SetActive(true);
        _ingredientItem.MoveToParent(_ingredients[_indexLastBuy].Point,true);
        _ingredientItem = null;
        _indexLastBuy++;
        G.SaveManager.SaveIngredients(_indexLastBuy);
        GetNextIngredients();
    }
    private void GetNextIngredients()
    {
        if (_indexLastBuy >= _ingredients.Count)
        {
            gameObject.SetActive(false);
            return;
        }
        _ingredientItem = Instantiate(_ingredients[_indexLastBuy].GetIngredient(), _pointSocket);

    }
}
