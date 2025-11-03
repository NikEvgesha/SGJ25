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
    [SerializeField] private NewRecipeNotification _notification;
    [SerializeField] private AudioClip _newRecipeSound;
    [SerializeField] private AudioClip _pageSound;

    private AudioSource _audio;
    private GameObject _navigationButtons;
    private Animator _animator;
    private bool _opened;
    private int _currentRecipeLastIndex;
    private Dictionary<Recipe, int> _unlockedStatus; // 0 .. 7

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navigationButtons = _nextButton.transform.parent.gameObject;
        _navigationButtons.SetActive(false);
        _unlockedStatus = new Dictionary<Recipe, int>();
        _audio = GetComponent<AudioSource>();
    }
    void Start()
    {
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
        _audio.PlayOneShot(_pageSound);
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
        _dynamicPageRight.SetRecipe(_recipes[_currentRecipeLastIndex - 1]);
        if (_currentRecipeLastIndex - 1 >= 0)
            _dynamicPageLeft.SetRecipe(_recipes[_currentRecipeLastIndex - 2]);


        _dynamicPageLeft.transform.parent.gameObject.SetActive(true);
        
        if (_currentRecipeLastIndex - 3 >= 0)
            _leftPage.SetRecipe(_recipes[_currentRecipeLastIndex - 3]);
        else
        {
            _leftPage.SetRecipe();
        }
        _currentRecipeLastIndex -= 2;
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
        _nextButton.gameObject.SetActive(_currentRecipeLastIndex + 2 < _recipes.Count);
        _prevButton.gameObject.SetActive(_currentRecipeLastIndex - 2 >= 0);

    }


    public int GetUnlockStatus(Recipe recipe)
    {
        return _unlockedStatus[recipe];
    }

    public int UpdateUnlockStatus(Recipe recipe, int idx)
    {
        _unlockedStatus[recipe] = _unlockedStatus[recipe] ^ (1 << idx);
        if (_unlockedStatus[recipe] == 7)
        {
            _audio.PlayOneShot(_newRecipeSound);
            //if (!_opened)
            //{
            _notification.gameObject.SetActive(true);
            _notification.Init(recipe);
            //            }

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


        foreach (Recipe recipe in _recipes)
        {
            bool check = true;
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (!recipe.Ingredients.Contains(ingredients[i]))
                {
                    check = false;
                    break;
                }
            }
            if (check)
            {
                if (GetUnlockStatus(recipe) != 7)
                {
                    UpdateUnlockStatus(recipe, 7);
                }
                return recipe.ResultIngredient;
            }

        }

        return _failRecipe.ResultIngredient;

    }

}
