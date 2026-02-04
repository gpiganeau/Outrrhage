using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HealthBarDisplay: MonoBehaviour
{
	private Slider _healthBarSlider;
    public void Initialize(DamageController damageController)
	{
		damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnDamaged(currentHealth, maxHealth));
        damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealed(currentHealth, maxHealth));
		_healthBarSlider = GetComponentInChildren<Slider>();
    }

	private void OnDamaged(float currentHealth, float maxHealth)
	{
		float healthPercentage = currentHealth / maxHealth;
		_healthBarSlider.value = healthPercentage;
    }

	private void OnHealed(float currentHealth, float maxHealth)
	{
		float healthPercentage = currentHealth / maxHealth;
		_healthBarSlider.value = healthPercentage;
    }
}
