using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    //[SerializeField] private PlayerController _player;
    //[SerializeField] private Inventory _inventory;
    [SerializeField] private GameObject _gameUI;
    //[SerializeField] private GameObject _lobbyUI;
    //[SerializeField] private PlayerManager _player;
    //[SerializeField] private UI _ui;
    


    private void Start()
    {
        //Instantiate(_lobbyUI);
        Instantiate(_gameUI);

        G.GameLoader.ShowLoadingImage(false);
        G.Game.OnGameStart();
    }
}