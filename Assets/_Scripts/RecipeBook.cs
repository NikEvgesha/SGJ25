using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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

    int currentLeft;
    int currentRight;
    int prevRight;
    int prevLeft;
    int nextRight;
    int nextLeft;

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
        G.Control.CursorActive = true;
    }

    public void Close()
    {
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
                bool haveIngredient = false;
                for (int j = 0; j < recipe.Ingredients.Count; j++)
                {
                    if (ingredients[i].Data.Name == recipe.Ingredients[j].Data.Name)
                    {
                        haveIngredient = true;
                        break;
                    }
                }

                if (!haveIngredient)
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
