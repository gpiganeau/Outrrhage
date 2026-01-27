using DG.Tweening;
using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    [SerializeField] Blood _blood;
    private Tween attractTween;
    
    void Start()
    {
        _blood.Initialize();
    }
    
    public void Attract(GameObject target, float duration = 1.2f)
    {
        attractTween?.Kill();
        
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target.transform.position);
        float spiralAngle = Random.Range(0f, 360f);
        float spiralSpeed = Random.Range(600f, 900f);
        
        Sequence attractSequence = DOTween.Sequence();
        
        attractSequence.Append(
            DOVirtual.Float(0f, 1f, duration, value =>
            {
                float t = value;
                float easeT = Mathf.Pow(t, 2f);
                
                spiralAngle += Time.deltaTime * spiralSpeed * (1f - t);
                float currentRadius = distance * (1f - easeT) * 0.6f;
                
                Vector3 offset = new Vector3(
                    Mathf.Cos(spiralAngle * Mathf.Deg2Rad) * currentRadius,
                    Mathf.Sin(spiralAngle * Mathf.Deg2Rad * 2f) * currentRadius * 0.4f,
                    Mathf.Sin(spiralAngle * Mathf.Deg2Rad) * currentRadius
                );
                
                transform.position = Vector3.Lerp(startPos, target.transform.position + offset, easeT);
            })
            .SetEase(Ease.InOutQuad)
        );
        
        attractSequence.Join(
            transform.DOScale(0.3f, duration).SetEase(Ease.InQuad)
        );
        
        attractSequence.OnComplete(() => 
        {
            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        });
        
        attractTween = attractSequence;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterComponent>(out var riel))
        {
            CharacterComponent.Blood.Regain(_blood.Amount);
            
            attractTween?.Kill();
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        attractTween?.Kill();
    }
}