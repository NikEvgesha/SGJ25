using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _itemType;
    public ItemType Type => _itemType;
}
