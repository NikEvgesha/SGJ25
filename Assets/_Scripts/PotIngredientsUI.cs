using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotIngredientsUI : MonoBehaviour
{
    private List<PotSlot> _slots;

    private void Start()
    {
        if (G.Game.Cauldron != null)
        {
            G.Game.Cauldron.IngredientsUpdated.AddListener(OnIngredientsUpdated);
        } else
        {
            G.Game.CauldronReady.AddListener(() => G.Game.Cauldron.IngredientsUpdated.AddListener(OnIngredientsUpdated));
        }
            
        _slots = GetComponentsInChildren<PotSlot>().ToList();
        foreach (PotSlot slot in _slots) {
            slot.gameObject.SetActive(false);
        }
    }

    private void OnIngredientsUpdated(List<Ingredient> ingredients)
    {
        StopAllCoroutines();
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < ingredients.Count)
            {
                _slots[i].gameObject.SetActive(true);
                _slots[i].Icon = ingredients[i].Data.Icon;
            }
            else
                _slots[i].gameObject.SetActive(false);
        }

        if (ingredients.Count == 3)
        {
            StartCoroutine(Disappear());
        }

    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (PotSlot slot in _slots)
        {
            slot.gameObject.SetActive(false);
        }
    }

}
