using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _skillData;
    [SerializeField] private Image _icon;
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
        _icon.DOFade(0, 0);
        float halfCD = _skillData.Cooldown * 0.5f;
        DOVirtual.DelayedCall(halfCD, () => _icon.DOFade(1, halfCD));
    }
}
