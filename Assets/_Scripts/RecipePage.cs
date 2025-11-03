using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePage : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _icon;
    [SerializeField] private float _revealDuration;
    //[SerializeField] private GameObject _lockedPanel;

    private List<IngredientSlot> _ingredientSlots;
    private Recipe _recipe;
    private int _status;

    public Recipe RECIPE;

    private void Start()
    {
        _ingredientSlots = GetComponentsInChildren<IngredientSlot>().ToList();
        foreach (var slot in _ingredientSlots)
        {
            slot.IngredientUnlocked.AddListener(OnIngredientUnlock);
        }
    }

    private void Update()
    {
        RECIPE = _recipe;
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

        for (int i = recipe.Ingredients.Count-1; i >= 0; i--)
        {
            _ingredientSlots[i].SetData(recipe.Ingredients[i], recipe.UnlockIngredientPrice, ((_status >> i) & 1) == 1);
        }
        
        if (_status != 7)
        {
            _icon.color = Color.black;
        } else
        {
            _icon.color = Color.white;
        }

    }

    private void OnIngredientUnlock(Ingredient ingredient)
    {
        for (int i = 0; i < _recipe.Ingredients.Count; i++) 
        {
            if (_recipe.Ingredients[i] == ingredient)
            {
                _status = G.Game.RecipeBook.UpdateUnlockStatus(_recipe, i);
                CheckUnlock();
                return;
            }
        }

    }

    private void CheckUnlock()
    {
        if (_status == 7)
        {
            StartCoroutine(UnlockRevealAnimation());
        }
    }


    private IEnumerator UnlockRevealAnimation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _revealDuration)
        {
            _icon.color = Color.Lerp(Color.black, Color.white, elapsedTime / _revealDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _icon.color = Color.white;
    }

}
