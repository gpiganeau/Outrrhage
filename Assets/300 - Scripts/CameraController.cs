using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _self;
    [SerializeField] private Transform target;
    public Vector3 Up => transform.up;
    public Vector3 Forward => transform.forward;
    public Vector3 Right => transform.right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
