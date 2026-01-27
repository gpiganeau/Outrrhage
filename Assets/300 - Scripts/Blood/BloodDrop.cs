using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    [SerializeField] Blood _blood;


    void Start()
    {
        _blood.Initialize();
    }

    public void Attract()
    {
        // -- Spline Movement toward Riel, Trigger on Vacuum ?
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
