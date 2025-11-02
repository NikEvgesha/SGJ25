using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePage : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _icon;
    //[SerializeField] private GameObject _lockedPanel;

    private List<IngredientSlot> _ingredientSlots;
    private Recipe _recipe;
    private int _status;

    private void Start()
    {
        _ingredientSlots = GetComponentsInChildren<IngredientSlot>().ToList();
        foreach (var slot in _ingredientSlots)
        {
            slot.IngredientUnlocked.AddListener(OnIngredientUnlock);
        }
    }

    public void SetRecipe(Recipe recipe=null)
    {
        _recipe = recipe;
        if (recipe == null)
        {
            _panel.SetActive(false);
            return;
        }


        _panel.SetActive(true);
        if (recipe.Ingredients.Count != _ingredientSlots.Count)
        {
            Debug.LogError("RecipePage: slots.Count != ingredients.Count");
            return;
        }

        _nameText.text = recipe.Name;
        _descriptionText.text = recipe.Description;
        _icon.sprite = recipe.Icon;

        _status = G.Game.RecipeBook.GetUnlockStatus(recipe);

        //if (_status == -1)
        //{
        //    for (int i = 0; i < recipe.Ingredients.Count; i++)
        //    {
        //        _ingredientSlots[i].SetData(recipe.Ingredients[i], false);
        //    }
        //    _lockedPanel.SetActive(true);
        //} else
        //{
            for (int i = recipe.Ingredients.Count-1; i >= 0; i--)
            {
                _ingredientSlots[i].SetData(recipe.Ingredients[i], ((_status >> i) & 1) == 1);
            }
            //_lockedPanel.SetActive(false);
        //}

            

    }

    private void OnIngredientUnlock(Ingredient ingredient)
    {
        for (int i = 0; i < _recipe.Ingredients.Count; i++) 
        {
            if (_recipe.Ingredients[i].Ingredient == ingredient)
            {
                Debug.Log("Check index: " + i);
                G.Game.RecipeBook.UpdateUnlockStatus(_recipe, i);
                return;
            }
        }

    }

}
