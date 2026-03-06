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

    [SerializeField] Transform pivot;

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

        // Rotation 3 tours
        pivot.DORotate(new Vector3(0, 360 * 2, 0), 1.2f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic);
    }


    public void TurnOff()
    {
        _pulseTween?.Kill();
        
        float current = GetCurrentIntensity();
        
        // Quelques tressautements avant de tomber
        DOTween.Sequence()
            .Append(transform.DORotate(new Vector3(0, 0, 15f), 0.08f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(new Vector3(0, 0, -10f), 0.08f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(new Vector3(0, 0, 8f), 0.06f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(new Vector3(0, 0, -5f), 0.06f).SetEase(Ease.OutQuad))
            // Tombe sur le côté
            .Append(transform.DORotate(new Vector3(0, 0, 90f), 0.3f).SetEase(Ease.InCubic))
            // Petit bounce au sol
            .Append(transform.DORotate(new Vector3(0, 0, 80f), 0.1f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(new Vector3(0, 0, 90f), 0.1f).SetEase(Ease.InQuad))
            // Fade emission en parallèle
            .Insert(0f, DOVirtual.Float(current, 0f, 0.8f, val => SetEmission(_idleColor, val)));
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