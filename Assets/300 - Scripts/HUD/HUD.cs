using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// One and only HUD Instance, static. Work mostly by binding itself to Controllers Events.
/// </summary>
public class HUD : MonoBehaviour
{
    #region Fields
    public static HUD Instance;

    [Header("References")]
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] SkillBar _skillBar;
    private SkillsController _skillsController;
    private DamageController _damageController;

    [Header("Resources Gauges")]
    [SerializeField] ResourceGauge _healthGauge;
    [SerializeField] ResourceGauge _bloodGauge;
    [SerializeField] ResourceGauge _rageGauge;

    [Header("Debug")]
    public TMP_Text _rielHealth;
    public TMP_Text _rielBlood;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        // -- Activate HUD so we can hide it in scene while working and still play correctly.
        GetComponent<Canvas>().enabled = true;
    }
    public void Initialize(SkillsController sc, DamageController dc)
    {
        _skillsController = sc;
        _damageController = dc;

        if (_skillsController != null){
            _skillsController.OnSkillsInitialized += OnSkillsChanged;
            _skillsController.OnSkillExecuted += OnSkillExecuted;

        }
        if (_damageController != null) {
            _damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            _damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        } 

        // -- Force Refresh Due to Actual Initialization Races
        OnSkillsChanged(sc.ActiveSkillStrategies);
        CharacterComponent.Blood.OnBloodChanged.AddListener((currentBlood, maxBlood) => OnBloodChanged(currentBlood, maxBlood));
    }
    void OnEnable()
    {
        if (_skillsController != null){
            _skillsController.OnSkillsInitialized += OnSkillsChanged;
            _skillsController.OnSkillExecuted += OnSkillExecuted;

        }
        if (_damageController != null) {
            _damageController.OnDamaged.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            _damageController.OnHealed.AddListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }        
    }

    void OnDisable()
    {
        if (_skillsController != null){
            _skillsController.OnSkillsInitialized -= OnSkillsChanged;
            _skillsController.OnSkillExecuted -= OnSkillExecuted;
        }

        if (_damageController != null) {
            _damageController.OnDamaged.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
            _damageController.OnHealed.RemoveListener((currentHealth, maxHealth) => OnHealthChanged(currentHealth, maxHealth));
        }
        
    }

    public void Hide()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0, 0.5f).OnComplete(() => {
        gameObject.SetActive(false);
        });
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.5f);
    }

    public void ToggleVisibility()
    {
        if (_canvasGroup.alpha > 0.5f) Hide();
        else Show();
    }

    private void Update()
    {
        if (_skillsController != null && _skillBar != null) {
            _skillBar.UpdateAvailability(_skillsController);
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
        _healthGauge.UpdateGauge(currentHealth, maxHealth);

    }

    private void OnBloodChanged(float currentBlood, float maxBlood)
    {
        _bloodGauge.UpdateGauge(currentBlood, maxBlood);
        _rielBlood.text = $"Riel Blood : {currentBlood}/{maxBlood}";
    }

    private void OnSkillExecuted(SkillStrategy skill, int slot)
    {
        if(skill.IsInCooldown)
            _skillBar.SetInCooldown(slot);
    } 
    #endregion
}
