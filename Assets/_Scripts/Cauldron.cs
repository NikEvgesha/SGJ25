using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cauldron : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private GameObject _water;
    private AudioSource _audio;

    private List<Ingredient> _ingredients = new();

    private List<ItemTaked> _listItemsInСauldron = new();
    private Outline _outline;

    public UnityEvent<List<Ingredient>> IngredientsUpdated;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _water.SetActive(false);
        G.Game.GameStart.AddListener(StartPoint);
        _audio = GetComponent<AudioSource>();
    }
    private void Start()
    {
        G.Game.SetCauldron(this);
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
        G.Player.Hand.CurrentItem.PutDown(_pointSocket,false,true);
        AddItem(item);
    }
    private void AddItem(ItemTaked item)
    {
        _ingredients.Add(item.Item.Ingredient);
        _water.SetActive(true);
        IngredientsUpdated?.Invoke(_ingredients);
        _audio.Play();
        if (_ingredients.Count >= 3)
        {
            _water.SetActive(false);
            CheckRecept();
            foreach (var ingredient in _ingredients)
            {
                Destroy(ingredient.gameObject);
            }
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
