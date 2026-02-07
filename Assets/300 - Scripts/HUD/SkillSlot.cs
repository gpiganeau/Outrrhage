using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _skillData;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _cdRotor;
    [SerializeField] private TMP_Text _skillName;
    [SerializeField] private TMP_Text _skillCD;


    public void Init(SkillStrategy strategy)
    {
        _skillData = strategy.SkillData;
        _icon.sprite = _skillData.Icon;
        _skillName.text = _skillData.Name;
        _skillCD.text = _skillData.Cooldown.ToString();
    }

    public void TriggerCooldown()
    {
        _cdRotor.fillAmount = 1;
        _cdRotor.DOFillAmount(0, _skillData.Cooldown);
    }

    public void SetAvailable(bool value)
    {
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
