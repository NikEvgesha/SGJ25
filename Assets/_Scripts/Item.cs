using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _itemType;
    private Ingredient _ingredient;
    public ItemType Type => _itemType;
    public IngredientData Data => _ingredient.Data;

    private void Awake()
    {
        _ingredient.GetComponent<Ingredient>();
    }


}
