using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ItemTaked : MonoBehaviour
{
    [Header("Pick / Place")]
    [SerializeField] private float _pickDuration = 0.25f; // время "прилёта" в руку
    [SerializeField] private AnimationCurve _pickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool _alignRotation = true; // выравнивать ли поворот
    [SerializeField] private Vector3 _localOffset = Vector3.zero; // локальный оффсет в руке
    [SerializeField] private Vector3 _localEuler = Vector3.zero; // локальный поворот в руке

    // --- Runtime ---
    private Coroutine _moveRoutine;
    private Item _item;
    public Item Item => _item;
    private Outline _outline;

    public bool IsTaken { get; private set; }

    private void Awake()
    {
        _item = GetComponent<Item>();
        _outline = GetComponent<Outline>();
    }
    /// <summary>
    /// Точка входа: дерни это из системы интеракции.
    /// </summary>
    public void UseOutline(bool use)       
    {
        _outline.enabled = use;
    }
    
    public bool TryPut()
    {
        return _item.Type == ItemType.Potion;
    }

    public void Take()
    {
        //this.gameObject.layer = 0;

        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }
        UseOutline(false);
        G.Player.Hand.SetCurrentItem(this);
        IsTaken = true;

        StartMoveToParent(G.Player.Hand.HandSocket);
    }


    public void PutDown(Transform point,bool del = false,bool hide = false)
    {
        //this.gameObject.layer = 6;

        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }

        G.Player.Hand.SetCurrentItem(null);
        IsTaken = false;

        StartMoveToParent(point, del, hide);
    }
    public void MoveToParent(Transform point, bool del = false, bool hide = false)
    {
        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }
        StartMoveToParent(point, del, hide);
    }

    private void StartMoveToParent(Transform parent,bool del = false, bool hide = false)
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);


        _moveRoutine = StartCoroutine(MoveToParentRoutine(parent, del, hide));
    }
    private IEnumerator MoveToParentRoutine(Transform parent, bool del = false, bool hide = false)
    {
        // Простой вариант: сразу привязываем к parent, но сохраняем мировую позу (чтобы не было скачка),
        // затем плавно ведём ЛОКАЛЬНЫЕ position/rotation к целевым.
        transform.SetParent(parent, worldPositionStays: true);


        Vector3 startLocalPos = transform.localPosition;
        Quaternion startLocalRot = transform.localRotation;
        Vector3 endLocalPos = _localOffset;
        Quaternion endLocalRot = _alignRotation ? Quaternion.Euler(_localEuler) : transform.localRotation;


        float t = 0f;
        while (t < _pickDuration)
        {
            t += Time.deltaTime;
            float k = _pickDuration > 0f ? Mathf.Clamp01(t / _pickDuration) : 1f;
            float eased = _pickCurve.Evaluate(k);


            transform.localPosition = Vector3.LerpUnclamped(startLocalPos, endLocalPos, eased);
            transform.localRotation = Quaternion.SlerpUnclamped(startLocalRot, endLocalRot, eased);
            yield return null;
        }


        // Финал: фиксируем ровно целевые локальные значения
        transform.localPosition = endLocalPos;
        transform.localRotation = endLocalRot;


        _moveRoutine = null;

        if (del) Destroy(gameObject);
        if (hide) gameObject.SetActive(false);
    }
}
