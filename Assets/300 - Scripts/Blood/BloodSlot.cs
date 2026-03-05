using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BloodSlot : MonoBehaviour {

    private Image image;

    [Header("Colors")]
    public Color HiddenColor;
    public Color ShowColor;

    void Awake()
    {
        image = GetComponent<Image>();
        image.color = HiddenColor;
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        transform.DOScale(0.8f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        image.DOColor(HiddenColor, 0.2f);
        transform.DOScale(0.8f, 0.2f);
    }

    public void Show()
    {
        image.DOColor(ShowColor, 0.2f);
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }
}