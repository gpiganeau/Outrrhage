using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class AimPreviewController: MonoBehaviour
{
    [SerializeField] private float _cursorSpeed = 30f;
    [SerializeField] private PreviewCursor _cursor;
    [SerializeField] private Color _startColor = Color.white;
    [SerializeField] private Color _endColor = Color.red;

    bool _isPreviewing;
    Vector3 _previewMovement;
    public Vector3 AimPosition => _cursor.transform.position;
    private DeployType _deployType;

    internal void StartPreview(PreviewData data)
    {
        _isPreviewing = true;
        _deployType = data.deployType;
        _cursor.Initialize(transform.position, data);
        _cursor.transform.position = transform.position;
        _cursor.SetColor(_startColor);
        DOTween.To(
            () => _startColor,
            color => _cursor.SetColor(color),
            _endColor,
            data.timeToDeploy
        );
    }

    internal void HidePreview()
    {
        _cursor.Hide();
        _isPreviewing = false;
    }

    public void UpdatePreviewMovement(Vector3 movement)
    {
        _previewMovement = movement;
    }

    private void FixedUpdate()
    {
        if(_isPreviewing && _previewMovement != Vector3.zero && _deployType == DeployType.Free)
        {
            _cursor.transform.position += _previewMovement.normalized * _cursorSpeed * Time.fixedDeltaTime;
        }
    }

    public void PlayExplosionEffect(Action onComplete = null)
    {
        _cursor.PlayExplosionEffect(onComplete);
    }
}
