using UnityEngine;
using System.Collections;

public class DestructibleObject: MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    // Use this for initialization
    void Start()
	{
        DamageController damageController = GetComponent<DamageController>();
        damageController.Initialize(maxHealth);
        damageController.OnDied.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        Destroy(this.gameObject);
    }
}