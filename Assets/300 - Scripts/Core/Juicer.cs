using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Juicer : MonoBehaviour
{
    public static Juicer I;
    
    [Header("Post Process")]
    public Volume globalVolume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    
    [Header("Camera Shake")]
    public Camera mainCamera;
    private Vector3 originalCameraPosition;
    
    [Header("Time Control")]
    private float originalTimeScale = 1f;
    
    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(this.gameObject);
        
        // Setup references
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) originalCameraPosition = mainCamera.transform.localPosition;
        
        if (globalVolume == null) globalVolume = FindAnyObjectByType<Volume>();

        // Get Post Process effects
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out colorAdjustments);
        }
    }
    
    #region POST PROCESS
    
    /// <summary>Pulse la vignette (ex: quand le joueur prend des dégâts)</summary>
    public void PulseVignette(float intensity = 0.5f, float duration = 0.3f)
    {
        if (vignette == null) return;
        
        float originalIntensity = vignette.intensity.value;
        DOTween.Sequence()
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, intensity, duration * 0.5f))
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, originalIntensity, duration * 0.5f));
    }
    
    /// <summary>Set l'intensité de la vignette</summary>
    public void SetVignetteIntensity(float intensity, float duration = 0f)
    {
        if (vignette == null) return;
        
        if (duration > 0)
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, intensity, duration);
        else
            vignette.intensity.value = intensity;
    }
    
    /// <summary>Pulse l'aberration chromatique (impact visuel)</summary>
    public void PulseChromaticAberration(float intensity = 0.8f, float duration = 0.2f)
    {
        if (chromaticAberration == null) return;
        
        float originalIntensity = chromaticAberration.intensity.value;
        DOTween.Sequence()
            .Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, intensity, duration * 0.5f))
            .Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, originalIntensity, duration * 0.5f));
    }
    
    /// <summary>Flash de couleur (mort, hit, etc.)</summary>
    public void ColorFlash(Color color, float duration = 0.2f)
    {
        if (colorAdjustments == null) return;
        
        colorAdjustments.colorFilter.value = color;
        DOTween.To(() => colorAdjustments.colorFilter.value, 
            x => colorAdjustments.colorFilter.value = x, 
            Color.white, 
            duration);
    }
    
    #endregion
    
    #region SCREEN SHAKE
    
    /// <summary>Shake simple de la caméra</summary>
    public void ShakeCamera(float intensity = 0.3f, float duration = 0.3f)
    {
        if (mainCamera == null) return;
        
        mainCamera.transform.DOKill();
        originalCameraPosition = mainCamera.transform.localPosition;
        mainCamera.transform.DOShakePosition(duration, intensity, 20, 90, false, true)
            .OnComplete(() => mainCamera.transform.localPosition = originalCameraPosition);
    }
    
    /// <summary>Shake avec rotation pour plus d'impact</summary>
    public void ShakeCameraWithRotation(float positionIntensity = 0.3f, float rotationIntensity = 2f, float duration = 0.3f)
    {
        if (mainCamera == null) return;
        
        mainCamera.transform.DOKill();
        originalCameraPosition = mainCamera.transform.localPosition;
        
        Sequence shakeSequence = DOTween.Sequence();
        shakeSequence.Append(mainCamera.transform.DOShakePosition(duration, positionIntensity, 20, 90, false, true));
        shakeSequence.Join(mainCamera.transform.DOShakeRotation(duration, rotationIntensity, 20, 90, true));
        shakeSequence.OnComplete(() => 
        {
            mainCamera.transform.localPosition = originalCameraPosition;
            mainCamera.transform.localRotation = Quaternion.identity;
        });
    }
    
    #endregion
    
    #region FREEZE FRAME
    
    /// <summary>Freeze frame (arrêt total du temps)</summary>
    public void FreezeFrame(float duration = 0.1f)
    {
        DOTween.Kill("TimeControl");
        Time.timeScale = 0f;
        DOVirtual.DelayedCall(duration, () => Time.timeScale = originalTimeScale, false)
            .SetUpdate(true) // Important: utilise unscaled time
            .SetId("TimeControl");
    }
    
    /// <summary>Freeze frame sur un actor spécifique (via Animator)</summary>
    public void FreezeActor(Animator animator, float duration = 0.1f)
    {
        if (animator == null) return;
        
        animator.speed = 0f;
        DOVirtual.DelayedCall(duration, () => animator.speed = 1f, false)
            .SetUpdate(true);
    }
    
    #endregion
    
    #region TIME CONTROL
    
    /// <summary>Slow motion temporaire</summary>
    public void SlowMotion(float slowFactor = 0.3f, float duration = 1f)
    {
        DOTween.Kill("TimeControl");
        
        DOTween.Sequence()
            .Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, slowFactor, 0.1f).SetEase(Ease.OutQuad))
            .AppendInterval(duration)
            .Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, originalTimeScale, 0.3f).SetEase(Ease.InQuad))
            .SetUpdate(true)
            .SetId("TimeControl");
    }
    
    /// <summary>Set directement le time scale</summary>
    public void SetTimeScale(float scale, float transitionDuration = 0.2f)
    {
        DOTween.Kill("TimeControl");
        
        if (transitionDuration > 0)
        {
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, scale, transitionDuration)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true)
                .SetId("TimeControl");
        }
        else
        {
            Time.timeScale = scale;
        }
    }
    
    /// <summary>Reset le time scale à la normale</summary>
    public void ResetTimeScale(float transitionDuration = 0.2f)
    {
        SetTimeScale(originalTimeScale, transitionDuration);
    }
    
    #endregion
    
    #region COMBOS (Effets combinés populaires)
    
    /// <summary>Effet complet de hit/impact</summary>
    public void HitImpact(float intensity = 0.3f)
    {
        ShakeCamera(intensity, 0.15f);
        PulseChromaticAberration(0.5f, 0.2f);
        FreezeFrame(0.02f);
    }
    
    /// <summary>Effet de mort dramatique</summary>
    public void DeathEffect()
    {
        ShakeCameraWithRotation(0.5f, 3f, 0.5f);
        ColorFlash(Color.red, 1f);
        SlowMotion(0.2f, 0.8f);
        PulseVignette(0.6f, 0.5f);
    }
    
    /// <summary>Effet de dégâts sur le joueur</summary>
    public void PlayerDamaged()
    {
        PulseVignette(0.5f, 0.3f);
        ShakeCamera(0.2f, 0.2f);
        ColorFlash(new Color(1f, 0.3f, 0.3f), 0.15f);
    }
    
    #endregion
    
    private void OnDestroy()
    {
        // Reset time scale au cas où
        Time.timeScale = 1f;
        DOTween.Kill("TimeControl");
    }
}