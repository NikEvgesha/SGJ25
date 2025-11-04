using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private RecipeBook _recipeBook;
    private Cauldron _cauldron;
    private GameEndRoom _gameEndRoom;

    public RecipeBook RecipeBook => _recipeBook;
    public Cauldron Cauldron => _cauldron;
    public GameEndRoom GameEndRoom => _gameEndRoom;

    [HideInInspector]
    public UnityEvent<bool> GameEnd;
    public UnityEvent GameStart;
    public UnityEvent BookReady;
    public UnityEvent CauldronReady;



    private void Awake()
    {
        if (G.Game == null)
        {
            G.Game = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void OnGameStart()
    {
        GameStart?.Invoke();
        G.IsPaused = false;
        //G.Control.CursorActive = true;
        //G.Control.CursorActive = false;
    }

    public void OnGameEnd(bool win = false)
    {
        G.IsPaused = true;
        //G.GameLoader.ShowGameEndImage(true);
        // check if win
        GameEnd?.Invoke(win /*win*/);
        Transform spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
        //G.PlayerStatManager.transform.position = spawnPoint.position;
        //G.SaveManager.SaveCollection(G.Inventory.Collected);
        //G.GameLoader.ShowGameEndImage(false);
    }

    public void SetRecipeBook(RecipeBook book)
    {
        _recipeBook = book;
        BookReady?.Invoke();
    }

    public void SetCauldron(Cauldron cauldron)
    {
        _cauldron = cauldron;
        CauldronReady?.Invoke();
    }

    public void OnStoneAccepted()
    {
        G.Player.Hand.CurrentItem.PutDown(G.Player.transform, true, true);
        G.Player.Teleport(_gameEndRoom.TeleportPoint);
    }

    public void SetGameEndRoom(GameEndRoom room)
    {
        _gameEndRoom = room;
    }

    public void Restart()
    {
        G.SaveManager.Reset();
        G.Currency.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}