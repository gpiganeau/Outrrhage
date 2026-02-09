using UnityEngine;

public class SkillDataPreviewer : MonoBehaviour
{
    public SkillData skillData;

    private void OnValidate()
    {
        if (skillData != null)
        {
            // Ensure the radius is non-negative
            skillData.Radius = Mathf.Max(0, skillData.Radius);
        }
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
