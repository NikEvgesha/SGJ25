using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;
    [SerializeField] private Recipe _failRecipe;
    [SerializeField] RecipePage _leftPage;
    [SerializeField] RecipePage _rightPage;
    [SerializeField] RecipePage _dynamicPageLeft;
    [SerializeField] RecipePage _dynamicPageRight;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _prevButton;
    private GameObject _navigationButtons;
    private Animator _animator;
    private bool _opened;
    private int _currentRecipeLastIndex;
    private Dictionary<Recipe, int> _unlockedStatus; // -1 .. 7

    [SerializeField] public int Current;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navigationButtons = _nextButton.transform.parent.gameObject;
        _navigationButtons.SetActive(false);
        _unlockedStatus = new Dictionary<Recipe, int>();
    }
    void Start()
    {
        G.Game.SetRecipeBook(this);
        List<int> statuses = G.SaveManager.LoadRecipeStatuses();
        
        if (statuses.Count != 0)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                _unlockedStatus.Add(_recipes[i], statuses[i]);
            }
        } else
        {
            foreach (Recipe recipe in _recipes)
            {
                _unlockedStatus.Add(recipe, 0);
            }
        }      
    }

    private void Update()
    {
        Current = _currentRecipeLastIndex;
    }

    public void Open()
    {
        
        _leftPage.SetRecipe();
        _rightPage.SetRecipe(_recipes[0]);
        _currentRecipeLastIndex = 0;
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
        //_navigationButtons.SetActive(true);
        _animator.SetTrigger("Open");
        CheckNavigation();
    }

    public void Close() 
    {
        _animator.SetTrigger("Close");
        //_navigationButtons.SetActive(false);
    }

    public void ToggleOpen()
    {
        _opened = !_opened;
        _animator.SetTrigger(_opened ? "Open" : "Close");
        if (_opened)
        {
            Open();
        } else
        {
            Close();
        }

    }

    public void NextPage()
    {
        _dynamicPageLeft.SetRecipe(_recipes[_currentRecipeLastIndex]);
        if (_currentRecipeLastIndex + 1 < _recipes.Count)
            _dynamicPageRight.SetRecipe(_recipes[_currentRecipeLastIndex + 1]);


        _dynamicPageLeft.transform.parent.gameObject.SetActive(true);
        
        if (_currentRecipeLastIndex + 2 < _recipes.Count)
        {
            _rightPage.SetRecipe(_recipes[_currentRecipeLastIndex + 2]);
            _currentRecipeLastIndex++;
        }
        else
        {
            _rightPage.SetRecipe();
        }
        _animator.SetTrigger("Next");
    }

    public void NextComplete()
    {
        if (_currentRecipeLastIndex < _recipes.Count)
        {
            _leftPage.SetRecipe(_recipes[_currentRecipeLastIndex]);
            _currentRecipeLastIndex++;
        }
        else
        {
            _leftPage.SetRecipe();
        }
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
        CheckNavigation();
    }


    public void PreviousPage()
    {
        _dynamicPageRight.SetRecipe(_recipes[_currentRecipeLastIndex-1]);
        if (_currentRecipeLastIndex - 1>= 0)
            _dynamicPageLeft.SetRecipe(_recipes[_currentRecipeLastIndex - 2]);


        _dynamicPageLeft.transform.parent.gameObject.SetActive(true);
        _animator.SetTrigger("Previous");
        if (_currentRecipeLastIndex - 3 >= 0)
            _leftPage.SetRecipe(_recipes[_currentRecipeLastIndex - 3]);
        else
        {
            _leftPage.SetRecipe();
        }
    }

    public void PreviousComplete()
    {
        if (_currentRecipeLastIndex - 2 >= 0)
        {
            _rightPage.SetRecipe(_recipes[_currentRecipeLastIndex - 2]);
            _currentRecipeLastIndex -= 2;
        }
        else
        {
            _rightPage.SetRecipe();
        }
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
        CheckNavigation();
    }


    private void CheckNavigation()
    {
        _nextButton.gameObject.SetActive(_currentRecipeLastIndex+1 < _recipes.Count);
        _prevButton.gameObject.SetActive(_currentRecipeLastIndex - 2 >= 0);
            
    }


    public int GetUnlockStatus(Recipe recipe)
    {
        return _unlockedStatus[recipe];
    }

    public void UpdateUnlockStatus(Recipe recipe, int idx)
    {
        _unlockedStatus[recipe] = _unlockedStatus[recipe] ^ (1 << idx);
        Save();
    }

    private void Save()
    {
        List<int> statuses = new(_recipes.Count);
        foreach (Recipe recipe in _recipes) {
            statuses.Add(_unlockedStatus[recipe]);
        }
        G.SaveManager.SaveRecipeStatuses(statuses);
    }

    public Ingredient CheckRecipe(List<Ingredient> ingredients)
    {
        if (ingredients.Count != 3) return _failRecipe.ResultIngredient;


        foreach (Recipe recipe in _recipes)
        {
            bool check = true;
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (recipe.Ingredients.Contains(ingredients[i]))
                {
                    check = false; 
                    break;
                }
            }
            if (check)
                return recipe.ResultIngredient;
        }

        return _failRecipe.ResultIngredient;

    }

}
