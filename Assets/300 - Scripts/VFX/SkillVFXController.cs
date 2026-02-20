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

        Vector3 spawnPos = caster.position + skillData.vfxData.castVFXOffset;
        
        // Sound
        if (skillData.vfxData.castSound != null)
        {
            AudioManager.Instance.PlayClipAtPoint(skillData.vfxData.castSound, spawnPos, SoundGroup.SFX);
        }

        if (skillData.vfxData.castVFXPrefab == null) return null;
        
        currentCastVFX = Instantiate(skillData.vfxData.castVFXPrefab, spawnPos, Quaternion.identity);
        
        if (skillData.vfxData.attachCastVFXToCharacter)
        {
            currentCastVFX.transform.SetParent(caster);
        }
        
        return currentCastVFX;
    }
    
    // === AREA INDICATOR VFX ===
    public GameObject ShowAreaIndicator(Vector3 position, Vector3 direction)
    {
        if (skillData.vfxData.areaIndicatorVFXPrefab == null) return null;
        
        currentAreaVFX = Instantiate(skillData.vfxData.areaIndicatorVFXPrefab, position, 
                                      Quaternion.LookRotation(direction));
            
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
                
        // Sound
        if (skillData.vfxData.impactSound != null)
        {
            AudioManager.Instance.PlayClipAtPoint(skillData.vfxData.impactSound, position, SoundGroup.SFX);
        }

        return impactVFX;
    }
    
    // === HIT VFX (sur l'ennemi) ===
    public void PlayHitVFX(Vector3 position, int damage)
    {
        if (skillData.vfxData.hitVFXPrefab == null) return;
        Instantiate(skillData.vfxData.hitVFXPrefab, position, Quaternion.identity);
  }
}