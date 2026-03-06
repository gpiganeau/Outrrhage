using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections.Generic;

public class Juicer : MonoBehaviour
{
    #region Singleton & References
    public static Juicer I;
    private VisualSettings settings;
    
    [Header("Post Process")]
    public Volume globalVolume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    
    [Header("Camera Shake")]
    public Camera mainCamera;
    private Vector3 originalCameraPosition;
    
    [Header("Time Control")]
    private float originalTimeScale = 1f;
    
    #endregion
    
    #region Unity Callbacks
    
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
            globalVolume.profile.TryGet(out lensDistortion);
        }
    }

    void Start()
    {
        settings = SettingsManager.Instance.VisualSettings;
    }

    private void OnDestroy()
    {
        // Reset time scale au cas où
        Time.timeScale = 1f;
        DOTween.Kill("TimeControl");
    }

    #endregion

    #region Private, Internal Juice Methods (Post Process, Screen Shake, Time Control, Entity Effects)
    private bool IsJuiceEnabled() => settings.EnableJuicer;
    private HashSet<Renderer> _affectedRenderers = new ();

    #region POST PROCESS

    /// <summary>Pulse la vignette (ex: quand le joueur prend des dégâts)</summary>
    private void PulseVignette(float intensity = 0.5f, float duration = 0.3f)
    {
        if (vignette == null) return;
        
        float originalIntensity = vignette.intensity.value;
        DOTween.Sequence()
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, intensity, duration * 0.5f))
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, originalIntensity, duration * 0.5f));
    }
    
    /// <summary>Set l'intensité de la vignette</summary>
    private void SetVignetteIntensity(float intensity, float duration = 0f)
    {
        if (vignette == null) return;
        
        if (duration > 0)
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, intensity, duration);
        else
            vignette.intensity.value = intensity;
    }
    
    /// <summary>Pulse l'aberration chromatique (impact visuel)</summary>
    private void PulseChromaticAberration(float intensity = 0.8f, float duration = 0.2f)
    {
        if (chromaticAberration == null) return;
        
        float originalIntensity = chromaticAberration.intensity.value;
        DOTween.Sequence()
            .Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, intensity, duration * 0.5f))
            .Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, originalIntensity, duration * 0.5f));
    }
    
    /// <summary>Flash de couleur (mort, hit, etc.)</summary>
    private void ColorFlashPostProcess(Color color, float duration = 0.2f)
    {
        if (colorAdjustments == null) return;

        Color BeforeColor =  CharacterComponent.InRage  && settings.RageUseColorAdjustement ? settings.PlayerRageColor : Color.white;
        
        colorAdjustments.colorFilter.value = color;
        DOTween.To(() => colorAdjustments.colorFilter.value, 
            x => colorAdjustments.colorFilter.value = x, 
            BeforeColor, 
            duration);
    }
    
    #endregion
    
    #region SCREEN SHAKE
    
    /// <summary>Simple Shake</summary>
    private void ShakeCamera(float intensity = 0.3f, float duration = 0.3f)
    {
        if (mainCamera == null) return;
        
        mainCamera.transform.DOKill();
        originalCameraPosition = mainCamera.transform.localPosition;
        mainCamera.transform.DOShakePosition(duration, intensity, 20, 90, false, true)
            .OnComplete(() => mainCamera.transform.localPosition = originalCameraPosition);
    }
    
    /// <summary>Shake avec rotation, marche bien pour la death</summary>
    private void ShakeCameraWithRotation(float positionIntensity = 0.3f, float rotationIntensity = 2f, float duration = 0.3f)
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
    private void FreezeFrame(float duration = 0.1f)
    {
        DOTween.Kill("TimeControl");
        Time.timeScale = 0f;
        DOVirtual.DelayedCall(duration, () => Time.timeScale = originalTimeScale, false)
            .SetUpdate(true) // Important: utilise unscaled time
            .SetId("TimeControl")
            .onKill += () => Time.timeScale = originalTimeScale; // Assure que le time scale est reset même si le tween est tué prématurément
    }
    
    /// <summary>Freeze frame sur un actor spécifique (via Animator), à tester</summary>
    private void FreezeActor(Animator animator, float duration = 0.1f)
    {
        if (animator == null) return;
        
        animator.speed = 0f;
        DOVirtual.DelayedCall(duration, () => animator.speed = 1f, false)
            .SetUpdate(true);
    }
    
    #endregion
    
    #region TIME CONTROL
    
    /// <summary>Slow motion temporaire</summary>
    private void SlowMotion(float slowFactor = 0.3f, float duration = 1f)
    {
        DOTween.Kill("TimeControl");
        
        DOTween.Sequence()
            .AppendCallback(() => SetTimeScale(slowFactor))
            .AppendInterval(duration)
            .AppendCallback(() => ResetTimeScale())
            .SetId("TimeControl");
    }
    
    /// <summary>Set directement le time scale</summary>
    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
    
    /// <summary>Reset le time scale à la normale</summary>
    private void ResetTimeScale()
    {
        SetTimeScale(1.0F);
    }
    
    #endregion
    
    #region ENTITY EFFECTS

    /// <summary>Squash & Stretch sur une entité (bounce, impact, jump)</summary>
    private void SquashAndStretch(Transform entity, Vector3 squashScale, float duration = 0.2f, Ease ease = Ease.OutBack)
    {
        if (entity == null) return;
        
        Vector3 originalScale = entity.localScale;
        
        entity.DOKill(); // Kill les tweens précédents sur cette entité
        
        DOTween.Sequence()
            .Append(entity.DOScale(squashScale, duration * 0.5f).SetEase(ease))
            .Append(entity.DOScale(originalScale, duration * 0.5f).SetEase(ease));
    }

    /// <summary>Squash & Stretch preset - Impact au sol</summary>
    private void SquashHorizontal(Transform entity, float intensity = 0.3f, float duration = 0.2f)
    {
        Vector3 squash = new Vector3(1f + intensity, 1f - intensity, 1f + intensity);
        SquashAndStretch(entity, squash, duration, Ease.OutElastic);
    }

    /// <summary>Squash & Stretch preset - Jump/Anticipation</summary>
    private void SquashVertical(Transform entity, float intensity = 0.2f, float duration = 0.15f)
    {
        Vector3 squash = new Vector3(1f - intensity, 1f + intensity, 1f - intensity);
        SquashAndStretch(entity, squash, duration, Ease.InOutQuad);
    }

    /// <summary>Color Flash sur une entité</summary>
    private void EntityColorFlash(List<Renderer> renderers, Color flashColor, float duration = 0.2f)
    {
        if (renderers == null || renderers.Count == 0) return;

        foreach (var renderer in renderers)
        {
            // -- Check for hashset to prevent multiple flashes on the same renderer at the same time --
            if (_affectedRenderers.Contains(renderer)) continue; 
            _affectedRenderers.Add(renderer);
            
            Color originalColor = renderer.sharedMaterial.color;
            renderer.material.DOKill();
            
            DOTween.Sequence()
                .Append(renderer.material.DOColor(flashColor, duration * 0.3f))
                .Append(renderer.material.DOColor(originalColor, duration * 0.7f))
                .OnComplete(() => _affectedRenderers.Remove(renderer));
        }
    }

    /// <summary>Color Flash preset - Dégâts (rouge)</summary>
    private void EntityDamageFlash(List<Renderer> renderers, float duration = 0.15f)
    {
        EntityColorFlash(renderers, settings.PlayerDamagedColor, duration);
    }

    /// <summary>Color Flash preset - Heal (vert)</summary>
    private void EntityHealFlash(List<Renderer> renderers, float duration = 0.2f)
    {
        EntityColorFlash(renderers, settings.PlayerHealColor, duration);
    }
    #endregion
    #endregion
    
    #region Public API
    
    #region Damage & Death

    public void StartRage(float duration)
    {
        if (!IsJuiceEnabled()) return;
        
        DOTween.Kill("RageControl");
        
        DOTween.Sequence().SetId("RageControl")
            .AppendCallback(() => SetTimeScale(1.2f))
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, settings.RageVignetteIntensity, duration * 0.1f))
            .Join(DOTween.To(() => colorAdjustments.colorFilter.value, x => colorAdjustments.colorFilter.value = x, settings.RageUseColorAdjustement ? settings.PlayerRageColor : Color.white, duration * 0.1f))
            .Join(DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, settings.RageLensDistortionIntensity, duration * 0.1f))
            .Join(DOTween.To(() => chromaticAberration.intensity.value, x =>  chromaticAberration.intensity.value = x, settings.RageChromaIntensity, duration * 0.3f))
            .SetUpdate(false);
    }

    public void StopRage(float duration)
    {
        if (!IsJuiceEnabled()) return;
        
        DOTween.Kill("RageControl");
        
        DOTween.Sequence().SetId("RageControl")
            .AppendCallback(() => ResetTimeScale())
            .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, duration))
            .Join(DOTween.To(() => colorAdjustments.colorFilter.value, x => colorAdjustments.colorFilter.value = x, Color.white, duration))
            .Join(DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, 0f, duration))
            .Join(DOTween.To(() => chromaticAberration.intensity.value, x =>  chromaticAberration.intensity.value = x, 0f, duration))
            .AppendCallback(() => PulseChromaticAberration(1f, 0.4f));
    }
    /// <summary>Effet générique de hit sur les AI Actor Components</summary>
    public void EnemyDamagedImpact(float intensity = 0.3f, List<Renderer> renderers = null)
    {

        if (!IsJuiceEnabled()) return;
        
        ShakeCamera(intensity, 0.15f);
        PulseChromaticAberration(0.5f, 0.2f);
        FreezeFrame(0.05f);

        if (renderers.Count > 0) {
            EntityDamageFlash(renderers, intensity);
            //SquashAndStretch(renderer.transform, new Vector3(0.75f, 1.5f, 0.75f), intensity);

            foreach (var renderer in renderers)
            {
                SquashVertical(renderer.transform, 0.5f, 0.2f);
            }
        }
    }

    /// <summary>Effet de dégâts sur le joueur</summary>
    public void PlayerDamagedImpact(List<Renderer> playerRenderers)
    {
        if (!IsJuiceEnabled()) return;

        PulseVignette(0.5f, 0.3f);
        ShakeCamera(0.2f, 0.2f);
        ColorFlashPostProcess(new Color(1f, 0.3f, 0.3f), 0.15f);
        EntityDamageFlash(playerRenderers);
    }

    public void PlayerHealedEffect(List<Renderer> playerRenderers)
    {
        if (!IsJuiceEnabled()) return;

        ColorFlashPostProcess(settings.PlayerHealColor, 0.25f);
        EntityHealFlash(playerRenderers);
    }

    public void EnnemyHealedEffect(List<Renderer> renderers)
    {
        if (!IsJuiceEnabled()) return;

        EntityHealFlash(renderers);
    }
    
    /// <summary>Effet de mort sur Riel</summary>
    public void PlayerDeathEffect()
    {
        if (!IsJuiceEnabled()) return;

        ShakeCameraWithRotation(0.5f, 3f, 0.5f);
        ColorFlashPostProcess(settings.PlayerDeathColor, 2f);
        SlowMotion(0.2f, 0.8f);
        PulseVignette(0.6f, 2f);
    }

    public void EnemyDeathEffect()
    {
        if (!IsJuiceEnabled()) return;

        SlowMotion(0.5f, 0.1f);
    }

    public void LastEnemyEffect()
    {
        if (!IsJuiceEnabled()) return;
        SlowMotion(0.5f, .5f);
    }

    public void RoomEndEffect()
    {
        if (!IsJuiceEnabled()) return;
        PulseChromaticAberration(0.8f, 0.5f);
        PulseChromaticAberration(1.2f, 0.2f);
        PulseChromaticAberration(0.8f, 1f);
    }
    
    #endregion
    #endregion
}