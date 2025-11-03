using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    //[SerializeField] private Inventory _inventory;
    [SerializeField] private GameObject _gameUI;
    //[SerializeField] private GameObject _lobbyUI;
    //[SerializeField] private PlayerManager _player;
    //[SerializeField] private UI _ui;
    


    private void Start()
    {
        //Instantiate(_lobbyUI);
        Instantiate(_gameUI);
        Instantiate(_player);

        G.GameLoader.ShowLoadingImage(false);
        G.Game.OnGameStart();
    }
}