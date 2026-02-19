using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResourceGauge : MonoBehaviour
{
    public enum ResourceType { Health, Blood, Rage }
    [SerializeField] ResourceType _resourceType;
    
    [Header("References")]
    [SerializeField] Image _fillImage;
    [SerializeField] TMP_Text _resourceText;
    
    [Header("Animation Settings")]
    [SerializeField] float _tweenDuration = 0.3f;
    [SerializeField] Ease _tweenEase = Ease.OutCubic;
    
    private Tween _currentTween;
    
    public void UpdateGauge(float current, float max)
    {
        if (_fillImage != null) 
        {
            _currentTween?.Kill();
            
            float targetFillAmount = current / max;
            _currentTween = _fillImage.DOFillAmount(targetFillAmount, _tweenDuration)
                .SetEase(_tweenEase)
                .OnComplete(() => {
                    _fillImage.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 2, 0.25f);
            });
        }
        
        if (_resourceText != null) 
        {
            _resourceText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }
    }
    
    void OnDestroy()
    {
        _currentTween?.Kill();
    }
}