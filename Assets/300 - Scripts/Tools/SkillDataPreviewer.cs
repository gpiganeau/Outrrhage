using UnityEngine;

public class SkillDataPreviewer : MonoBehaviour
{
    public SkillData skillData;
    public SkillVFXController vfxController;

    private void OnValidate()
    {
        if (skillData != null)
        {
            // Ensure the radius is non-negative
            skillData.Radius = Mathf.Max(0, skillData.Radius);

            vfxController = GetComponent<SkillVFXController>();
            vfxController.Initialize(skillData);
        }
    }

// Call this method to preview the skill's effects in the scene 
    public void PreviewSkill(Vector3 position)
    {
        if (Application.isPlaying) return;

        if (skillData == null || vfxController == null)
        {
            Debug.LogWarning("SkillData or SkillVFXController is missing. Cannot preview skill.");
            return;
        }

        var holderPreview = Instantiate(new GameObject("SkillPreviewHolder"), position, Quaternion.identity);
        holderPreview.transform.SetParent(transform);  

        // Skill Projectile
        var prefab = skillData.SkillProjectilePrefab;
        if (prefab != null)
        {
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.transform.SetParent(holderPreview.transform);
        }
        else
        {
            Logger.LogError(Logger.LogCategory.Core, "No Projectile Prefab assigned in the SkillData.");
        }

        // Play casting VFX at the object's position
        vfxController.PlayCastVFX(holderPreview.transform);

        // Show area indicator in front of the object
        Vector3 direction = transform.forward; // Assuming forward is the direction of the skill
        var area = vfxController.ShowAreaIndicator(position, direction);
        area.transform.SetParent(holderPreview.transform);
        
    }

    private void OnDrawGizmos()
    {
        if (skillData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, skillData.Radius);
        }
    }
}
