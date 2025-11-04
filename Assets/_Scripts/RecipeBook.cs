using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private List<Recipe> _recipes;
    [SerializeField] private Recipe _failRecipe;
    [SerializeField] private Recipe _stoneRecipe;
    [SerializeField] RecipePage _leftPage;
    [SerializeField] RecipePage _rightPage;
    [SerializeField] RecipePage _dynamicPageLeft;
    [SerializeField] RecipePage _dynamicPageRight;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private AudioClip _newRecipeSound;
    [SerializeField] private AudioClip _pageSound;

    private Dictionary<string, HashSet<Recipe>> _ingredientRecipes;
    private AudioSource _audio;
    private GameObject _navigationButtons;
    private Animator _animator;
    private bool _opened;
    private int _currentRecipeLastIndex;
    private Dictionary<Recipe, int> _unlockedStatus; // 0 .. 7

    private int currentLeft;
    private int currentRight;
    private int prevRight;
    private int prevLeft;
    private int nextRight;
    private int nextLeft;

    public UnityEvent<Recipe> NewRecipe;
    public UnityEvent StoneCreated;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navigationButtons = _nextButton.transform.parent.gameObject;
        _navigationButtons.SetActive(false);
        _unlockedStatus = new Dictionary<Recipe, int>();
        _audio = GetComponent<AudioSource>();

        _ingredientRecipes = new();

    }

    void Start()
    {
        foreach (Recipe r in _recipes)
        {
            foreach (Ingredient i in r.Ingredients)
            {
                if (!_ingredientRecipes.ContainsKey(i.Data.Name))
                {
                    _ingredientRecipes.Add(i.Data.Name, new HashSet<Recipe>());
                }
                _ingredientRecipes[i.Data.Name].Add(r);
            }
        }

        foreach (Ingredient i in _stoneRecipe.Ingredients)
        {
            if (!_ingredientRecipes.ContainsKey(i.Data.Name))
            {
                _ingredientRecipes.Add(i.Data.Name, new HashSet<Recipe>());
            }
            _ingredientRecipes[i.Data.Name].Add(_stoneRecipe);
        }


        G.Game.SetRecipeBook(this);
        List<int> statuses = G.SaveManager.LoadRecipeStatuses();

        if (statuses.Count == _recipes.Count)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                _unlockedStatus.Add(_recipes[i], statuses[i]);
            }
        }
        else
        {
            foreach (Recipe recipe in _recipes)
            {
                _unlockedStatus.Add(recipe, 0);
            }
        }

        _unlockedStatus.Add(_stoneRecipe, 0);
    }


    public void Open()
    {
        _opened = true;
        _leftPage.SetRecipe();
        _rightPage.SetRecipe(_recipes[0]);
        _currentRecipeLastIndex = 0;
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
        //_navigationButtons.SetActive(true);
        _animator.SetTrigger("Open");
        CheckNavigation();
        _audio.PlayOneShot(_pageSound);
        G.Control.CursorActive = true;
    }

    public void Close()
    {
        _opened = false;
        _animator.SetTrigger("Close");
        //_navigationButtons.SetActive(false);
        G.Control.CursorActive = false;
    }

    public void ToggleOpen()
    {
        _opened = !_opened;
        _animator.SetTrigger(_opened ? "Open" : "Close");
        if (_opened)
        {
            Open();
        }
        else
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
            _currentRecipeLastIndex += 2;
        }
        else
        {
            _rightPage.SetRecipe();
            if (_currentRecipeLastIndex + 1 < _recipes.Count)
                _currentRecipeLastIndex++;
        }
        _animator.SetTrigger("Next");
        _audio.PlayOneShot(_pageSound);
        CheckNavigation();
    }

    public void NextComplete()
    {
        if (_currentRecipeLastIndex % 2 != 0)
        {
            _leftPage.SetRecipe(_recipes[_currentRecipeLastIndex]);
        }
        else
        {
            _leftPage.SetRecipe(_recipes[_currentRecipeLastIndex-1]);
        }
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
        //CheckNavigation();
    }


    public void PreviousPage()
    {

        if (_currentRecipeLastIndex % 2 == 0) {
            currentLeft = _currentRecipeLastIndex - 1;
            prevRight = _currentRecipeLastIndex - 2;
            prevLeft = _currentRecipeLastIndex - 3;
            _currentRecipeLastIndex -= 2;
        } else
        {
            currentLeft = _currentRecipeLastIndex;
            prevRight = _currentRecipeLastIndex - 1;
            prevLeft = _currentRecipeLastIndex - 2;
            _currentRecipeLastIndex--;
        }
        _dynamicPageRight.SetRecipe(_recipes[currentLeft]);

        if (prevRight >= 0)
            _dynamicPageLeft.SetRecipe(_recipes[prevRight]);


        _dynamicPageLeft.transform.parent.gameObject.SetActive(true);
        
        if (prevLeft >= 0)
            _leftPage.SetRecipe(_recipes[prevLeft]);
        else
        {
            _leftPage.SetRecipe();
        }
        _animator.SetTrigger("Previous");
        _audio.PlayOneShot(_pageSound);
        CheckNavigation();
    }

    public void PreviousComplete()
    {
        if (_currentRecipeLastIndex >= 0)
        {
            _rightPage.SetRecipe(_recipes[_currentRecipeLastIndex]);
        }
        else
        {
            _rightPage.SetRecipe();
        }
        _dynamicPageLeft.transform.parent.gameObject.SetActive(false);
    }


    private void CheckNavigation()
    {
        _nextButton.gameObject.SetActive(_currentRecipeLastIndex + 1 < _recipes.Count);
        _prevButton.gameObject.SetActive(_currentRecipeLastIndex - 2 >= 0);

    }


    public int GetUnlockStatus(Recipe recipe)
    {
        return _unlockedStatus[recipe];
    }

    public int UpdateUnlockStatus(Recipe recipe, int idx=-1)
    {
        if (idx != -1)
            _unlockedStatus[recipe] = _unlockedStatus[recipe] ^ (1 << idx);
        else
            _unlockedStatus[recipe] = 7;

        if (_unlockedStatus[recipe] == 7)
        {
            _audio.PlayOneShot(_newRecipeSound);
            if (!_opened)
                NewRecipe?.Invoke(recipe);
        }

        Save();
        return _unlockedStatus[recipe];
    }

    private void Save()
    {
        List<int> statuses = new(_recipes.Count);
        foreach (Recipe recipe in _recipes)
        {
            statuses.Add(_unlockedStatus[recipe]);
        }
        G.SaveManager.SaveRecipeStatuses(statuses);
    }

    public Ingredient CheckRecipe(List<Ingredient> ingredients)
    {
        if (ingredients.Count != 3) return _failRecipe.ResultIngredient;

        //bool isStone = true;
        //foreach (Ingredient i in ingredients)
        //{
        //    if (!_stoneRecipe.Ingredients.Contains(i))
        //    {
        //        isStone = false;
        //        break;
        //    }
        //}

        //if (isStone)
        //{
        //    StoneCreated?.Invoke();
        //    return _stoneRecipe.ResultIngredient;
        //}


        HashSet<Recipe> recipes = new HashSet<Recipe>(_ingredientRecipes[ingredients[0].Data.Name]);

        for (int i = 1; i < ingredients.Count; i++)
        {
            recipes.IntersectWith(_ingredientRecipes[ingredients[i].Data.Name]);
        }

        if (recipes.Count > 0)
        {
            Recipe recipe = recipes.ToList()[0];
            if (GetUnlockStatus(recipe) != 7)
            {
                UpdateUnlockStatus(recipe);
            }
            G.Currency.AddCurrency(CurrencyType.Insight, recipe.InsightReward);
            if (recipe == _stoneRecipe)
                StoneCreated?.Invoke();
            return recipe.ResultIngredient;
        }
        return _failRecipe.ResultIngredient;

       

    }

}
