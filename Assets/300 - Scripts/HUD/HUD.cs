using System.Collections.Generic;
using System.IO.Compression;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SkillBar _skillBar;
    [SerializeField] SkillsController _skillsController;
    [SerializeField] DamageController damageController;

    [Header("Debug")]
    public TMP_Text _rielHealth;

    HUD Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }
    void OnEnable()
    {
        if (_skillsController != null) _skillsController.OnSkillsInitialized += OnSkillsChanged;
        if (damageController != null) {
            damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }        
    }

    void OnDisable()
    {
        if (_skillsController != null) _skillsController.OnSkillsInitialized -= OnSkillsChanged;
        if (damageController != null) {
            damageController.OnDamaged.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            damageController.OnHealed.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }
    }


    public void Initalize()
    {
    }

    private void OnSkillsChanged(List<SkillStrategy> strategies)
    {
        _skillBar.Init(strategies);
    }


    private void OnHealthChanged(float currentHealth, float maxHealth)
	{
		float healthPercentage = currentHealth / maxHealth * 100;
        _rielHealth.text = $"Riel Health : {currentHealth} / {maxHealth} ({healthPercentage}%)";
    }
}
