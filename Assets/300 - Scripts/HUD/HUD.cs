using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// One and only HUD Instance, static. Work mostly by binding itself to Controllers Events.
/// </summary>
public class HUD : MonoBehaviour
{
    #region Fields
    HUD Instance;

    [Header("References")]
    [SerializeField] SkillBar _skillBar;

    [Header("Controllers")]
    [SerializeField] SkillsController _skillsController;
    [SerializeField] DamageController damageController;

    [Header("Debug")]
    public TMP_Text _rielHealth;
    public TMP_Text _rielBlood;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }
    void OnEnable()
    {
        if (_skillsController != null){
            _skillsController.OnSkillsInitialized += OnSkillsChanged;
            _skillsController.OnSkillExecuted += OnSkillExecuted;

        }
        if (damageController != null) {
            damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }        
    }

    void OnDisable()
    {
        if (_skillsController != null){
            _skillsController.OnSkillsInitialized -= OnSkillsChanged;
            _skillsController.OnSkillExecuted -= OnSkillExecuted;
        }

        if (damageController != null) {
            damageController.OnDamaged.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            damageController.OnHealed.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }
    }

    #endregion

    #region Callbacks
    private void OnSkillsChanged(List<SkillStrategy> strategies)
    {
        _skillBar.Init(strategies);
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
	{
		float healthPercentage = currentHealth / maxHealth * 100;
        _rielHealth.text = $"Riel Health : {currentHealth} / {maxHealth} ({healthPercentage}%)";
    }

    private void OnSkillExecuted(SkillStrategy skill, int slot)
    {
        // -- Todo : Actually we should have events on Blood, and register blood change somewhere.
        Blood b = CharacterComponent.Blood;
        _rielBlood.text = $"Riel Blood : {b.Amount}/{b.Maximum}";
        _skillBar.SetInCooldown(slot);
    } 
    #endregion
}
