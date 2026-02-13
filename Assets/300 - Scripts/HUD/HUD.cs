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
        _canvasGroup.DOFade(0, 0.5f).OnComplete(() => _canvasGroup.interactable = false);
    }

    public void Show()
    {
        _canvasGroup.interactable = true;
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
    }

    private void OnSkillExecuted(SkillStrategy skill, int slot)
    {
        // -- Todo : Actually we should have events on Blood, and register blood change somewhere.
        Blood b = CharacterComponent.Blood;
        _rielBlood.text = $"Riel Blood : {b.Amount}/{b.Maximum}";
        if(skill.IsInCooldown)
            _skillBar.SetInCooldown(slot);
    } 
    #endregion
}
