using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

internal class PreviewCursor : MonoBehaviour
{
    [SerializeField] private GameObject _visualsContainer;

    public void Initialize(Vector3 startingPosition, PreviewData data)
    {
        transform.position = startingPosition;
        Show();
    }

    public void Show()
    {
        _visualsContainer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _visualsContainer.gameObject.SetActive(false);
    }
}

