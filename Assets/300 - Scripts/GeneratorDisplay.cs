using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GeneratorDisplay : MonoBehaviour
{
    [SerializeField] private List<Renderer> _renderers;
    [SerializeField] private Color _pulseColor = new Color(1f, 0.3f, 0f);
    [SerializeField] private Color _idleColor = new Color(0.2f, 0.8f, 1f);
    [SerializeField] private float _pulseIntensity = 3f;
    [SerializeField] private float _idleIntensity = 0.5f;

    private List<Material> _mats = new();
    private Tween _pulseTween;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        foreach (var r in _renderers)
        {
            var mat = r.material;
            mat.EnableKeyword("_EMISSION");
            _mats.Add(mat);
        }
        SetEmission(_idleColor, _idleIntensity);
    }

    public void Pulse()
    {
        _pulseTween?.Kill();
        
        SetEmission(_pulseColor, _pulseIntensity);
        
        _pulseTween = DOVirtual.Float(_pulseIntensity, _idleIntensity * 0.5f, 0.3f, val =>
            SetEmission(_pulseColor, val))
            .SetLoops(6, LoopType.Yoyo) // 3 aller-retour = 6 loops
            .SetEase(Ease.InOutSine)
            .OnComplete(() => SetEmission(_pulseColor, _idleIntensity)); // ← reste en orange idle
    }

    public void TurnOff()
    {
        _pulseTween?.Kill();
        float current = GetCurrentIntensity();
        DOVirtual.Float(current, 0f, 0.5f, val =>
            SetEmission(_idleColor, val));
    }

    private void SetEmission(Color color, float intensity)
    {
        foreach (var mat in _mats)
            mat.SetColor(EmissionColor, color * intensity);
    }

    private float GetCurrentIntensity()
    {
        if (_mats.Count == 0) return 0f;
        return _mats[0].GetColor(EmissionColor).maxColorComponent;
    }

    void OnDestroy()
    {
        _pulseTween?.Kill();
        foreach (var mat in _mats) Destroy(mat);
    }
}