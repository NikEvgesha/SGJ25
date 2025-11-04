using UnityEngine;
using UnityEngine.UI;

public class PotSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public Sprite Icon { set
        {
            _icon.sprite = value;
        } }
}
