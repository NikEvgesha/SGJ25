using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    //[SerializeField] private Inventory _inventory;
    [SerializeField] private GameObject _gameUI;
    //[SerializeField] private GameObject _lobbyUI;
    //[SerializeField] private PlayerManager _player;
    //[SerializeField] private UI _ui;
    [SerializeField] private Transform _spawnPoint;



    private void Start()
    {
        //Instantiate(_lobbyUI);
        Instantiate(_gameUI);
        Instantiate(_player);
        _player.transform.position = _spawnPoint.position;
        G.GameLoader.ShowLoadingImage(false);
        G.Game.OnGameStart();
    }
    public void _StartGame()
    {
        G.Control.CursorActive = false;
    }
}