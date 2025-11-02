using System;
using UnityEngine;

[Serializable]
public struct IngredientData
{
    public string Name;
    public int BuyPrice;
    //public int InsightPrice;
    public Sprite Icon;
    public string hint;
    public ItemType Type;


    //public static bool operator ==(IngredientData c1, IngredientData c2)
    //{
    //    return c1.Name.Equals(c2.Name);
    //}

    //public static bool operator !=(IngredientData c1, IngredientData c2)
    //{
    //    return !c1.Name.Equals(c2.Name);
    //}

}

public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientData _data;



    public IngredientData Data => _data;
}
