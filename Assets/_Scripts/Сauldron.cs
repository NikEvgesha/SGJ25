using System.Collections.Generic;
using UnityEngine;

public class Сauldron : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;            
    [SerializeField] private InteractionPanel _interactionPanel;
    [SerializeField] private GameObject _water;


    private List<ItemTaked> _listItemsInСauldron = new();
    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _water.SetActive(false);
    }
    private void Start()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {

        if (G.Player.Hand.IsEmpty)
        {
            _outline.enabled = false;
            _interactionPanel.gameObject.SetActive(false);
            return;
        } 
        _outline.enabled = isView;
        _interactionPanel.gameObject.SetActive(isView);
        
    }

    public void Interact()
    {
        G.Player.Hand.CurrentItem.PutDown(_pointSocket,true);
        AddItem(G.Player.Hand.CurrentItem); 
    }
    private void AddItem(ItemTaked item)
    {

        _water.SetActive(true);
        _listItemsInСauldron.Add(item);
        if (_listItemsInСauldron.Count >= 3)
        {

            _water.SetActive(false);
            CheckRecept();
            _listItemsInСauldron.Clear();
        }
    }
    private void CheckRecept()
    {

    }
}
