using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _skillData;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _inputPrompt;
    [SerializeField] private Image _cdRotor;
    [SerializeField] private TMP_Text _skillName;
    [SerializeField] private TMP_Text _skillCD;

    private int slotIndex;

    public void Init(SkillStrategy strategy, int index)
    {
        _skillData = strategy.SkillData;
        _icon.sprite = _skillData.Icon;
        _skillName.text = _skillData.Name;
        _skillCD.text = _skillData.Cooldown.ToString();
        slotIndex = index;
        InputManager.Instance.OnDeviceChanged.AddListener(OnDeviceChanged);

        // -- Initial Icon Setup
        UpdateIcon();
    }

    private void OnDeviceChanged(InputDevice device)
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        var sprite = InputManager.Instance.GetSlotInputSprite(slotIndex);
        _inputPrompt.sprite = sprite;
    }

    public void TriggerCooldown()
    {
        _cdRotor.fillAmount = 1;
        _cdRotor.DOFillAmount(0, _skillData.Cooldown).SetEase(Ease.Linear);
    }

    public void SetAvailable(bool value)
    {
        if (_icon == null) return;

        if(value)
        {
            _icon.color = Color.white;
            _skillName.color = Color.white;
            _skillCD.color = Color.white;
        }
        else
        {
            _icon.color = Color.gray;
            _skillName.color = Color.gray;
            _skillCD.color = Color.gray;
        }
    }
}
