using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DG.Tweening;

internal class PreviewCursor : MonoBehaviour
{
    [SerializeField] private GameObject _visualsContainer;
    private SpriteRenderer _spriteRenderer;

    public void Initialize(Vector3 startingPosition, PreviewData data)
    {
        _spriteRenderer = _visualsContainer.GetComponent<SpriteRenderer>();
        transform.position = startingPosition;
        _visualsContainer.transform.localScale = Vector3.one * data.radius * 2f;
        Show();
    }

    public void SetColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }


    public void Show()
    {
        _visualsContainer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _visualsContainer.gameObject.SetActive(false);
    }

    public void PlayExplosionEffect(Action onComplete = null)
    {
        _spriteRenderer.DOFade(0f, 0.3f);
        _visualsContainer.transform.DOScale(_visualsContainer.transform.localScale * 1.5f, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Hide();
                _visualsContainer.transform.localScale = Vector3.one;
                onComplete?.Invoke();
            });
    }
}

