using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Сauldron : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private GameObject _water;

    private List<Ingredient> _ingredients = new();

    private List<ItemTaked> _listItemsInСauldron = new();
    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _water.SetActive(false);
        G.Game.GameStart.AddListener(StartPoint);
    
    }
    private void StartPoint()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {

        if (G.Player.Hand.IsEmpty)
        {
            _outline.enabled = false;
            _interactionPanel.gameObject.SetActive(false);
            return;
        } 
        _outline.enabled = isView;
        _interactionPanel.gameObject.SetActive(isView);
        
    }

    public void Interact()
    {
        ItemTaked item = G.Player.Hand.CurrentItem;
        G.Player.Hand.CurrentItem.PutDown(_pointSocket,true);
        AddItem(item);
    }
    private void AddItem(ItemTaked item)
    {
        _ingredients.Add(item.Item.Ingredient);
        _water.SetActive(true);
        if (_ingredients.Count >= 3)
        {
            _water.SetActive(false);
            CheckRecept();
            _ingredients.Clear();
        }
    }
    private void CheckRecept()
    {
        Ingredient newIngredient = G.Game.RecipeBook.CheckRecipe(_ingredients);
        if (!newIngredient)
        {
            return;
        }
        ItemTaked newItem = Instantiate(newIngredient, _pointSocket).GetComponent<ItemTaked>();
        newItem.Take();
    }
}
