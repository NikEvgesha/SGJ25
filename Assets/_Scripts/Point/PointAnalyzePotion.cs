using UnityEngine;

public class PointAnalyzePotion : MonoBehaviour
{

    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            // куда ставить предмет
    [SerializeField] private InteractionPanel _interactionPanel;

    private void Start()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {
        if (!G.Player.Hand.IsEmpty && !G.Player.Hand.CurrentItem.TryPut())
        {
            _interactionPanel.gameObject.SetActive(false);
            return;
        }

        if (!G.Player.Hand.IsEmpty)
        {
            _interactionPanel.gameObject.SetActive(isView);
        }
        else
        {
            _interactionPanel.gameObject.SetActive(false);
        }

    }

    public void _Interact()
    {
        if (G.Player.Hand.IsEmpty)
            return;
        AnalyzeItem();
    }

    private void AnalyzeItem()
    {
        //Взять цену из потиона и дать игроку
        G.Player.Hand.CurrentItem.PutDown(_pointSocket, true);
    }
}
