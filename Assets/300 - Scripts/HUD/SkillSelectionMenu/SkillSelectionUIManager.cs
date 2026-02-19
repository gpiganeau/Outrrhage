using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillSelectionUIManager : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private SkillsController _targetSkillsController;
    [SerializeField] private SkillDatabase _skillDatabase;
    [SerializeField] private GameObject _menuPanel;
    
    [Header("UI Containers")]
    [SerializeField] private Transform _currentSkillsContainer;
    [SerializeField] private Transform _availableSkillsContainer;
    [SerializeField] private GameObject _skillSlotPrefab;
    
    [Header("UI Elements")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_Text _instructionsText;
    [SerializeField] private TMP_Text _descriptionText;
    
    [Header("Animation")]
    [SerializeField] private float _animDuration = 0.3f;
    [SerializeField] private Ease _openEase = Ease.OutBack;
    [SerializeField] private Ease _closeEase = Ease.InBack;
    
    private List<SkillSlotUI> _currentSkillSlots = new List<SkillSlotUI>();
    private int _selectedSlotIndex = -1;
    private bool _isOpen = false;
    #endregion
    #region Core Setup
    void Start()
    {
        _closeButton?.onClick.AddListener(CloseMenu);
        _menuPanel.SetActive(false);
        
        UpdateInstructions();
    }
    
    public void SetTargetController(SkillsController controller)
    {
        _targetSkillsController = controller;
    }
    
    public void ToggleMenu()
    {
        if (_isOpen)
            CloseMenu();
        else
            OpenMenu();
    }
    
    public void OpenMenu()
    {
        if (_targetSkillsController == null)
        {
            _targetSkillsController = GameManager.Instance.Riel.GetComponent<SkillsController>();
        }
        
        _isOpen = true;
        _menuPanel.SetActive(true);
        _menuPanel.transform.localScale = Vector3.zero;
        _menuPanel.transform.DOScale(1f, _animDuration).SetEase(_openEase).SetUpdate(true);
        
        Time.timeScale = 0f; // Pause
        
        RefreshUI();
    }
    
    public void CloseMenu()
    {
        _isOpen = false;
        
        _menuPanel.transform.DOScale(0f, _animDuration)
            .SetEase(_closeEase)
            .SetUpdate(true)
            .OnComplete(() => 
            {
                _menuPanel.SetActive(false);
                Time.timeScale = 1f; // Unpause
                HUD.Instance.Refresh();
            });
        
        DeselectSlot();

    }
    
    #endregion
    private void RefreshUI()
    {
        // Clear existing slots
        foreach (Transform child in _currentSkillsContainer)
            Destroy(child.gameObject);
        foreach (Transform child in _availableSkillsContainer)
            Destroy(child.gameObject);
        
        _currentSkillSlots.Clear();
        
        // === CURRENT SKILLS (joueur) ===
        var currentSkills = _targetSkillsController.ActiveSkillStrategies;
        int maxSlots = _targetSkillsController.MaxSkillSlots;
        
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(_skillSlotPrefab, _currentSkillsContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            
            if (i < currentSkills.Count)
            {
                slotUI.SetSkill(currentSkills[i].SkillData, i, true);
            }
            else
            {
                slotUI.SetEmpty(i);
            }
            
            slotUI.OnSlotClicked += OnCurrentSkillSlotClicked;
            slotUI.OnSlotHovered += OnSlotHovered;
            _currentSkillSlots.Add(slotUI);
        }
        
        // === AVAILABLE SKILLS (database) ===
        foreach (var skillData in _skillDatabase.AllSkills)
        {
            GameObject slotObj = Instantiate(_skillSlotPrefab, _availableSkillsContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            slotUI.SetSkill(skillData, -1); // -1 = available skill
            slotUI.OnSlotClicked += OnAvailableSkillClicked;
            slotUI.OnSlotHovered += OnSlotHovered;
        

        }
        
        UpdateInstructions();
    }

    void OnSlotHovered(int slotIndex, SkillData skill, bool isHovering)
    {
        if (isHovering)
        {
            _descriptionText.text = skill != null ? skill.Description : "";
        }
        else
        {
            _descriptionText.text = "";
        }
    }

    
    private void OnCurrentSkillSlotClicked(int slotIndex, SkillData skill, bool isEmpty)
    {

        Logger.Core("Slot index " + slotIndex + " clicked. Skill: " + (skill != null ? skill.Name : "Empty") + ", IsEmpty: " + isEmpty);
        if (_selectedSlotIndex == -1)
        {
            _selectedSlotIndex = slotIndex;
            _currentSkillSlots[slotIndex].SetHighlight(true);
            UpdateInstructions(skill);
        }
        else if (_selectedSlotIndex == slotIndex)
        {
            DeselectSlot();
        }
        else
        {
            _targetSkillsController.SwapSkills(_selectedSlotIndex, slotIndex);
            DeselectSlot();
            RefreshUI();
        }
    }
    
    private void OnAvailableSkillClicked(int _, SkillData skill, bool __)
    {
        if (_selectedSlotIndex != -1)
        {
            // Replace le slot sélectionné
            var currentSkills = _targetSkillsController.ActiveSkillStrategies;
            
            if (_selectedSlotIndex < currentSkills.Count)
            {
                // Slot occupé -> replace
                _targetSkillsController.ReplaceSkill(_selectedSlotIndex, skill);
            }
            else
            {
                // Slot vide -> add
                _targetSkillsController.AddSkill(skill);
            }
            
            DeselectSlot();
            RefreshUI();
        }
        else
        {
            // Aucun slot sélectionné -> ajoute au premier slot vide
            bool added = _targetSkillsController.AddSkill(skill);
            if (added)
            {
                RefreshUI();
            }
            else
            {
                Debug.Log("No empty slots available. Select a slot to replace.");
            }
        }
    }
    
    private void DeselectSlot()
    {
        if (_selectedSlotIndex != -1 && _selectedSlotIndex < _currentSkillSlots.Count)
        {
            _currentSkillSlots[_selectedSlotIndex].SetHighlight(false);
        }
        _selectedSlotIndex = -1;
        UpdateInstructions();
    }
    
    private void UpdateInstructions(SkillData selectedSkill = null)
    {
        if (_instructionsText == null) return;
        
        if (_selectedSlotIndex == -1)
        {
            _instructionsText.text = "Click a skill slot to select it, or click an available skill to add it.";
        }
        else
        {
            _instructionsText.text = $"Slot {_selectedSlotIndex + 1} selected. Click another slot to swap, or click an available skill to replace.";
        }
    }
    
    void OnDestroy()
    {
        _closeButton?.onClick.RemoveListener(CloseMenu);
    }
}