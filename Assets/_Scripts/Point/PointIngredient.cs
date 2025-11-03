using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PointIngredient : MonoBehaviour
{
    [Header("Socket / Target point")]
    [SerializeField] private Transform _pointSocket;          
    [SerializeField] private InteractionPanel _interactionPanelGet;

    [Header("Endless Item")]
    [SerializeField] private ItemTaked _ingredientItem;

    private Outline _outline;
    public Transform Point => _pointSocket;
    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }
    private void Start()
    {
        _PlayerView(false);
    }
    public void _PlayerView(bool isView)
    {
        if (!G.Player.Hand.IsEmpty)
        {
            _outline.enabled = false;
            _interactionPanelGet.gameObject.SetActive(false);
            return;
        }
        _outline.enabled = isView;
        _interactionPanelGet.gameObject.SetActive(isView);
    }
    public void _Interact()
    {
            ItemTaked item = Instantiate(_ingredientItem, _pointSocket);
            item?.UseOutline(false);
            item.Take();
    }
    public ItemTaked GetIngredient()
    {
        return _ingredientItem;
    }


}
