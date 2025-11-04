using UnityEngine;

public class GameEndRoom : MonoBehaviour
{
    [SerializeField] private Transform _teleportPoint;

    public Transform TeleportPoint => _teleportPoint;

    private void Start()
    {
        G.Game.SetGameEndRoom(this);
    }
}
