using UnityEngine;

public class HandHolder : MonoBehaviour
{
    [Header("Hand / Socket")]
    [SerializeField] private Transform _handSocket; // Точка, куда прилетает предмет


    [Header("Drop settings")]
    [SerializeField] private float _dropForwardOffset = 0.6f; // куда положить/уронить вперёд
    [SerializeField] private float _dropUpOffset = 0.2f; // немного вверх, чтобы не проваливался в пол


    public ItemTaked CurrentItem { get; private set; }
    public bool IsEmpty => CurrentItem == null;


    public Transform HandSocket => _handSocket;


    public void SetCurrentItem(ItemTaked item)
    {
        CurrentItem = item;
    }


    public Vector3 GetDropPosition()
    {
        Vector3 pos = _handSocket.position + _handSocket.forward * _dropForwardOffset + Vector3.up * _dropUpOffset;
        return pos;
    }
}