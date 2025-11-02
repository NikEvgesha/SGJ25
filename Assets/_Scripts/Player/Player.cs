using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(HandHolder))]
public class Player : MonoBehaviour
{
    private PlayerController _controller;
    private HandHolder _handHolder;

    private void Awake()
    {
        G.Player = this;
        _controller = GetComponent<PlayerController>();
        _handHolder = GetComponent<HandHolder>();
    }
    public PlayerController Controller => _controller;
    public HandHolder Hand => _handHolder;
}
