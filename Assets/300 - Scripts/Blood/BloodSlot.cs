using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BloodSlot : MonoBehaviour {
    //@ Todo : Cool Tween on Spawn ? 

    private Image image;

    [Header("Colors")]
    public Color HiddenColor;
    public Color ShowColor;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void Hide()
    {
        image.DOColor(HiddenColor, 0.2f);
    }

    public void Show()
    {
        image.DOColor(ShowColor, 0.2f);
    }
}