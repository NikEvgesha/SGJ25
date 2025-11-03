using UnityEngine;

public class GameUI : MonoBehaviour
{
    private SettingUI settings;


    private void Awake()
    {
        settings = GetComponentInChildren<SettingUI>();
    }
    private void Start()
    
    {
        G.Input.AMenu += OpenSettings;
        G.Input.ABook += OpenRecipeBook;
    }
    public void OpenRecipeBook()
    {
        G.Game.RecipeBook.ToggleOpen();
    }
    public void OpenSettings()
    {
        settings.ToggleOpen();
    }
}
