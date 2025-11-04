using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameEndPanel;
    [SerializeField] private GameObject _restart;
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
        _restart.SetActive(false);
        G.Input.AMenu += OpenSettings;
        G.Input.ABook += OpenRecipeBook;
        G.Game.BookReady.AddListener(() => { 
            G.Game.RecipeBook.NewRecipe.AddListener(ShowRecipeNotification);
            G.Game.RecipeBook.StoneCreated.AddListener(OnStoneCreated);
            });
        
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

    private void OnStoneCreated()
    {
        _gameEndPanel.gameObject.SetActive(true);
        G.Control.CursorActive = true;
    }

    public void OnStoneReject()
    {
        _gameEndPanel.gameObject.SetActive(false);
        G.Control.CursorActive = false;
    }

    public void OnStoneAccepted()
    {
        G.Control.CursorActive = false;
        _gameEndPanel.gameObject.SetActive(false);
        G.Game.OnStoneAccepted();
        _restart.SetActive(true);
    }

    public void OnRestart()
    {
        G.Game.Restart();
    }

}
