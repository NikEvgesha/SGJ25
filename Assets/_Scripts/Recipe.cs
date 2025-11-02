using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnlockableIngredient
{
    public Ingredient Ingredient;
    public int InsightPrice;
}

[CreateAssetMenu(menuName ="Scriptable/Recipe", fileName ="Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private List<UnlockableIngredient> _ingredients;
    [SerializeField] private int _insightReward;
    [SerializeField] private int _unlockInsightPrice;
    [SerializeField] private int _sellPrice;
    [SerializeField] private Sprite _icon;

    public string Name => _name;
    public string Description => _description;
    public List<UnlockableIngredient> Ingredients => _ingredients;
    public int SellPrice => _sellPrice;
    public Sprite Icon => _icon;
}
