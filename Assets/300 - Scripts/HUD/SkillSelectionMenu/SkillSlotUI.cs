using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _highlightObject;
    [SerializeField] private GameObject _emptyStateObject;
    
    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _highlightColor = Color.yellow;
    [SerializeField] private Color _emptyColor = Color.gray;
    
    private SkillData _skillData;
    private int _slotIndex;
    private bool _isEmpty;
    
    public event Action<int, SkillData, bool> OnSlotClicked; // slotIndex, skillData, isEmpty
    public event Action<int, SkillData, bool> OnSlotHovered; // hovered
    
    void Awake()
    {
        _button?.onClick.AddListener(HandleClick);
        SetHighlight(false);
    }
    
    public void SetSkill(SkillData skill, int slotIndex)
    {
        _skillData = skill;
        _slotIndex = slotIndex;
        _isEmpty = false;
        
        if (_iconImage != null)
        {
            _iconImage.sprite = skill.Icon;
            _iconImage.enabled = true;
        }
        
        if (_nameText != null)
            _nameText.text = skill.Name;
        
        if (_backgroundImage != null)
            _backgroundImage.color = _normalColor;
        
        if (_emptyStateObject != null)
            _emptyStateObject.SetActive(false);
    }
    
    public void SetEmpty(int slotIndex)
    {
        _skillData = null;
        _slotIndex = slotIndex;
        _isEmpty = true;
        
        if (_iconImage != null)
            _iconImage.enabled = false;
        
        if (_nameText != null)
            _nameText.text = "Empty Slot";
        
        if (_backgroundImage != null)
            _backgroundImage.color = _emptyColor;
        
        if (_emptyStateObject != null)
            _emptyStateObject.SetActive(true);
    }
    
    public void SetHighlight(bool active)
    {
        if (_highlightObject != null)
        {
            _highlightObject.SetActive(active);
        }
        
        if (_backgroundImage != null)
        {
            _backgroundImage.DOColor(active ? _highlightColor : _normalColor, 0.2f);
        }
    }
    
    private void HandleClick()
    {
        OnSlotClicked?.Invoke(_slotIndex, _skillData, _isEmpty);
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }
    
    void OnDestroy()
    {
        _button?.onClick.RemoveListener(HandleClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSlotHovered?.Invoke(_slotIndex, _skillData, true);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        OnSlotHovered?.Invoke(_slotIndex, _skillData, false);
    }
}