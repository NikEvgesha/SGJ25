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
        foreach (Recipe recipe in _recipes)
        {
            _unlockedStatus.Add(recipe, 0);
        }
        Debug.Log("Set recipe book");
        G.Game.SetRecipeBook(this);
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
        _animator.SetTrigger("Next");
        if (_currentRecipeLastIndex + 2 < _recipes.Count)
        {
            _rightPage.SetRecipe(_recipes[_currentRecipeLastIndex + 2]);
            _currentRecipeLastIndex++;
        }
        else
        {
            _rightPage.SetRecipe();
        }
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
    }

}
