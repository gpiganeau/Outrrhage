using DG.Tweening;
using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    [SerializeField] Blood _blood;


    void Start()
    {
        _blood.Initialize();

    }

    public void Attract(GameObject target)
    {
        // -- Spline Movement toward Riel, Trigger on Vacuum ?
        transform.DOMove(target.transform.position, 2f).OnComplete( () => Destroy(this.gameObject));
    }

    void OnTriggerEnter(Collider other)
    {
        // -- Collect on Riel
        if (other.TryGetComponent<CharacterComponent>(out var riel)){
            CharacterComponent.Blood.Regain(_blood.Amount);
            // -- Todo : VFX
            Destroy(this.gameObject);
        }
    }
}
