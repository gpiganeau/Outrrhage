using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftSlotUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _emptyIndicator;

    private Action<int> _onClicked;
    private int _index;

    public void Init(int index, Action<int> onClicked)
    {
        _index = index;
        _onClicked = onClicked;
        _button.onClick.AddListener(() => _onClicked(_index));
        Clear();
    }

    public void SetSkill(SkillData skill)
    {
        _icon.sprite = skill.Icon;
        _icon.gameObject.SetActive(true);
        _label.text = skill.Name;
        _emptyIndicator.SetActive(false);
    }

    public void Clear()
    {
        _icon.gameObject.SetActive(false);
        _label.text = "Empty";
        _emptyIndicator.SetActive(true);
        SetHighlight(false);
    }

    public void SetHighlight(bool on) => _highlight.SetActive(on);
}