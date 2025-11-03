using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewRecipeNotification : MonoBehaviour
{
    [Header("UI Elements")]
    public Image _background;
    public Image _icon;
    public TextMeshProUGUI _name;
    public AudioClip unlockSound;

    [Header("Animation Settings")]
    public float appearDuration = 0.5f;
    public float displayDuration = 2.5f;
    public float disappearDuration = 0.4f;
    public float pulseIntensity = 1.3f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector2 targetPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Начальная позиция — ниже экрана
        startPos = new Vector2(0, -rectTransform.rect.height - 50);
        targetPos = Vector2.zero;

        rectTransform.anchoredPosition = startPos;
        _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0f);
        _icon.transform.localScale = Vector3.zero;
    }

    public void Init(Recipe recipe)
    {
        _icon.sprite = recipe.Icon;
        _name.text = recipe.Name;
        StartCoroutine(ShowNotification());
    }

    public IEnumerator ShowNotification()
    {
        _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0f);

        // Сброс состояния
        rectTransform.anchoredPosition = startPos;
        _icon.transform.localScale = Vector3.zero;
        _background.transform.localScale = Vector3.one;

        // === ФАЗА 1: Появление ===
        float timer = 0f;
        while (timer <= appearDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / appearDuration;

            // 1. Выезд снизу
            float slideT = slideCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, slideT);

            //// 2. Прозрачность фона
            //float alpha = Mathf.Lerp(0f, 1f, t);
            //_background.color = new Color(_background.color.r, _background.color.g, _background.color.b, alpha);

            // 3. Масштаб иконки: pop-in с overshoot
            float scaleProgress = scaleCurve.Evaluate(t); // 0 → 1
            //float overshootScale = Mathf.LerpUnclamped(0f, 1f, scaleProgress); // до 1.25x
            //float bounceBack = 1f + (overshootScale - 1f) * 0.3f; // смягчённый возврат
            _icon.transform.localScale = Vector3.one * scaleProgress;

            yield return null;
        }

        // Финальная фиксация
        rectTransform.anchoredPosition = targetPos;
        _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 1f);
        _icon.transform.localScale = Vector3.one;


        StartCoroutine(PulseBackground(_background.color, displayDuration));

        // === ФАЗА 3: Ожидание ===
        yield return new WaitForSecondsRealtime(displayDuration);

        // === ФАЗА 4: Исчезновение ===
        timer = 0f;
        while (timer <= disappearDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / disappearDuration;

            // Уезд вниз
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(targetPos, startPos, t);

            // Прозрачность
            float alpha = Mathf.Lerp(1f, 0f, t);
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, alpha);

            // Лёгкое уменьшение иконки при исчезновении (опционально)
            _icon.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        // Скрытие
        gameObject.SetActive(false);
    }

    private IEnumerator PulseBackground(Color baseColor, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float pulse = 1f + Mathf.Sin(timer * 2f) * 0.15f * pulseIntensity;
            _background.transform.localScale = Vector3.one * pulse;
            _icon.transform.localScale = Vector3.one * pulse;
            yield return null;
        }
        _background.transform.localScale = Vector3.one;
        _icon.transform.localScale = Vector3.one;
    }

}
