using UnityEngine;

public class GameUI : MonoBehaviour
{
    private NewRecipeNotification _notification;
    private SettingUI settings;


    private void Awake()
    {
        settings = GetComponentInChildren<SettingUI>();
        _notification = GetComponentInChildren<NewRecipeNotification>();
        _notification.gameObject.SetActive(false);
    }
    private void Start() 
    {
        G.Input.AMenu += OpenSettings;
        G.Input.ABook += OpenRecipeBook;
        G.Game.BookReady.AddListener(() => { G.Game.RecipeBook.NewRecipe.AddListener(ShowRecipeNotification);});
    }

    

    private void OnDisable()
    {
        G.Input.AMenu -= OpenSettings;
        G.Input.ABook -= OpenRecipeBook;
    }
    public void OpenRecipeBook()
    {
        G.Game.RecipeBook.ToggleOpen();
    }
    public void OpenSettings()
    {
        settings.ToggleOpen();
    }

    private void ShowRecipeNotification(Recipe recipe)
    {
        _notification.gameObject.SetActive(true);
        _notification.Init(recipe);
    }

}
