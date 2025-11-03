using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable/Recipe", fileName ="Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private List<Ingredient> _ingredients;
    [SerializeField] private int _insightReward;
    [SerializeField] private int _unlockInsightPrice;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Ingredient _potionIngredient;

    public string Name => _name;
    public string Description => _description;
    public List<Ingredient> Ingredients => _ingredients;
    public Sprite Icon => _icon;
    public int UnlockIngredientPrice => _unlockInsightPrice;
    public Ingredient ResultIngredient => _potionIngredient;
    public int InsightReward => _insightReward;
}
