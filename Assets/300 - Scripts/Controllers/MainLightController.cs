using UnityEngine;
using DG.Tweening;

public class MainLightController : MonoBehaviour
{
    public static MainLightController I;

    [SerializeField] private Light _light;

    [Header("Default")]
    [SerializeField] private Color _defaultColor = new Color(0.9f, 0.95f, 1f);
    [SerializeField] private float _defaultIntensity = 1.2f;

    [Header("Alert")]
    [SerializeField] private Color _alertColor = new Color(1f, 0.1f, 0.05f);
    [SerializeField] private float _alertIntensity = 1.8f;

    [Header("Transition")]
    [SerializeField] private float _transitionDuration = 0.5f;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        QualitySettings.shadowDistance = 100f;
    }

    // ── API ────────────────────────────────────────────────────────

    public void SetAlert()
    {
        TransitionTo(_alertColor, _alertIntensity);
        DOVirtual.DelayedCall(_transitionDuration, () => {
        DOTween.Kill(_light);
        DOTween.To(() => _light.intensity, x => _light.intensity = x, _alertIntensity * 0.7f, 0.8f)
            .SetTarget(_light)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    });
    }

    public void SetDefault()
    {
        DOTween.Kill(_light); // stoppe le flicker perma
        TransitionTo(_defaultColor, _defaultIntensity);
    }

    public void Flicker(Color color, float intensity, int count = 3, float speed = 0.1f, System.Action onComplete = null)
    {
        DOTween.Kill(_light);

        var seq = DOTween.Sequence().SetTarget(_light);

        for (int i = 0; i < count; i++)
        {
            seq.Append(_light.DOColor(color, speed).SetEase(Ease.InOutSine));
            seq.Join(_light.DOIntensity(intensity, speed).SetEase(Ease.InOutSine));
            seq.Append(_light.DOColor(_defaultColor, speed).SetEase(Ease.InOutSine));
            seq.Join(_light.DOIntensity(_defaultIntensity, speed).SetEase(Ease.InOutSine));
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }

    // ── Internal ───────────────────────────────────────────────────

    private void TransitionTo(Color color, float intensity)
    {
        DOTween.Kill(_light);
        DOTween.To(() => _light.intensity, x => _light.intensity = x, intensity, _transitionDuration).SetTarget(_light);
        DOTween.To(() => _light.color, x => _light.color = x, color, _transitionDuration).SetTarget(_light);
    }

    void OnDestroy() => DOTween.Kill(_light);
}