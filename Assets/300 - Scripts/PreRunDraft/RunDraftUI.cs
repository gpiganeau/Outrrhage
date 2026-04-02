using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;

public class RunDraftUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillDatabase _skillDatabase;
    [SerializeField] private GameObject _panel;

    [Header("Slots")]
    [SerializeField] private List<DraftSlotUI> _slots;

    [Header("Confirm")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_Text _confirmLabel;
    [SerializeField] private TMP_Text _descriptionLabel;

    private List<SkillData> _selectedSkills = new(); // index = slot
    private int _filledCount = 0;

    public event Action<List<SkillData>> OnDraftConfirmed;

    [Header("Skills Grid")]
    [SerializeField] private Transform _skillsContainer;
    [SerializeField] private GameObject _skillButtonPrefab; // simple prefab : Button + Image + TMP_Text

    [SerializeField] private GameObject _pauseMenu;

    void Awake()
    {
        // Init slots
        for (int i = 0; i < _slots.Count; i++)
        {
            int idx = i;

            _slots[i].Init(idx, OnSlotClicked, (hoverIndex) => {
                if (hoverIndex == -1) { _descriptionLabel.text = ""; return; }
                var skill = _selectedSkills[hoverIndex];
                _descriptionLabel.text = skill != null ? skill.Description : "";
            });
        }

        foreach (var skill in _skillDatabase.AllSkills)
        {
            var go = Instantiate(_skillButtonPrefab, _skillsContainer);
            go.GetComponentInChildren<TMP_Text>().text = skill.Name;
            go.transform.Find("Label").GetComponent<TMP_Text>().text = skill.Name;
            go.transform.Find("Icon").GetComponent<Image>().sprite = skill.Icon;
            go.transform.Find("Icon").GetComponent<Image>().raycastTarget = false; // ← ici

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnSkillPicked(skill));

            var highlight = go.transform.Find("Highlight");
            var listener = btn.gameObject.AddComponent<SelectionListener>();
            listener.OnSelected += () => {
                highlight?.gameObject.SetActive(true);
                _descriptionLabel.text = skill.Description;
            };
            
            listener.OnDeselected += () => {
                highlight?.gameObject.SetActive(false);
                _descriptionLabel.text = "Selectionnez une compétence...";
            };

        }

        _confirmButton.onClick.AddListener(Confirm);
        _confirmButton.interactable = false;
        var l = _confirmButton.gameObject.GetComponent<SelectionListener>();
        l.OnSelected += () => _confirmButton.GetComponent<Image>().color = Color.yellow;
        l.OnDeselected += () => _confirmButton.GetComponent<Image>().color = Color.white;

        // Init liste vide
        for (int i = 0; i < _slots.Count; i++)
            _selectedSkills.Add(null);
    }

    public void Show()
    {
        _panel.SetActive(true);
        _panel.transform.localScale = Vector3.zero;
        _panel.transform.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // ← timeScale = 0 compatible
        
        Time.timeScale = 0f;
        if (_pauseMenu != null) _pauseMenu.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_slots[0].GetButton().gameObject);
    }

    public void Hide()
    {
        Confirm();
    }

    // ── Logique ────────────────────────────────────────────────────

    int _activeSlot = 0; // slot actuellement ciblé

    private void OnSlotClicked(int slotIndex)
    {
        _activeSlot = slotIndex;
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].SetHighlight(i == _activeSlot);
    }

    private void OnSkillPicked(SkillData skill)
    {
        AudioManager.Instance.PlayUIValidate();

        // Évite les doublons
        int existing = _selectedSkills.IndexOf(skill);
        if (existing != -1)
        {
            _selectedSkills[existing] = null;
            _slots[existing].Clear();
        }

        _selectedSkills[_activeSlot] = skill;
        _slots[_activeSlot].SetSkill(skill);

        // Avance au prochain slot vide auto
        int next = FindNextEmptySlot();
        if (next != -1) OnSlotClicked(next);

        RefreshConfirm();
    }

    private int FindNextEmptySlot()
    {
        for (int i = 0; i < _selectedSkills.Count; i++)
            if (_selectedSkills[i] == null) return i;
        return -1;
    }

    private void RefreshConfirm()
    {
        _filledCount = 0;
        foreach (var s in _selectedSkills)
            if (s != null) _filledCount++;

        bool ready = _filledCount == _slots.Count;
        _confirmButton.interactable = ready;
        _confirmLabel.text = ready ? "Commencer !" : $"{_filledCount}/{_slots.Count} skills";
    }

    private void Confirm()
    {
        AudioManager.Instance.PlayUICOnfirm();
         _panel.transform.DOScale(0f, 0.2f)
        .SetEase(Ease.InBack)
        .SetUpdate(true) // ← important si timeScale = 0
        .OnComplete(() =>
        {
            _panel.SetActive(false);
            Time.timeScale = 1f;
            if (_pauseMenu != null) _pauseMenu.SetActive(true);
            OnDraftConfirmed?.Invoke(_selectedSkills);
        });
    }
}