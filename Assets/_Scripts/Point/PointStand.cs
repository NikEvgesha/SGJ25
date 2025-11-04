using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PointStend : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            // куда ставить предмет
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private InteractionPanel _interactionPanelGet;
    private AudioSource _audio;

    // --- Runtime ---
    [Header("Test")]
    [SerializeField] private ItemTaked _currentItem; // что сейчас стоит на подставке

    private void Awake()
    {
        G.Game.GameStart.AddListener(StartPoint);
        _audio = GetComponent<AudioSource>();
    }
    private void StartPoint()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {
        if (!G.Player.Hand.IsEmpty && !G.Player.Hand.CurrentItem.TryPut())
        {
            _currentItem?.UseOutline(false);
            _interactionPanel.gameObject.SetActive(false);
            _interactionPanelGet.gameObject.SetActive(false);
            return;
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
            _audio.Play();
        } 
        else
        {
            _audio.Play();
            if (!G.Player.Hand.IsEmpty)
            {
                ItemTaked item = _currentItem;
                _currentItem = G.Player.Hand.CurrentItem;
                _currentItem.PutDown(_pointSocket);
                item.Take();
                
                return;
            }
            _currentItem.Take(); 
            _currentItem = null;
        }
    }


}
