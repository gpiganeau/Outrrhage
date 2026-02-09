using UnityEngine;

public class SkillVFXController : MonoBehaviour
{
    private SkillData skillData;
    private GameObject currentCastVFX;
    private GameObject currentAreaVFX;
    
    public void Initialize(SkillData data)
    {
        skillData = data;
    }
    
    // === CASTING VFX ===
    public GameObject PlayCastVFX(Transform caster)
    {
        if (skillData.vfxData.castVFXPrefab == null) return null;
        
        Vector3 spawnPos = caster.position + skillData.vfxData.castVFXOffset;
        currentCastVFX = Instantiate(skillData.vfxData.castVFXPrefab, spawnPos, Quaternion.identity);
        
        if (skillData.vfxData.attachCastVFXToCharacter)
        {
            currentCastVFX.transform.SetParent(caster);
        }
        
        Destroy(currentCastVFX, skillData.vfxData.castVFXDuration);
        
        // Sound
        if (skillData.vfxData.castSound != null)
        {
            AudioSource.PlayClipAtPoint(skillData.vfxData.castSound, spawnPos);
        }

        return currentCastVFX;
    }
    
    // === AREA INDICATOR VFX ===
    public GameObject ShowAreaIndicator(Vector3 position, Vector3 direction)
    {
        if (skillData.vfxData.areaIndicatorVFXPrefab == null) return null;
        
        currentAreaVFX = Instantiate(skillData.vfxData.areaIndicatorVFXPrefab, position, 
                                      Quaternion.LookRotation(direction));
        
        if (skillData.vfxData.scaleAreaVFXWithRadius)
        {
            float scale = skillData.Radius * 2f; // Diamètre
            currentAreaVFX.transform.localScale = new Vector3(scale, 1f, scale);
        }
        
        // Coloration
        var renderer = currentAreaVFX.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial.color = skillData.vfxData.areaVFXColor;
        }

        return currentAreaVFX;
    }
    
    public void HideAreaIndicator()
    {
        if (currentAreaVFX != null)
        {
            Destroy(currentAreaVFX);
        }
    }
    
    // === PROJECTILE VFX ===
    public void AttachProjectileVFX(GameObject projectile)
    {
        if (skillData.vfxData.projectileTrailVFXPrefab == null) return;
        
        GameObject trailVFX = Instantiate(skillData.vfxData.projectileTrailVFXPrefab, 
                                          projectile.transform);
        
        if (skillData.vfxData.scaleProjectileVFXWithRadius)
        {
            trailVFX.transform.localScale = Vector3.one * skillData.Radius;
        }
        
        // Sound
        if (skillData.vfxData.projectileSound != null)
        {
            AudioSource source = projectile.GetComponent<AudioSource>();
            if (source == null) source = projectile.AddComponent<AudioSource>();
            source.clip = skillData.vfxData.projectileSound;
            source.loop = true;
            source.Play();
        }
    }
    
    // === IMPACT VFX ===
    public GameObject PlayImpactVFX(Vector3 position, Vector3 normal)
    {
        if (skillData.vfxData.projectileImpactVFXPrefab == null) return null;
        
        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject impactVFX = Instantiate(skillData.vfxData.projectileImpactVFXPrefab, 
                                           position, rotation);
        
        Destroy(impactVFX, 3f);
        
        // Sound
        if (skillData.vfxData.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(skillData.vfxData.impactSound, position);
        }

        return impactVFX;
    }
    
    // === HIT VFX (sur l'ennemi) ===
    public void PlayHitVFX(Vector3 position, int damage)
    {
        if (skillData.vfxData.hitVFXPrefab == null) return;
        
        GameObject hitVFX = Instantiate(skillData.vfxData.hitVFXPrefab, position, 
                                        Quaternion.identity);
        
        if (skillData.vfxData.scaleHitVFXWithDamage)
        {
            float scale = 1f + (damage / 100f); // Exemple de scaling
            hitVFX.transform.localScale = Vector3.one * scale;
        }
        
        Destroy(hitVFX, skillData.vfxData.hitVFXLifetime);
    }
}