using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PointStend : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            // куда ставить предмет
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private InteractionPanel _interactionPanelGet;

    [Header("Endless Item")]
    [SerializeField] private ItemTaked _endlessItem;

    private bool _isEndless;
    // --- Runtime ---
    [Header("Test")]
    [SerializeField] private ItemTaked _currentItem; // что сейчас стоит на подставке
    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _isEndless = _endlessItem != null;

        if (_isEndless)
            _currentItem = _endlessItem;
        
    }
    private void Start()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {
        if (!G.Player.Hand.IsEmpty)
        {
            if (_isEndless || !G.Player.Hand.CurrentItem.TryPut())
            {
                _currentItem?.UseOutline(false);
                _interactionPanel.gameObject.SetActive(false);
                _interactionPanelGet.gameObject.SetActive(false);
                return;
            }
        }

        if (_currentItem)
        {
            _currentItem.UseOutline(isView);
            _interactionPanelGet.gameObject.SetActive(isView);
            _interactionPanel.gameObject.SetActive(false);
        }  
        else
        {
            if (!G.Player.Hand.IsEmpty)
            {
                _interactionPanel.gameObject.SetActive(isView);
                _interactionPanelGet.gameObject.SetActive(false);
            }
            else
            {
                _interactionPanel.gameObject.SetActive(false);
                _interactionPanelGet.gameObject.SetActive(false);
            }
        }
    }


    public void Interact()
    {
        if (!_currentItem)
        {

            if (G.Player.Hand.IsEmpty)
                return;
            _currentItem = G.Player.Hand.CurrentItem;
            _currentItem.PutDown(_pointSocket);
        } 
        else
        {
            ItemTaked item = _currentItem;
            _currentItem = null;
            if (_isEndless)
            {
                _currentItem = Instantiate(item, _pointSocket);
                _currentItem?.UseOutline(false);
            }
            else if (!G.Player.Hand.IsEmpty)
            {
                _currentItem = G.Player.Hand.CurrentItem;
                _currentItem.PutDown(_pointSocket);
                item.Take();
                return;
            }

            item.Take();
        }
    }


}
