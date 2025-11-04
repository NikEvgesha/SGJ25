using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 4.5f;          // базовая скорость ходьбы
    [SerializeField] private float _sprintMultiplier = 1.5f;   // множитель спринта (Shift)
    [SerializeField] private float _jumpHeight = 1.2f;         // высота прыжка в метрах
    [SerializeField] private float _gravity = -9.81f;          // сила гравитации (отрицательная)
    [SerializeField] private float _groundedStick = -2.0f;     // небольшая притяжка к земле

    [Header("Look")]
    [SerializeField] private Transform _cameraTransform;       // ссылка на камеру (дочерний объект)
    [SerializeField] private float _mouseSensitivity = 1.5f;   // чувствительность мыши
    [SerializeField] private float _minPitch = -80f;           // ограничение наклона вниз
    [SerializeField] private float _maxPitch = 80f;            // ограничение наклона вверх

    [Header("Misc")]
    [SerializeField] private bool _lockCursor = true;          // захватывать курсор во время игры

    // --- Runtime-поля (не в инспекторе) ---
    private CharacterController _controller;
    private Vector3 _velocity; // вертикальная скорость/гравитация
    private float _pitch;      // локальный наклон камеры по оси X
    private bool _teleport;
    private Transform _teleportPoint;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (_lockCursor)
        {
            _lockCursor = false;
            G.Control.CursorActive = _lockCursor;
        }
    }
    private void Update()
    {
        if (G.Input.Inventory)
        {
            _lockCursor = !_lockCursor;
            G.Control.CursorActive = _lockCursor;
        }
        if (!G.Control.CursorActive)
            Look();

        Move();
    }

    private void Look()
    {
        // Старая система ввода: оси Mouse X / Mouse Y
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        // Поворот корпуса (yaw) — вокруг Y
        transform.Rotate(Vector3.up, mouseX);

        // Наклон только камеры (pitch)
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        if (_cameraTransform != null)
        {
            Vector3 euler = _cameraTransform.localEulerAngles;
            euler.x = _pitch;
            euler.y = 0f;
            euler.z = 0f;
            _cameraTransform.localEulerAngles = euler;
        }
    }

    private void Move()
    {

        if (_teleport)
        {
            _controller.enabled = false;
            transform.position = _teleportPoint.position;
            _controller.enabled = true;
            _teleport = false;
        }

        bool grounded = _controller.isGrounded;

        // Старая система ввода: Horizontal (A/D), Vertical (W/S)
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * inputX + transform.forward * inputZ);
        float speed = _moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? _sprintMultiplier : 1f);

        // Горизонтальная проекция движения
        _controller.Move(move * speed * Time.deltaTime);

        // Притягиваем к земле, чтобы не "зависать"
        if (grounded && _velocity.y < 0f)
        {
            _velocity.y = _groundedStick;
        }

        // Прыжок по кнопке Jump (Space по умолчанию)
        if (grounded && Input.GetButtonDown("Jump"))
        {
            // Формула v = sqrt(h * -2g)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        // Гравитация
        _velocity.y += _gravity * Time.deltaTime;

        // Вертикальное перемещение (гравитация/прыжок)
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (_lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Teleport(Transform point)
    {
        _teleportPoint = point;
        _teleport = true;
    }
}
